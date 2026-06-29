Option Infer On
Option Strict On
Option Explicit On
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.Gis.Map.Topology
Imports Autodesk.Gis.Map.ObjectData
Imports System.Data

Namespace TPlanGraph
   Public Enum enTopoPurpose
		Undefined = -1
		Parcel = 1
		Approved = 2
		Proposed = 3
		AdditionalA = 9
		Bamash = 11
   End Enum
   Public Enum enOverlayMethod
      Merge = 0
		Union = 1
		Dissolve = 2
   End Enum
	Public Enum enDataOptions
		[Default]
		AcadArea
		CalcMergeArea
		CalcMergeArea2
		RoundedArea
		CalcRoundedArea
		CalcUnionArea
		CalcGroupArea
		CalcGroupArea2

	End Enum
	Public Enum enFormatType
      BlockLayer
      Topology
      Landuse
	End Enum
	Public Enum enGeoMethod
		Undefined
		Topologia
		ClosedPolygons
	End Enum
	Public Class UnionPgonArea

		Private mdaArea(DMAcadExt.enOverlayIndex.OverlayIndexUB) As Double
		Public Property Item(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As Double
			Get
				Return mdaArea(iOverlayIndex)
			End Get
			Set(ByVal dValue As Double)
				mdaArea(iOverlayIndex) = dValue
			End Set
		End Property
		Public Property Item(ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Double
			Get
				Dim iOverlayIndex As DMAcadExt.enOverlayIndex = GetOverlayIndex(iOverlayMethod, iTopoPurpose)
				Return mdaArea(iOverlayIndex)
			End Get
			Set(ByVal dValue As Double)
				Dim iOverlayIndex As DMAcadExt.enOverlayIndex = GetOverlayIndex(iOverlayMethod, iTopoPurpose)
				mdaArea(iOverlayIndex) = dValue
			End Set
		End Property
		Public Shared Function GetOverlayIndex(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As DMAcadExt.enOverlayIndex
			System.Windows.Forms.MessageBox.Show(CStr(bMerge) & vbCrLf & CStr(bUnion) & vbCrLf & CStr(bFDO_Overlay) & vbCrLf & iTopoPurpose.ToString(), "27_472")
			If bMerge AndAlso iTopoPurpose = enTopoPurpose.Approved Then
				Return DMAcadExt.enOverlayIndex.ApprMerge
			ElseIf bMerge AndAlso iTopoPurpose = enTopoPurpose.Proposed Then
				Return DMAcadExt.enOverlayIndex.PropMerge
			ElseIf bUnion AndAlso iTopoPurpose = enTopoPurpose.Approved Then
				Return DMAcadExt.enOverlayIndex.ApprUnion
			ElseIf bUnion AndAlso iTopoPurpose = enTopoPurpose.Proposed Then
				Return DMAcadExt.enOverlayIndex.PropUnion
			ElseIf bFDO_Overlay AndAlso iTopoPurpose = enTopoPurpose.Approved Then
				Return DMAcadExt.enOverlayIndex.ApprFDO_Overlay
			ElseIf bFDO_Overlay AndAlso iTopoPurpose = enTopoPurpose.Proposed Then
				Return DMAcadExt.enOverlayIndex.PropFDO_Overlay
			Else
				Return DMAcadExt.enOverlayIndex.Undefined
			End If
		End Function
		Public Shared Function GetOverlayIndex(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean) As DMAcadExt.enOverlayIndex
			If bMerge And bApproved Then
				Return DMAcadExt.enOverlayIndex.ApprMerge
			ElseIf bMerge And bProposed Then
				Return DMAcadExt.enOverlayIndex.PropMerge
			ElseIf bUnion And bApproved Then
				Return DMAcadExt.enOverlayIndex.ApprUnion
			ElseIf bUnion And bProposed Then
				Return DMAcadExt.enOverlayIndex.PropUnion
			Else
				Return DMAcadExt.enOverlayIndex.Undefined
			End If
		End Function

		Public Shared Function GetOverlayIndex(ByVal tTopoDefID As DMAcadExt.TopoDefID) As DMAcadExt.enOverlayIndex
			Dim bMerge As Boolean = tTopoDefID.TopoIsMerge
			Dim bUnion As Boolean = tTopoDefID.TopoIsUnion
			Dim bFDO_Overlay As Boolean = tTopoDefID.TopoIsFDO_Overlay
			System.Windows.Forms.MessageBox.Show(CStr(bMerge) & vbCrLf & CStr(bUnion) & vbCrLf & CStr(bFDO_Overlay), "27_466")
			Dim iTopoPurpose As DMAcadExt.enTopoPurpose = tTopoDefID.SourceID.BaseID
			System.Windows.Forms.MessageBox.Show(iTopoPurpose.ToString(), "27_466a")
			If iTopoPurpose = enTopoPurpose.Parcel Then
				iTopoPurpose = tTopoDefID.OverlayID.BaseID
			End If
			Return GetOverlayIndex(bMerge, bUnion, bFDO_Overlay, iTopoPurpose)
		End Function
		Public Shared Function GetOverlayIndex(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean) As DMAcadExt.enOverlayIndex

			Dim iOverlayIndex As DMAcadExt.enOverlayIndex
			If bMerge AndAlso bApproved Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.ApprMerge
			ElseIf bMerge AndAlso bProposed Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.PropMerge
			ElseIf bUnion AndAlso bApproved Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.ApprUnion
			ElseIf bUnion AndAlso bProposed Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.PropUnion
			ElseIf bFDO_Overlay AndAlso bApproved Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.ApprFDO_Overlay
			ElseIf bFDO_Overlay AndAlso bProposed Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.PropFDO_Overlay
			End If
			Return iOverlayIndex
		End Function
		Public Shared Function GetOverlayArray(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean) As Boolean()
			Dim iaOverlay(DMAcadExt.enOverlayIndex.OverlayIndexUB) As Boolean
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex
			If bMerge AndAlso bApproved Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.ApprMerge
				iaOverlay(iOverlayIndex) = True
			End If
			If bMerge AndAlso bProposed Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.PropMerge
				iaOverlay(iOverlayIndex) = True
			End If
			If bUnion AndAlso bApproved Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.ApprUnion
				iaOverlay(iOverlayIndex) = True
			End If
			If bUnion AndAlso bProposed Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.PropUnion
				iaOverlay(iOverlayIndex) = True
			End If
			If bFDO_Overlay AndAlso bApproved Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.ApprFDO_Overlay
				iaOverlay(iOverlayIndex) = True
			End If
			If bFDO_Overlay AndAlso bProposed Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.PropFDO_Overlay
				iaOverlay(iOverlayIndex) = True
			End If
			Return iaOverlay
		End Function

		Public Shared Function GetOverlayIndex(ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As DMAcadExt.enOverlayIndex
			If iOverlayMethod = enOverlayMethod.Merge And iTopoPurpose = enTopoPurpose.Approved Then
				Return DMAcadExt.enOverlayIndex.ApprMerge
			ElseIf iOverlayMethod = enOverlayMethod.Merge And iTopoPurpose = enTopoPurpose.Proposed Then
				Return DMAcadExt.enOverlayIndex.PropMerge
			ElseIf iOverlayMethod = enOverlayMethod.Union And iTopoPurpose = enTopoPurpose.Approved Then
				Return DMAcadExt.enOverlayIndex.ApprUnion
			ElseIf iOverlayMethod = enOverlayMethod.Union And iTopoPurpose = enTopoPurpose.Proposed Then
				Return DMAcadExt.enOverlayIndex.PropUnion
			ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay And iTopoPurpose = enTopoPurpose.Approved Then
				Return DMAcadExt.enOverlayIndex.ApprFDO_Overlay
			ElseIf iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay And iTopoPurpose = enTopoPurpose.Proposed Then
				Return DMAcadExt.enOverlayIndex.PropFDO_Overlay
			ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Parcel Then
				Return DMAcadExt.enOverlayIndex.Undefined
			ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Expro Then
				Return DMAcadExt.enOverlayIndex.Undefined

			Else
				MessageBox.Show(iOverlayMethod.ToString & vbCrLf & iTopoPurpose.ToString(), "01_323c")
				Return DMAcadExt.enOverlayIndex.Undefined
			End If
		End Function
		Public Shared Function GetTopoPurpose(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As DMAcadExt.enTopoPurpose
			Select Case iOverlayIndex
				Case DMAcadExt.enOverlayIndex.ApprMerge, DMAcadExt.enOverlayIndex.ApprUnion, DMAcadExt.enOverlayIndex.ApprFDO_Overlay
					Return DMAcadExt.enTopoPurpose.Approved
				Case DMAcadExt.enOverlayIndex.PropMerge, DMAcadExt.enOverlayIndex.PropUnion, DMAcadExt.enOverlayIndex.PropFDO_Overlay
					Return DMAcadExt.enTopoPurpose.Proposed
				Case Else
					Return DMAcadExt.enTopoPurpose.Undefined
			End Select
		End Function
		Public Shared Function GetOverlayMethod(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As DMAcadExt.enOverlayMethod
			Select Case iOverlayIndex
				Case DMAcadExt.enOverlayIndex.ApprMerge, DMAcadExt.enOverlayIndex.PropMerge
					Return DMAcadExt.enOverlayMethod.Merge
				Case DMAcadExt.enOverlayIndex.ApprUnion, DMAcadExt.enOverlayIndex.PropUnion
					Return DMAcadExt.enOverlayMethod.Union
				Case DMAcadExt.enOverlayIndex.ApprFDO_Overlay, DMAcadExt.enOverlayIndex.PropFDO_Overlay
					Return DMAcadExt.enOverlayMethod.FDO_Overlay
				Case Else
					Return Nothing
			End Select
		End Function
		Public Sub Reset()
			For iIndex As Integer = 0 To DMAcadExt.enOverlayIndex.OverlayIndexUB
				mdaArea(iIndex) = 0.0
			Next
		End Sub
		Public Sub Terminate()
			'Erase mdaArea

		End Sub
		Public Sub New()

		End Sub
	End Class

	Public Class TplnProject
#Region "Declarations"
      Public Const XDataAppName As String = "ImportShape"
      Private Const miFormatsUB As Integer = 2
      Private Const miTopoRange As Integer = 128
      Private Const msFormatsSettingKey As String = "Formats"
      Private Const msCoordFormatSettingKey As String = "CoordFormat"
      Private Const msAreaFormatSettingKey As String = "AreaFormat"

      Public Shared UnitScaleFactor As Double = 1000.0
      Public Shared CalcRoundFactor As Double = 1.0

		'Private Shared mdicMapThemes As IDictionary(Of DMAcadExt.enMapTheme, DMAcadExt.MapThemeData)
		Private Shared mdicPlans As TPlanGraph.TplnPlans
      Private Shared mdicLotsAppr As TPlanGraph.TplnLots
      Private Shared mdicLotsProp As TPlanGraph.TplnLots

      Private Shared mdicBlocks As TPlanGraph.TplnBlocks
      Private Shared mdicRegions As TPlanGraph.TplnRegions

      Private Shared mdicParcels As TPlanGraph.TplnParcels
      Private Shared mdicExpros As TPlanGraph.TplnExpros
      Private Shared mdicMerhav As TPlanGraph.TplnMerhavDic
		Private Shared mdicZones As TPlanGraph.TplnZones
		Private Shared mdicOwnershipNotes As TPlanGraph.TplnOwnershipNotes


		Private Shared mdicLusePgonsAppr As IDictionary(Of Integer, TplnLusePgon)
      Private Shared mdicLusePgonsProp As IDictionary(Of Integer, TplnLusePgon)



      Private Shared mdicUnionPgonsAppr As TPlanGraph.TplnOverlayPgons
      Private Shared mdicUnionPgonsProp As TPlanGraph.TplnOverlayPgons

      Private Shared mdicOverlayPgons(DMAcadExt.enOverlayIndex.OverlayIndexUB) As TPlanGraph.TplnOverlayPgons
		'	Private Shared mdicUnionGroups(DMAcadExt.enOverlayIndex.OverlayIndexUB) As TPlanGraph.TplnUnionGroups
		Private Shared mdicOverlayGroups(DMAcadExt.enOverlayIndex.OverlayIndexUB) As TPlanGraph.TplnOverlayGroups
		Private Shared mdicOverlayGroupsNew As TPlanGraph.TplnOverlayGroups

		Private Shared moOverlayTables(DMAcadExt.enOverlayIndex.OverlayIndexUB) As System.Data.DataTable
      Private Shared mdicParcelLotExproPgons As TPlanGraph.TplnOverlayPgons
		Private Shared mdicAddOverlayPgons As TPlanGraph.TplnOverlayPgons

		Private Shared mdicUnionGroupsAppr As TplnUnionGroups
      Private Shared mdicUnionGroupsProp As TplnUnionGroups
      Private Shared moTransaction As Transaction = Nothing
      Private Shared mdicLanduses As DMCommon.ItemDataDict
      Private Shared miLandusesFormatID As Integer = 1
      Private Shared mbInitializedServerDB As Boolean = False
      Private Shared mbInitializedProjectDB As Boolean = False

      Private Shared msServerDataSource As String
      Private Shared msBlockFolder As String

      '   Private Shared msProjectDataSource As String

      '	Private Shared msServerDataBase As String
      Const msServerDataBase As String = "ProjectData" ' "ProjectDataTest"
      Const msProjectDatabase As String = "UD_Projects" ' "ProjectDataTest"


      Private Shared msGushimVectorizedRootPath As String

      Private Shared mbInitializedList As Boolean = False
      '   Private Shared mbNeedCommandLine As Boolean = False
      Private Shared msCoordinateFormat As String
      Private Shared msAreaFormat As String
      Private Shared mdLegendPaintFactor As Double
      Private Shared mtPaintApprLayerDef As DMAcadExt.AcadLayerDef
      Private Shared mtPaintPropLayerDef As DMAcadExt.AcadLayerDef
      Private Shared mtPaintTempLayerDef As DMAcadExt.AcadLayerDef
      Private Shared mtPaintParcelExeptLayerDef As DMAcadExt.AcadLayerDef
      Private Shared mtDrawPLineApprLayerDef As DMAcadExt.AcadLayerDef
      Private Shared mtDrawPLinePropLayerDef As DMAcadExt.AcadLayerDef



      Private Shared moInitView As Autodesk.AutoCAD.DatabaseServices.ViewTableRecord = Nothing
      Private Shared moProjectData As ProjectData

      Private Shared mbIsParcelTopology As Boolean
      Private Shared miParcelGeoMethod As enGeoMethod = enGeoMethod.Undefined
      Private Shared miDefaultTopoPurpose As DMAcadExt.enTopoPurpose = DMAcadExt.enTopoPurpose.Undefined
      Private Shared miOverlayMethod As DMAcadExt.enOverlayMethod = DMAcadExt.enOverlayMethod.Undefined
      Private Shared mbAddOverlayExists As Boolean
      Private Shared mdicMapThemes As IDictionary(Of DMAcadExt.enMapTheme, DMAcadExt.MapThemeData)
      Private Shared mbMerhavExists As Boolean
#End Region
#Region "Shared members"
      'ssssssss
      Public Shared Detail_ID_Control As DMCommon.ID_Control
      Public Shared Landuse_ID_Control As DMCommon.ID_Control
		Public Shared ColorScheme_ID_Control As DMCommon.ID_Control
		Public Shared ExproType_ID_Control As DMCommon.ID_Control
		Public Shared ColorSchemeExpro_ID_Control As DMCommon.ID_Control

		Private Shared miDebugCounterA As Integer
      Public Shared FirstBlueLine As Boolean = True
      ' Public Shared mdicRegions As SortedDictionary(Of Integer, TplnRegion)
      Public Shared mcolLotForcedArea As System.Collections.ObjectModel.Collection(Of BalanceArea.ConstArea)
      Public Shared mcolRegionForcedArea As System.Collections.ObjectModel.Collection(Of BalanceArea.ConstArea)

      Public Shared PlanMapThemeData As DMAcadExt.MapThemeData
      Public Shared ApprMapThemeData As DMAcadExt.MapThemeData
		Public Shared PropMapThemeData As DMAcadExt.MapThemeData
		Public Shared ParcelMapThemeData As DMAcadExt.MapThemeData
		Public Shared BlockMapThemeData As DMAcadExt.MapThemeData

		Public Shared MithamMapThemeData As DMAcadExt.MapThemeData
      Public Shared MithamProxMapThemeData As DMAcadExt.MapThemeData

		Public Shared ParcelApprMapThemeData As DMAcadExt.MapThemeData
		Public Shared ParcelPropMapThemeData As DMAcadExt.MapThemeData


		Public Shared ExproMapThemeData As DMAcadExt.MapThemeData
      Public Shared MerhavMapThemeData As DMAcadExt.MapThemeData
      Public Shared ZoneMapThemeData As DMAcadExt.MapThemeData
		Public Shared ExproZoneMapThemeData As DMAcadExt.MapThemeData
		Public Shared ExproLotThemeData As DMAcadExt.MapThemeData

		Public Shared OwnershipNoteMapThemeData As DMAcadExt.MapThemeData


		Public Shared FragmentMapThemeData As DMAcadExt.MapThemeData

      Public Shared UD_ParcelMapThemeData As DMAcadExt.MapThemeData



      Public Shared TopoPolygons As Dictionary(Of Integer, TopoManager.TPlanGraph.TplnTopoPgon)
		Private Shared moExcelAppExt As DMCommon.ExcelAppExt

		Public Shared Sub InitializeServerDB_SQL()
			Const sBaseServerName As String = TPlServerDB.ServerDB.DBServerName

			Const sDatamapDomain As String = "DM_S1"
         Const sAllaDomain As String = "DELL-ALLA"
         Const sMarinaDomain As String = "MARINAR-M4700"  '\SQLEXPRESS
         'Const sMariannaDomain As String = "DataMap-E5540-2"
         Const sMariannaDomain As String = "DESKTOP-FM1OJA0"

			Dim sCurrentDomainName As String = System.Environment.UserDomainName
			'		Dim msServerDataSource As String	'= "ALLA-DELL\PLUTO"
			'sCurrentDomainName = System.Environment.UserDomainName
			' sCurrentDomainName = sDatamapDomain
			''''''''''   sCurrentDomainName = sAllaDomain

			'  MessageBox.Show(sCurrentDomainName, "11_020")
			'	sCurrentDomainName = sMariannaDomain
			Select Case sCurrentDomainName
            Case sDatamapDomain
               msServerDataSource = sBaseServerName
               Detail_ID_Control = New DMCommon.ID_Control(0, 200)
               Landuse_ID_Control = New DMCommon.ID_Control(DMAcadExt.ColorScheme.MaxStandardID + 1, 8000)
					ColorScheme_ID_Control = New DMCommon.ID_Control(DMAcadExt.ColorScheme.MaxLanduseID + 1, 200000)
					ExproType_ID_Control = New DMCommon.ID_Control(1, 1000)
					ColorSchemeExpro_ID_Control = New DMCommon.ID_Control(1, 1000)

					DMAcadExt.DMPatterns.AcadPatPath = "C:\Program Files\TownPlanner\Support\acad.pat"
               msGushimVectorizedRootPath = "R:\Gushim\Gushim-Vectorized"
               msBlockFolder = "M:\Dm_Work\Blocks"
               'Detail_ID_Control = New DMCommon.ID_Control(201, 300)
            Case sMariannaDomain
					msServerDataSource = sMariannaDomain & "\" & "SQLEXPRESS"   ' DATAMAP-E5540-2\13_105
					Detail_ID_Control = New DMCommon.ID_Control(201, 300)
               Landuse_ID_Control = New DMCommon.ID_Control(8001, 9000)
               ColorScheme_ID_Control = New DMCommon.ID_Control(80001, 90000)
               DMAcadExt.DMPatterns.AcadPatPath = "C:\Program Files\TownPlanner\Support\acad.pat"
               msGushimVectorizedRootPath = "C:\Gushim-Vectorized"
            Case sAllaDomain
               msServerDataSource = sAllaDomain & "\" & sBaseServerName
               msServerDataSource = sBaseServerName
               Detail_ID_Control = New DMCommon.ID_Control(301, 400)
               Landuse_ID_Control = New DMCommon.ID_Control(9001, 10000)
               ColorScheme_ID_Control = New DMCommon.ID_Control(90001, 100000)
               DMAcadExt.DMPatterns.AcadPatPath = "C:\Program Files\TownPlanner\Support\acad.pat"
            Case sMarinaDomain
               msServerDataSource = sMarinaDomain & "\" & "SQLEXPRESS"
               Detail_ID_Control = New DMCommon.ID_Control(401, 500)
               '   Landuse_ID_Control = New DMCommon.ID_Control(8001, 9000)
               '   ColorScheme_ID_Control = New DMCommon.ID_Control(80001, 90000)
               DMAcadExt.DMPatterns.AcadPatPath = "C:\Program Files\TownPlanner\Support\acad.pat"
               msBlockFolder = "C:\DM_Work\Blocks"
            Case Else
               DMAcadExt.DMPatterns.AcadPatPath = "C:\Program Files\TownPlanner\Support\acad.pat"
               MessageBox.Show("Domain was not found" & vbCrLf & "Current domain:'" & sCurrentDomainName & "'", "04_897")
               msServerDataSource = sBaseServerName
         End Select
			'MessageBox.Show(CStr(mbInitializedDB), "07_120")
			'MessageBox.Show(System.Environment.UserDomainName & vbCrLf & sMariannaDomain & vbCrLf & System.Environment.MachineName & vbCrLf & System.Environment.UserName & vbCrLf & msServerDataSource & vbCrLf & msGushimVectorizedRootPath, "11_900")
			'	MessageBox.Show("", "11_200")
			'	If Not mbInitializedServerDB Then
			'	TPlServerDB.ServerDB.InitCurrentProject()
			TPlServerDB.ServerDB.InitCurrentServer()
			'	MessageBox.Show(CStr(msServerDataSource) & ":" & sDatabaseName, "07_140")
			'	TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(msServerDataSource, sDatabaseName, True)
			TPlServerDB.ServerDB.CurrentServerDB.SetSQL(msServerDataSource, msServerDataBase, True, False)
			'	MessageBox.Show(CStr(msServerDataSource) & ":" & sDatabaseName, "07_160")
			If TPlServerDB.ServerDB.CurrentServerDB.State = ConnectionState.Open Then
            mbInitializedServerDB = True
         End If


		End Sub
      Public Shared Sub InitializeProjectDB_SQL(Optional bForce As Boolean = False)
         If bForce OrElse Not mbInitializedProjectDB Then
            '	TPlServerDB.ServerDB.InitCurrentProject()
            TPlServerDB.ServerDB.InitCurrentProject()
				'	MessageBox.Show(CStr(sServerName) & ":" & sDatabaseName, "07_140")
				'	TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(sServerName, sDatabaseName, True)
				TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(msServerDataSource, msProjectDatabase, True, False)
				'	MessageBox.Show(CStr(sServerName) & ":" & sDatabaseName, "07_160")
				If TPlServerDB.ServerDB.CurrentProjectDB.State = ConnectionState.Open Then
               mbInitializedProjectDB = True
            End If
            '	MessageBox.Show(CStr(sServerName) & ":" & sDatabaseName, "07_180")
         End If
      End Sub
      Public Shared Sub InitializeDB_OleDB()




         '	Const sDBResourceFile As String = "\\Zeus\DM_App\Tababuild\Support\tblData.mdb"
         'Const sDBResourceFile As String = "\\Olympus\Project\AppData\TopoSolution\tblData.mdb"
         'Const sDBResourceFile As String = "M:\Tababuild\NetApp\DB\ProjectData.mdb"
         '	Const sProjectFile As String = "D:\NetProjects2010\TopoSolution12\Files\5_R8705_1324-2013\UD8705\udProc.mdb"
         '  Const sDBResourceFile As String = "C:\Program Files\TownPlanner\tblData.accdb"
         Const sDBResourceFile As String = "P:\AppData\TopoSolution\tblData.accdb"
         '	Const sSysDBFile As String = "" '"\\zeus\dm_app\Tababuild\Support\System.mdw"
         '	Const iProvider As TPlServerDB.TPlProvider = TPlServerDB.TPlProvider.ProviderJet
         '	Const sServerName As String = "neptune"
         '	Const sDatabaseName As String = "tblData2009"

         If Not mbInitializedServerDB Then
            'mbInitializedDB = TPlServerDB.ServerDB.Initialize(iProvider, sDBResourceFile, sSysDBFile)
            TPlServerDB.ServerDB.InitCurrentProject()
            TPlServerDB.ServerDB.InitCurrentServer()

				TPlServerDB.ServerDB.CurrentProjectDB.SetOleDB(sDBResourceFile, String.Empty, False)
				TPlServerDB.ServerDB.CurrentServerDB.SetOleDB(sDBResourceFile, String.Empty, False)

				''''''''''''''TPlServerDB.ServerDB.CurrentServerDB.SetSQL(sServerName, sDatabaseName)
				mbInitializedServerDB = TPlServerDB.ServerDB.CurrentServerDB.DBConnectionState <> ConnectionState.Closed
            If mbInitializedServerDB Then
               TPlServerDB.ServerDB.AddInitialize()
            End If
         End If
      End Sub
      Public Shared Function InitializePrjUD_OleDB() As Boolean
         Const sPrjFile As String = "udProc.mdb"
         Try


            Dim oDWGFileInfo As System.IO.FileInfo = New System.IO.FileInfo(DMAcadExt.AcadDocument.GetCurrentDWGName())
            Dim sDBName As String = oDWGFileInfo.DirectoryName & "\" & sPrjFile
            Dim oDBFileInfo As IO.FileInfo = New IO.FileInfo(sDBName)
            If oDBFileInfo.Exists Then
               TPlServerDB.ServerDB.InitCurrentProject()
					TPlServerDB.ServerDB.CurrentProjectDB.SetOleDB(sDBName, String.Empty, False)
					Return True
            Else
               Return False
            End If

         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnProject - InitializePrjUD_OleDB")
            Return False
         End Try

      End Function
      Public Shared ReadOnly Property ServerDataSource As String
         Get
            Return msServerDataSource
         End Get
      End Property
      Public Shared ReadOnly Property BlockFolder As String
         Get
            Return msBlockFolder
         End Get
      End Property

      Public Shared Sub InitAppName()
         Select Case DMAcadExt.DMApp.AppID
            Case DMAcadExt.enApplications.Taba
               Common.AppName = Common.TabaAppName
            Case DMAcadExt.enApplications.TopoMaster
               Common.AppName = Common.TopoMasterAppName
            Case DMAcadExt.enApplications.Unidiv
               Common.AppName = Common.UnidivAppName
         End Select
      End Sub
      Public Shared Sub InitializeList()

         If False Then  'Not mbInitializedList
            Select Case DMAcadExt.DMApp.AppID
               Case DMAcadExt.enApplications.Taba
                  Dim iaFormatsSetting() As Integer = FormatsSetting
                  msCoordinateFormat = CoordinateFormatSetting
                  msAreaFormat = AreaFormatSetting

                  DMAcadExt.AcadBlockDef.Format = iaFormatsSetting(0)
                  DMAcadExt.TopoDef.Format = iaFormatsSetting(1)
                  miLandusesFormatID = iaFormatsSetting(2)
                  DMAcadExt.TopoDef.Format = 1
                  Try
                     mdicLanduses = TPlServerDB.ServerDB.CurrentServerDB.GetItemDict(TPlServerDB.enListType.LanduseM, , , , miLandusesFormatID)
                  Catch oEx As Exception
                     System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnProject - InitializeList_1")
                  End Try

                  mbInitializedList = True
                  If mdicLanduses Is Nothing Then
                     System.Windows.Forms.MessageBox.Show("Landuse list was not found")
                  End If
               Case DMAcadExt.enApplications.Unidiv
                  msCoordinateFormat = CoordinateFormatSetting
                  msAreaFormat = AreaFormatSetting
                  mbInitializedList = True
            End Select
         End If


      End Sub
      Public Shared Sub InitializeView()
         Try
            If moInitView Is Nothing Then
               moInitView = GetEditor().GetCurrentView()
            End If
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnProject - InitializeView")
         End Try

      End Sub
		Public Shared Function GetScales() As System.Collections.Generic.List(Of String)
			Dim oScaleList As System.Collections.Generic.List(Of String) = New System.Collections.Generic.List(Of String)
			Dim sComText As String = "SELECT * FROM Scales ORDER BY ID"
			Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
			If oDataReader IsNot Nothing Then
				While oDataReader.Read
					oScaleList.Add(oDataReader.GetString(1))
				End While
				oDataReader.Close()
				Return oScaleList
			Else
				Return Nothing
			End If
		End Function
		Public Shared Function AddNewElement(ByVal sTableName As String, ByVal iID As Integer, ByVal sName As String) As Boolean
			sName = Replace(sName, "'", "''")
			Dim sComText As String = "INSERT INTO " & sTableName & " (ID,Name) SELECT " & CStr(iID) & ",'" & sName & "'"
			Dim iRes As Integer = TPlServerDB.ServerDB.CurrentServerDB.RunCommand(sComText, CommandType.Text)
			DMCommon.Debug.MsgBox("13_438m", sComText, iRes)
			'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!Element", sComText, iRes)
			Return (iRes = 1)
		End Function

		Public Shared Function AddNewLanduse(ByVal iID As Integer, ByVal sName As String) As Boolean
			sName = Replace(sName, "'", "''")
			Dim sComText As String = "INSERT INTO  Landuses_" & CStr(miLandusesFormatID) & "F (ID,Name) SELECT " & CStr(iID) & ",'" & sName & "'"

			Dim iRes As Integer = TPlServerDB.ServerDB.CurrentServerDB.RunCommand(sComText, CommandType.Text)
			'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!Landuse", sComText, iRes)
			DMCommon.Debug.MsgBox("13_438k", sComText, iRes)
			Return (iRes = 1)
		End Function
		Public Shared Function AddNewExproType(ByVal iID As Integer, ByVal sName As String) As Boolean
			sName = Replace(sName, "'", "''")
			Dim sComText As String = "INSERT INTO ExproTypes (ID,Name) SELECT " & CStr(iID) & ",'" & sName & "'"
			Return (TPlServerDB.ServerDB.CurrentServerDB.RunCommand(sComText, CommandType.Text) = 1)
		End Function
		Public Shared Function UpdateElement(ByVal sTableName As String, ByVal iID As Integer, ByVal sName As String) As Boolean
			sName = DMCommon.Functions.StringToSQL(sName)
			Dim sComText As String = "UPDATE " & sTableName & " SET Name='" & sName & "' WHERE (ID=" & CStr(iID) & ")"
			Return (TPlServerDB.ServerDB.CurrentServerDB.RunCommand(sComText, CommandType.Text) = 1)
		End Function
		Public Shared Function UpdateLanduse(ByVal iID As Integer, ByVal sName As String) As Boolean
         sName = DMCommon.Functions.StringToSQL(sName)
         Dim sComText As String = "UPDATE Landuses_" & CStr(miLandusesFormatID) & "F SET Name='" & sName & "' WHERE (ID=" & CStr(iID) & ")"
         Return (TPlServerDB.ServerDB.CurrentServerDB.RunCommand(sComText, CommandType.Text) = 1)
      End Function
      Public Shared Sub FillLanduses(ByRef oCombo As ComboBox, ByVal sDelim As String, Optional ByVal iDefaultID As Integer = 0)
         Dim sComText As String = "SELECT ID,Name FROM Landuses_" & CStr(miLandusesFormatID) & "F WHERE ID < 200000 ORDER BY ID "
         Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
         Dim oItemData As DMCommon.ItemData
         Dim iLanduseID As Integer
         Dim iDefaultInfex As Integer = 0
         If oDataReader IsNot Nothing Then
            DMAcadExt.AcadDocument.WriteDebugMessage("!#!" & sComText)
            While oDataReader.Read
               iLanduseID = oDataReader.GetInt32(0)

               oItemData = New DMCommon.ItemData(iLanduseID, Convert.ToString(iLanduseID) & sDelim & oDataReader.GetString(1))

               oCombo.Items.Add(oItemData)
               If iLanduseID = iDefaultID Then
                  iDefaultInfex = oCombo.Items.Count - 1
               End If
            End While
            oDataReader.Close()
            oCombo.SelectedIndex = iDefaultInfex
         End If

      End Sub



      Public Shared Sub FillColorSets(ByVal cmbColorSets As ComboBox, Optional ByVal iInitValue As Integer = 0)

         Const sComText As String = "SELECT ID,Name FROM ColorSetList ORDER BY ID"

         Dim oItemData As DMCommon.ItemData
         Dim iID As Integer
         Dim oInitItemData As DMCommon.ItemData = Nothing
         With cmbColorSets
            .Items.Clear()
            .ValueMember = DMCommon.ItemData.ValueMember
            .DisplayMember = DMCommon.ItemData.DisplayMember
            iID = CType(enColorSetType.All, Integer)
            oItemData = New DMCommon.ItemData(iID, DMAcadExt.ColorScheme.AllLanduseName)
            .Items.Add(oItemData)
            If iInitValue = iID Then
               oInitItemData = oItemData
            End If
            iID = CType(enColorSetType.Standard, Integer)
            oItemData = New DMCommon.ItemData(iID, DMAcadExt.ColorScheme.StandardName)
            .Items.Add(oItemData)
            If iInitValue = iID Then
               oInitItemData = oItemData
            End If
            iID = CType(enColorSetType.Local, Integer)
            oItemData = New DMCommon.ItemData(iID, DMAcadExt.ColorScheme.LocalName)
            .Items.Add(oItemData)
            If iInitValue = iID Then
               oInitItemData = oItemData
            End If


            If oInitItemData IsNot Nothing Then
               .SelectedItem = oInitItemData
            End If

            FillCombo(cmbColorSets, sComText, iInitValue)
         End With
      End Sub

      Public Shared Function AddNewColorSet(ByVal sName As String) As Integer
         Dim sComText As String = "SELECT Max(ID) FROM ColorSetList"
         Dim oIDMax As System.Object = TPlServerDB.ServerDB.CurrentServerDB.GetDataScalar(sComText, CommandType.Text)
         Dim iIDNew As Integer
         If oIDMax Is Nothing OrElse IsDBNull(oIDMax) Then
            iIDNew = 11
         Else
            iIDNew = DirectCast(oIDMax, Integer) + 1
         End If
         sName = Strings.Replace(sName, "'", "''")
         sComText = "INSERT INTO ColorSetList (ID,Name) SELECT " & Convert.ToString(iIDNew) & ",'" & sName & "'"
         If TPlServerDB.ServerDB.CurrentServerDB.RunCommand(sComText, CommandType.Text) = 1 Then
            Return iIDNew
         Else
            Return -1
         End If

      End Function
      Public Shared Sub GetAllColorSchemes(ByRef oListView As ListView, ByVal sDelim As String)
         Dim sComText As String = "SELECT * FROM ColorSchemes ORDER BY ID"
         Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
         If oDataReader IsNot Nothing Then
            While oDataReader.Read

				End While
				oDataReader.Close()
			End If

      End Sub


      Public Shared Property ProjectData() As ProjectData
         Get
            Return moProjectData
         End Get
         Set(ByVal oValue As ProjectData)
            moProjectData = oValue
         End Set
      End Property
      Public Shared Function SetPaintLayers(iMapTheme As DMAcadExt.enMapTheme, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As String
         Dim tLayer As DMAcadExt.AcadLayerDef = PaintLayerDef(iMapTheme)
         Dim oTempLayer As DMAcadExt.AcadLayerDef = PaintTempLayerDef
         Dim bOK As Boolean
         bOK = DMAcadExt.AcadTransaction.CreateLayer(tLayer, True)

         If bOK Then
            bOK = DMAcadExt.AcadTransaction.SetCurrentLayer(oTempLayer, True, False, False, True)
            If bOK Then
               Return tLayer.Name
            Else
               Return Nothing
            End If
         Else
            Return Nothing
         End If
      End Function
		Public Shared Function SetPaintLayers(iMapTheme As DMAcadExt.enMapTheme, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByRef sPaintLayer As String, ByRef sPLineLayer As String) As Boolean
			Dim tPaintLayer As DMAcadExt.AcadLayerDef = PaintLayerDef(iMapTheme)
			Dim tPLineLayer As DMAcadExt.AcadLayerDef = DrawPlineLayerDef(iMapTheme)

			Dim oTempLayer As DMAcadExt.AcadLayerDef = PaintTempLayerDef
			Dim bOK As Boolean
			bOK = DMAcadExt.AcadTransaction.CreateLayer(tPaintLayer, True)
			If bOK Then
				bOK = DMAcadExt.AcadTransaction.CreateLayer(tPLineLayer, True)
			End If
			If bOK Then
				bOK = DMAcadExt.AcadTransaction.SetCurrentLayer(tPaintLayer, True, False, False, True)
				If bOK Then
					sPaintLayer = tPaintLayer.Name
					sPLineLayer = tPLineLayer.Name
					Return True
				Else
					Return False
				End If
			Else
				Return False
			End If

		End Function
		Public Shared Sub UpdateLotsByRegions_120226(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, sPlanTopoName As String, slotsTopoName As String)
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
			'   Dim iRow As Integer
			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			Dim oRegionTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(TplnRegion.GetTopoName(), Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			Dim sRegionTopoName As String
			Dim oRegion As TplnRegion = Nothing
			Dim oRegionPgon As Polygon = Nothing
			Dim bProx As Boolean
			sRegionTopoName = TplnRegion.GetTopoName()
			If String.IsNullOrEmpty(sRegionTopoName) Then
				sRegionTopoName = TplnRegion.GetProxTopoName()
				bProx = True
			End If
			oRegionTopology = TopoManager.TopoCreator.GetOpenedTopology(sRegionTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			oRegionTopology = zzGetRegionTopology()
			If (oRegionTopology Is Nothing) AndAlso (Not bProx) Then
				sRegionTopoName = TplnRegion.GetProxTopoName()

				oRegionTopology = TopoManager.TopoCreator.GetOpenedTopology(sRegionTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			End If

			If oRegionTopology IsNot Nothing Then
				'  System.Windows.Forms.MessageBox.Show(dicLots.Count.ToString() & vbCrLf & oRegionTopology.Name & vbCrLf & mdicRegions.Count.ToString(), "07_004")
				For Each oReg As TplnRegion In mdicRegions.Values
					'130126	DMCommon.Debug.ExcelLog.SetNextValue(0, "Reg-n", 1, oReg.TopoID, oReg.RegionNo, oReg.RegionName)
				Next
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!Reg+Lots", mdicRegions.Count, dicLots.Count)
				For Each oLot As TplnLot In dicLots.Values
					'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!LotL", oLot.TopoID, oLot.Name)
					oRegionPgon = Nothing
					Try
						oRegionPgon = oRegionTopology.FindPolygon(oLot.CentroidPoint3d)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						'130126 DMCommon.Debug.ExcelLog.SetValue(0, "!RegExcept", oMapEx.ToString, oLot.Name)
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx, False, "UpdateLotsByRegions-LotPgon", False, "CenterDisPgon=" & DMAcadExt.TPlnPoint.DispPoint(oLot.CentroidPoint3d))
						'	Return
					End Try

					If oRegionPgon IsNot Nothing Then
						'  DMCommon.ExcelLog.SetValue(iRow, 4, oRegionPgon.ID)

						If mdicRegions.TryGetRegion(oRegionPgon.ID, oRegion) Then
							'130126 DMCommon.Debug.ExcelLog.SetValue(5, "!UpdLot", oRegionPgon.ID, oRegion.RegionNo)
							oLot.UpdateRegion(oRegion.RegionNo)
						Else
							'  DMCommon.ExcelLog.SetValue(iRow, 4, "NFnd")
						End If
					Else
						'  DMCommon.ExcelLog.SetValue(iRow, 4, "RegN")
					End If
				Next
				oRegionTopology.Close()
			Else
				System.Windows.Forms.MessageBox.Show("Topology '" & TplnPlan.GetTopoName() & "' was not found", "UpdateLotsByRegions-TplnProject")
			End If

			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()

			DMAcadExt.AcadDocument.CloseMessage()
			DMAcadExt.AcadDocument.Unlock()
			'   DMAcadExt.AcadDocument.CommandLine(True)
			DMAcadExt.AcadDocument.UpdateScreen()
		End Sub
		Public Shared Sub UpdateLotsByRegions(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, sPlanTopoName As String, sLotTopoName As String)
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
			'   Dim iRow As Integer
			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			Dim oRegionTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(TplnRegion.GetTopoName(), Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			Dim sRegionTopoName As String
			Dim oRegion As TplnRegion = Nothing
			Dim oRegionPgon As Polygon = Nothing
			Dim bProx As Boolean
			sRegionTopoName = TplnRegion.GetTopoName()
			If String.IsNullOrEmpty(sRegionTopoName) Then
				sRegionTopoName = TplnRegion.GetProxTopoName()
				bProx = True
			End If
			oRegionTopology = TopoManager.TopoCreator.GetOpenedTopology(sRegionTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			oRegionTopology = zzGetRegionTopology()
			If (oRegionTopology Is Nothing) AndAlso (Not bProx) Then
				sRegionTopoName = TplnRegion.GetProxTopoName()

				oRegionTopology = TopoManager.TopoCreator.GetOpenedTopology(sRegionTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			End If

			If oRegionTopology IsNot Nothing Then
				'  System.Windows.Forms.MessageBox.Show(dicLots.Count.ToString() & vbCrLf & oRegionTopology.Name & vbCrLf & mdicRegions.Count.ToString(), "07_004")
				For Each oReg As TplnRegion In mdicRegions.Values
					'130126	DMCommon.Debug.ExcelLog.SetNextValue(0, "Reg-n", 1, oReg.TopoID, oReg.RegionNo, oReg.RegionName)
				Next
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!Reg+Lots", mdicRegions.Count, dicLots.Count)
				For Each oLot As TplnLot In dicLots.Values
					'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!LotL", oLot.TopoID, oLot.Name)
					oRegionPgon = Nothing
					Try
						oRegionPgon = oRegionTopology.FindPolygon(oLot.CentroidPoint3d)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						'130126 DMCommon.Debug.ExcelLog.SetValue(0, "!RegExcept", oMapEx.ToString, oLot.Name)
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx, False, "UpdateLotsByRegions-LotPgon", False, "CenterDisPgon=" & DMAcadExt.TPlnPoint.DispPoint(oLot.CentroidPoint3d))
						'	Return
					End Try

					If oRegionPgon IsNot Nothing Then
						'  DMCommon.ExcelLog.SetValue(iRow, 4, oRegionPgon.ID)

						If mdicRegions.TryGetRegion(oRegionPgon.ID, oRegion) Then
							'130126 DMCommon.Debug.ExcelLog.SetValue(5, "!UpdLot", oRegionPgon.ID, oRegion.RegionNo)
							oLot.UpdateRegion(oRegion.RegionNo)
						Else
							'  DMCommon.ExcelLog.SetValue(iRow, 4, "NFnd")
						End If
					Else
						'  DMCommon.ExcelLog.SetValue(iRow, 4, "RegN")
					End If
				Next
				oRegionTopology.Close()
			Else
				System.Windows.Forms.MessageBox.Show("Topology '" & TplnPlan.GetTopoName() & "' was not found", "UpdateLotsByRegions-TplnProject")
			End If

			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()

			DMAcadExt.AcadDocument.CloseMessage()
			DMAcadExt.AcadDocument.Unlock()
			'   DMAcadExt.AcadDocument.CommandLine(True)
			DMAcadExt.AcadDocument.UpdateScreen()
		End Sub

		Public Shared Sub UpdateLotsByRegions(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose)
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)
			'   Dim iRow As Integer
			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			Dim oRegionTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(TplnRegion.GetTopoName(), Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			Dim sRegionTopoName As String
			Dim oRegion As TplnRegion = Nothing
			Dim oRegionPgon As Polygon = Nothing
			Dim bProx As Boolean
			sRegionTopoName = TplnRegion.GetTopoName()
			If String.IsNullOrEmpty(sRegionTopoName) Then
				sRegionTopoName = TplnRegion.GetProxTopoName()
				bProx = True
			End If
			oRegionTopology = TopoManager.TopoCreator.GetOpenedTopology(sRegionTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			oRegionTopology = zzGetRegionTopology()
			If (oRegionTopology Is Nothing) AndAlso (Not bProx) Then
				sRegionTopoName = TplnRegion.GetProxTopoName()

				oRegionTopology = TopoManager.TopoCreator.GetOpenedTopology(sRegionTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			End If

			If oRegionTopology IsNot Nothing Then
				'  System.Windows.Forms.MessageBox.Show(dicLots.Count.ToString() & vbCrLf & oRegionTopology.Name & vbCrLf & mdicRegions.Count.ToString(), "07_004")
				For Each oReg As TplnRegion In mdicRegions.Values
					'130126	DMCommon.Debug.ExcelLog.SetNextValue(0, "Reg-n", 1, oReg.TopoID, oReg.RegionNo, oReg.RegionName)
				Next
				DMCommon.Debug.ExcelLog.SetNextValue(0, "!Reg+Lots", mdicRegions.Count, dicLots.Count)
				For Each oLot As TplnLot In dicLots.Values
					'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!LotL", oLot.TopoID, oLot.Name)
					oRegionPgon = Nothing
					Try
						oRegionPgon = oRegionTopology.FindPolygon(oLot.CentroidPoint3d)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						'130126 DMCommon.Debug.ExcelLog.SetValue(0, "!RegExcept", oMapEx.ToString, oLot.Name)
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx, False, "UpdateLotsByRegions-LotPgon", False, "CenterDisPgon=" & DMAcadExt.TPlnPoint.DispPoint(oLot.CentroidPoint3d))
						'	Return
					End Try

					If oRegionPgon IsNot Nothing Then
						'  DMCommon.ExcelLog.SetValue(iRow, 4, oRegionPgon.ID)

						If mdicRegions.TryGetRegion(oRegionPgon.ID, oRegion) Then
							'130126 DMCommon.Debug.ExcelLog.SetValue(5, "!UpdLot", oRegionPgon.ID, oRegion.RegionNo)
							oLot.UpdateRegion(oRegion.RegionNo)
						Else
							'  DMCommon.ExcelLog.SetValue(iRow, 4, "NFnd")
						End If
					Else
						'  DMCommon.ExcelLog.SetValue(iRow, 4, "RegN")
					End If
				Next
				oRegionTopology.Close()
			Else
				System.Windows.Forms.MessageBox.Show("Topology '" & TplnPlan.GetTopoName() & "' was not found", "UpdateLotsByRegions-TplnProject")
			End If

			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()

			DMAcadExt.AcadDocument.CloseMessage()
			DMAcadExt.AcadDocument.Unlock()
			'   DMAcadExt.AcadDocument.CommandLine(True)
			DMAcadExt.AcadDocument.UpdateScreen()
		End Sub

		Public Shared Sub UpdateLotsByPlans_120226(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose)
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)

			'MessageBox.Show(dicLots.Count.ToString(), TplnPlan.GetTopoName(), "13_820d")

			Dim oPlanTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(TplnPlan.GetTopoName(), Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			Dim oPlan As TplnPlan

			Dim oPlanPgon As Polygon = Nothing
			If oPlanTopology IsNot Nothing Then
				'  System.Windows.Forms.MessageBox.Show(dicLots.Count.ToString(), "TplnProject - SetInitView")
				For Each oLot As TplnLot In dicLots.Values
					Try
						oPlanPgon = oPlanTopology.FindPolygon(oLot.CentroidPoint3d)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "LoadLusePgons-LotPgon", False, "CenterDisPgon=" & DMAcadExt.TPlnPoint.DispPoint(oLot.CentroidPoint3d))
						'	Return
					End Try

					If oPlanPgon IsNot Nothing Then
						oPlan = GetPlan(oPlanPgon.ID, "DDDD")
						If oPlan IsNot Nothing Then
							oLot.UpdatePlanName(oPlan.Name)
						End If
					End If
				Next
				oPlanTopology.Close()
			Else
				System.Windows.Forms.MessageBox.Show("Topology '" & TplnPlan.GetTopoName() & "' was not found", "TplnProject - UpdateLotsByPlans")
			End If
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()

			DMAcadExt.AcadDocument.CloseMessage()
			DMAcadExt.AcadDocument.Unlock()
			'   DMAcadExt.AcadDocument.CommandLine(True)
			DMAcadExt.AcadDocument.UpdateScreen()
		End Sub


		Public Shared Sub UpdateLotsByPlans(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, sPlanTopoName As String, sLotTopoName As String)
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, True)

			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			If dicLots IsNot Nothing Then
				DMCommon.Debug.MsgBox("13_808k", dicLots.Count)
			Else
				DMCommon.Debug.MsgBox("13_808l", "dicLots Is Nothing")
			End If

			'MessageBox.Show(dicLots.Count.ToString(), TplnPlan.GetTopoName(), "13_820d")

			Dim oPlanTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sPlanTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			Dim oPlan As TplnPlan

			Dim oPlanPgon As Polygon = Nothing
			If oPlanTopology IsNot Nothing Then
				'  System.Windows.Forms.MessageBox.Show(dicLots.Count.ToString(), "TplnProject - SetInitView")
				moExcelAppExt = New DMCommon.ExcelAppExt()
				moExcelAppExt.Open()
				For Each oLot As TplnLot In dicLots.Values
					Try
						oPlanPgon = oPlanTopology.FindPolygon(oLot.CentroidPoint3d)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "LoadLusePgons-LotPgon", False, "CenterDisPgon=" & DMAcadExt.TPlnPoint.DispPoint(oLot.CentroidPoint3d))
						'	Return
					End Try

					If oPlanPgon IsNot Nothing Then
						oPlan = GetPlan(oPlanPgon.ID, "DDDD")
						If oPlan IsNot Nothing Then
							oLot.UpdatePlanName(oPlan.Name)
						End If
					End If
				Next
				oPlanTopology.Close()
			Else
				System.Windows.Forms.MessageBox.Show("Topology '" & TplnPlan.GetTopoName() & "' was not found", "TplnProject - UpdateLotsByPlans")
			End If
			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()

			DMAcadExt.AcadDocument.CloseMessage()
			DMAcadExt.AcadDocument.Unlock()
			'   DMAcadExt.AcadDocument.CommandLine(True)
			DMAcadExt.AcadDocument.UpdateScreen()
		End Sub
		Public Shared Sub FillScales(ByRef oCombo As ComboBox)
         Dim sComText As String = "SELECT * FROM Scales ORDER BY ID"
         Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
         If oDataReader IsNot Nothing Then
            While oDataReader.Read
               oCombo.Items.Add(oDataReader.GetString(1))
            End While
            oDataReader.Close()
         End If
      End Sub
      Public Shared Sub FillScalesA(ByRef oCombo As ComboBox, Optional ByVal iInitValue As Integer = 0)
         Dim sComText As String = "SELECT * FROM Scales ORDER BY ID"
         FillCombo(oCombo, sComText)

      End Sub
      Public Shared Sub FillCombo(ByRef oCombo As ComboBox, ByVal sComText As String, Optional ByVal iInitValue As Integer = 0)

			Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
         Dim oItemData As DMCommon.ItemData
         Dim iID As Integer
         Dim oInitItemData As DMCommon.ItemData = Nothing
         With oCombo
            .ValueMember = DMCommon.ItemData.ValueMember
            .DisplayMember = DMCommon.ItemData.DisplayMember
            If oDataReader IsNot Nothing Then
               While oDataReader.Read
                  iID = oDataReader.GetInt32(0)
                  oItemData = New DMCommon.ItemData(iID, oDataReader.GetString(1))
                  .Items.Add(oItemData)
                  If iInitValue = iID Then
                     oInitItemData = oItemData
                  End If
               End While
               oDataReader.Close()
            End If
            If oInitItemData IsNot Nothing Then
               .SelectedItem = oInitItemData
            End If
         End With
      End Sub
      Public Shared Sub FillLineWeights(ByRef oCombo As ComboBox)
         Dim sComText As String = "SELECT * FROM LineWeights ORDER BY Value"
         Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
         If oDataReader IsNot Nothing Then
            While oDataReader.Read
               oCombo.Items.Add(oDataReader.GetInt32(0))
            End While
            oDataReader.Close()
         End If

      End Sub

      Public Shared Sub SetInitView()
         Try
            GetEditor().SetCurrentView(moInitView)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnProject - SetInitView")

         End Try

      End Sub
      Public Shared Sub Close()
         If mdicLotsAppr IsNot Nothing Then
            mdicLotsAppr.Terminate()
            mdicLotsAppr.Clear()
            mdicLotsAppr = Nothing
         End If
         If mdicLotsProp IsNot Nothing Then
            mdicLotsProp.Terminate()
            mdicLotsProp.Clear()
            mdicLotsProp = Nothing
         End If
         If mdicBlocks IsNot Nothing Then
            mdicBlocks.Terminate()
            mdicBlocks.Clear()
            mdicBlocks = Nothing
         End If

         If mdicParcels IsNot Nothing Then
            mdicParcels.Terminate()
            mdicParcels = Nothing
         End If

         If mdicUnionPgonsAppr IsNot Nothing Then
            mdicUnionPgonsAppr.Terminate()
            mdicUnionPgonsAppr = Nothing
         End If

         If mdicUnionPgonsProp IsNot Nothing Then
            mdicUnionPgonsProp.Terminate()
            mdicUnionPgonsProp = Nothing
         End If
         TPlanGraph.TplnParcel.SharedTerminate()
         TPlanGraph.TplnLot.SharedTerminate()
         TPlanGraph.TplnBlock.SharedTerminate()

         mbInitializedServerDB = False
         mbInitializedProjectDB = False

         mbInitializedList = False

         TPlServerDB.ServerDB.CurrentServerDB.Close()
         If TPlServerDB.ServerDB.CurrentProjectDB IsNot Nothing Then
            TPlServerDB.ServerDB.CurrentProjectDB.Close()
         End If


      End Sub
      Public Shared Sub CalculateOldVer(ByVal bApproved As Boolean, ByVal bProposed As Boolean, ByVal bParcel As Boolean, ByVal bParcelClPgon As Boolean, ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean)

         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, False)
         DMAcadExt.AcadTransaction.Start()
         '	DMAcadExt.AcadDocument.OpenLog()

         If bApproved OrElse bProposed Then
				TPlanGraph.TplnLot.InitializeOldVer()
			End If

         If bApproved Then
            TplnLot.Reset(enTopoPurpose.Approved)
            bApproved = LoadLotsOldVer(DMAcadExt.enTopoPurpose.Approved)
         Else
            TplnLot.Dispose(enTopoPurpose.Approved)
         End If

         If bProposed Then
            TplnLot.Reset(enTopoPurpose.Proposed)
            bProposed = LoadLotsOldVer(DMAcadExt.enTopoPurpose.Proposed)
         Else
            TplnLot.Dispose(enTopoPurpose.Proposed)
         End If

         '	MessageBox.Show(miParcelGeoMethod.ToString(), "02_980")
         If bParcel Then
            TplnParcel.Initialize()
            TplnParcel.Reset()
            InitBlockDic()
            If False Then  '150413
               If miParcelGeoMethod = enGeoMethod.Topologia Then
						bParcel = LoadParcelsOldVer()  '  LoadParcelsCP()	 
					End If

               If miParcelGeoMethod = enGeoMethod.ClosedPolygons Then
                  bParcel = LoadParcelsCP()
                  LoadCentroids()
               End If
            End If

            If bParcelClPgon Then
               bParcel = LoadParcelsCP()
               LoadCentroids()
            Else
					bParcel = LoadParcelsOldVer()
				End If
            '

            If True Then
               LoadBlocksOldVer()
            End If
            '   TPlanGraph.TplnProject.CheckLegalArea()
         End If

         If True Then
            If bMerge AndAlso bApproved AndAlso (mdicParcels IsNot Nothing) Then
               DMAcadExt.AcadDocument.WriteDebugMessage("bMerge And bApproved")
               bMerge = TPlanGraph.TplnProject.LoadMerge(DMAcadExt.enTopoPurpose.Approved)
            End If

            If bMerge AndAlso bProposed AndAlso (mdicParcels IsNot Nothing) Then
               DMAcadExt.AcadDocument.WriteDebugMessage("bMerge And bProposed")
               bMerge = TPlanGraph.TplnProject.LoadMerge(DMAcadExt.enTopoPurpose.Proposed)
            End If

            If bUnion AndAlso bApproved Then
               DMAcadExt.AcadDocument.WriteDebugMessage("bUnion And bApproved")
               TPlanGraph.TplnProject.LoadUnion(DMAcadExt.enTopoPurpose.Approved)
            End If

            If bUnion AndAlso bProposed Then
               DMAcadExt.AcadDocument.WriteDebugMessage("bUnion And bProposed")
               TPlanGraph.TplnProject.LoadUnion(DMAcadExt.enTopoPurpose.Proposed)
            End If
            If False Then   '150413
               If bFDO_Overlay AndAlso bApproved AndAlso (mdicParcels IsNot Nothing) Then
                  If miParcelGeoMethod = enGeoMethod.Topologia Then
                     bFDO_Overlay = TPlanGraph.TplnProject.LoadFDO_OverlayOldVer(DMAcadExt.enTopoPurpose.Approved)  'TPlanGraph.TplnProject.LoadFDO_OverlayCP(DMAcadExt.enTopoPurpose.Approved)
                  End If
                  If miParcelGeoMethod = enGeoMethod.ClosedPolygons Then
                     bFDO_Overlay = TPlanGraph.TplnProject.LoadFDO_OverlayCP(DMAcadExt.enTopoPurpose.Approved)  'TPlanGraph.TplnProject.LoadFDO_OverlayCP(DMAcadExt.enTopoPurpose.Approved)
                  End If
               End If

               If bFDO_Overlay AndAlso bProposed AndAlso (mdicParcels IsNot Nothing) Then
                  bFDO_Overlay = TPlanGraph.TplnProject.LoadFDO_OverlayOldVer(DMAcadExt.enTopoPurpose.Proposed)
               End If
            End If
            If bFDO_Overlay AndAlso bApproved AndAlso (mdicParcels IsNot Nothing) Then
               If bParcelClPgon Then
                  bFDO_Overlay = TPlanGraph.TplnProject.LoadFDO_OverlayCP(DMAcadExt.enTopoPurpose.Approved)
               Else
                  bFDO_Overlay = TPlanGraph.TplnProject.LoadFDO_OverlayOldVer(DMAcadExt.enTopoPurpose.Approved)
               End If
            End If
            If bFDO_Overlay AndAlso bProposed AndAlso (mdicParcels IsNot Nothing) Then
               If bParcelClPgon Then
                  bFDO_Overlay = TPlanGraph.TplnProject.LoadFDO_OverlayCP(DMAcadExt.enTopoPurpose.Proposed)
               Else
                  bFDO_Overlay = TPlanGraph.TplnProject.LoadFDO_OverlayOldVer(DMAcadExt.enTopoPurpose.Proposed)
               End If
            End If
         End If
         'Balance
         '''''	System.Windows.Forms.MessageBox.Show("", "PrjCalc_0360")

         If bParcel Then
				'Balance
				TPlanGraph.TplnProject.CalculateParcels(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed, False)
			End If
         If False Then  'Debug

            Try
               Dim oAllOverlayGroups As TPlanGraph.TplnOverlayGroups = TPlanGraph.TplnProject.OverlayGroups(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
               Dim oOverlayGroup As TPlanGraph.TplnOverlayGroup = oAllOverlayGroups.GetItem(TPlanGraph.TplnParcel.ID_Debug, TPlanGraph.TplnLot.ID_Debug)
               Dim tAreaSet As TPlanGraph.TplnAreaSet = oOverlayGroup.AreaSet
               DMAcadExt.AcadDocument.WriteMessage("X##- " & tAreaSet.ToString())
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "Debug 160")
            End Try
         End If


         If True Then
				'Calculate 2
				If mdicRegions.Count > 1 Then
				Else
					CalculateOverlayGroups(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
				End If


			End If
         System.Windows.Forms.MessageBox.Show("Stage 1" & vbCrLf & "" & vbCrLf & "", "04_011")

         '		qqqqqqqqqqqqqqqqqqqqqqqqqq
         If True Then
            If bParcel Then ''''''''''''gggggggggggg
					TPlanGraph.TplnProject.UpdateParcelTable(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed, False)   '''''''''''''''''''''''''''''   02/06/09
					'System.Windows.Forms.MessageBox.Show("", "PrjCalc_0400")
				End If
         End If

         If True Then   '''''''''''TEMP FALSE

            If bApproved Then
               TPlanGraph.TplnProject.UpdateLotTable(bMerge, bUnion, bFDO_Overlay, DMAcadExt.enTopoPurpose.Approved)
               LoadLusePgons(DMAcadExt.enTopoPurpose.Approved)
            End If
            If bProposed Then
               TPlanGraph.TplnProject.UpdateLotTable(bMerge, bUnion, bFDO_Overlay, DMAcadExt.enTopoPurpose.Proposed)
               LoadLusePgons(DMAcadExt.enTopoPurpose.Proposed)
            End If

            ''''''''''''''bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb
         End If

         If True Then
            Dim bOverlayExists As Boolean = False
            If bMerge Then
               TplnLot.CalcLanduses(False, DMAcadExt.enOverlayMethod.Merge)
               bOverlayExists = True
            End If
            If bUnion Then
               TplnLot.CalcLanduses(False, DMAcadExt.enOverlayMethod.Union)
               bOverlayExists = True
            End If
            If bFDO_Overlay Then
               TplnLot.CalcLanduses(False, DMAcadExt.enOverlayMethod.FDO_Overlay)
               bOverlayExists = True
            End If
            If Not bOverlayExists Then
               TplnLot.CalcLanduses(False, DMAcadExt.enOverlayMethod.Merge)
            End If
            ''''''''''''''''''''''''''''''''aaaaaaaaaaaaaa 

            If bParcel AndAlso bMerge Then  'FFFFFFFFFFFFFFFFFFFFFFFFFF

               If bApproved Then
                  TPlanGraph.TplnParcel.CalculateLotsContent(DMAcadExt.enOverlayIndex.ApprMerge)
                  UpdateOverlayTable(DMAcadExt.enOverlayIndex.ApprMerge)
                  TPlanGraph.TplnParcel.CalculateBlocks(DMAcadExt.enOverlayIndex.ApprMerge)
               End If
               If bProposed Then
                  TPlanGraph.TplnParcel.CalculateLotsContent(DMAcadExt.enOverlayIndex.PropMerge)
                  UpdateOverlayTable(DMAcadExt.enOverlayIndex.PropMerge)
                  TPlanGraph.TplnParcel.CalculateBlocks(DMAcadExt.enOverlayIndex.PropMerge)
               End If
            End If

            If bParcel AndAlso bFDO_Overlay Then  'FFFFFFFFFFFFFFFFFFFFFFFFFF
               '''''''''''' Temp	TPlanGraph.TplnParcel.CalculateBlocks()
               If bApproved Then
                  TPlanGraph.TplnParcel.CalculateLotsContent(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
                  UpdateOverlayTable(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
                  TPlanGraph.TplnParcel.CalculateBlocks(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
               End If
               If bProposed Then
                  TPlanGraph.TplnParcel.CalculateLotsContent(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
                  UpdateOverlayTable(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
                  TPlanGraph.TplnParcel.CalculateBlocks(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
               End If
            End If
         End If
         '	System.Windows.Forms.MessageBox.Show("", "PrjCalc_0990")
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.CloseMessage()
         DMAcadExt.AcadDocument.Unlock()
         DMAcadExt.AcadDocument.CloseLog()
         '   RestoreCommandLine()
      End Sub
		'  Public Shared Sub CalculateNew(tPlanMapThemeData As DMAcadExt.MapThemeData, tApprMapThemeData As DMAcadExt.MapThemeData, tPropMapThemeData As DMAcadExt.MapThemeData, tParcelMapThemeData As DMAcadExt.MapThemeData, tBlockMapThemeData As DMAcadExt.MapThemeData, tExproMapThemeData As DMAcadExt.MapThemeData, tMerhavMapThemeData As DMAcadExt.MapThemeData, iRegion As Integer)
		Public Shared Sub Calculate()
			Dim bPlan As Boolean, bApproved As Boolean, bProposed As Boolean, bParcel As Boolean, bBlock As Boolean, bMitham As Boolean, bMithamProx As Boolean, bExpro As Boolean, bMerhav As Boolean, bZone As Boolean, bExproZoneOverlay As Boolean, bExproLotOverlay As Boolean, bFragment As Boolean, bUd_Parcel As Boolean, bOwnershipNote As Boolean
			Dim bMerge As Boolean, bUnion As Boolean, bFDO_Overlay As Boolean
			bPlan = PlanMapThemeData.IsNotEmpty
			bApproved = ApprMapThemeData.IsNotEmpty
			bProposed = PropMapThemeData.IsNotEmpty
			bExpro = ExproMapThemeData.IsNotEmpty
			bParcel = ParcelMapThemeData.IsNotEmpty

			bBlock = BlockMapThemeData.IsNotEmpty
			bMitham = MithamMapThemeData.IsNotEmpty
			bMithamProx = MithamProxMapThemeData.IsNotEmpty

			bMerhav = MerhavMapThemeData.IsNotEmpty
			bZone = ZoneMapThemeData.IsNotEmpty
			bExproZoneOverlay = ExproZoneMapThemeData.IsNotEmpty
			bExproLotOverlay = ExproLotThemeData.IsNotEmpty
			bFragment = FragmentMapThemeData.IsNotEmpty

			bUd_Parcel = UD_ParcelMapThemeData.IsNotEmpty
			bOwnershipNote = OwnershipNoteMapThemeData.IsNotEmpty

			mbMerhavExists = bMerhav

			If bOwnershipNote Then
				TplnParcel.HasOwnershipNotes = True
			End If
			'	DMCommon.Debug.MsgBox("13_120c", bOwnershipNote, TplnParcel.HasOwnershipNotes)
			Dim bLanduseByPolygons As Boolean = False

			'bExpro = tExproMapThemeData.IsNotEmpty  ''''''''''''''''NB!
			'  MessageBox.Show(CStr(bApproved) & ":" & CStr(bFDO_Overlay) & ":" & CStr(mdicParcels IsNot Nothing) & vbCrLf & miParcelGeoMethod.ToString(), "04_348d")
			If miOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
				bMerge = True
			ElseIf miOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
				bFDO_Overlay = True
			End If


			'MessageBox.Show("CalculateNew" & vbCrLf & "Parcel: " & miParcelGeoMethod.ToString(), "10_120")
			'		MessageBox.Show(CStr(tApprMapThemeData.IsNotEmpty) & ":" & CStr(tPropMapThemeData.IsNotEmpty) & ":" & CStr(tParcelMapThemeData.IsNotEmpty) & vbCrLf & miOverlayMethod.ToString(), "04_346x")
			'  MessageBox.Show(CStr(bApproved) & ":" & CStr(bProposed) & ":" & CStr(bParcel) & vbCrLf & CStr(bExpro) & ":" & CStr(bMerhav) & vbCrLf & CStr(bMerge) & ":" & CStr(bUnion) & ":" & CStr(bFDO_Overlay) & ":" & CStr(999), "04_348y")
			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, False)
			DMAcadExt.AcadTransaction.Start()
			DMAcadExt.AcadTransaction.OpenModelSpace(Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False)
			DMAcadExt.AcadTransaction.OpenHandleDictionary()
			'   DMAcadExt.AcadDocument.OpenLog(True)


			'	DMCommon.Debug.MsgBox("13_401", bApproved, bProposed, ApprMapThemeData.CentroidBlocks, bMitham, bMithamProx, "bPlan=", bPlan, PlanMapThemeData.CentroidBlock, PlanMapThemeData.CentroidLayers, PlanMapThemeData.LinkLayers)
			If bPlan Then
				TopoManager.TPlanGraph.TplnPlan.Initialize(PlanMapThemeData)
			End If
			DMCommon.Debug.MsgBox("!Lot.Init", ApprMapThemeData.TopoName, ApprMapThemeData.GraphType, bMithamProx, bMitham)
			If bApproved OrElse bProposed Then
				'  MessageBox.Show(CStr(bApproved) & ":" & CStr(bProposed) & ":" & CStr(bParcel) & vbCrLf & CStr(bExpro) & ":" & CStr(bMerhav) & vbCrLf & CStr(bMerge), "06_397 bef")
				If bApproved Then
					TopoManager.TPlanGraph.TplnLot.Initialize(ApprMapThemeData)
				End If
				If bProposed Then
					TopoManager.TPlanGraph.TplnLot.Initialize(PropMapThemeData)
				End If
				''''''''''''''''''''''''''''260226 TopoManager.TPlanGraph.TplnLot.Initialize(ApprMapThemeData, PropMapThemeData)
				TopoManager.TPlanGraph.TplnLot.Initialize(ApprMapThemeData, PropMapThemeData)
				' DMAcadExt.AcadDocument.WriteDebugMessage("#10x " & MithamMapThemeData.CentroidBlock)
				' MessageBox.Show("", "06_398 af")

				If bMitham Then
					TopoManager.TPlanGraph.TplnRegion.Initialize(MithamMapThemeData)
				End If
				If bMithamProx Then
					TopoManager.TPlanGraph.TplnRegion.InitializeProx(MithamProxMapThemeData)
				End If
				DMCommon.Debug.MsgBox("13_244a", "A_InitRegionDic()")
				InitRegionDic()
			End If
			If bMitham Or bMithamProx Then

				LoadRegionTopology()
				'18/11/19 LoadRegions()
			End If
			'	TopoManager.TPlanGraph.TplnLot.NameIsNum = Me.chkLotNameNum.Checked
			'   MessageBox.Show(bMerhav.ToString(), "09_549")
			If bMerhav Then
				TopoManager.TPlanGraph.TplnMerhav.Initialize(MerhavMapThemeData)
			End If

			If bExpro Then
				TopoManager.TPlanGraph.TplnExpro.Initialize(ExproMapThemeData)

				'   TPlanGraph.TplnProject.CheckLegalArea()
			End If  'If bParcel

			If bZone Then
				TopoManager.TPlanGraph.TplnZone.Initialize(ZoneMapThemeData)

				'   TPlanGraph.TplnProject.CheckLegalArea()
			End If
			If bOwnershipNote Then
				TopoManager.TPlanGraph.TplnOwnershipNote.Initialize(OwnershipNoteMapThemeData)
			End If
			If bPlan Then
				bPlan = LoadPlans()

			End If
			DMCommon.Debug.MsgBox("13_120d", ApprMapThemeData.GraphType)

			If bApproved Then
				TplnLot.Reset(enTopoPurpose.Approved)
				If ApprMapThemeData.GraphType = DMAcadExt.enGraphType.Topology Then
					bApproved = LoadLots(DMAcadExt.enTopoPurpose.Approved)
				Else
					bApproved = LoadLots_CP(DMAcadExt.enTopoPurpose.Approved)
				End If
			Else
				TplnLot.Dispose(enTopoPurpose.Approved)
			End If







			If bProposed Then
				TplnLot.Reset(enTopoPurpose.Proposed)
				If PropMapThemeData.GraphType = DMAcadExt.enGraphType.Topology OrElse PropMapThemeData.GraphType = DMAcadExt.enGraphType.TopoOverlay Then
					bProposed = LoadLots(DMAcadExt.enTopoPurpose.Proposed)
				Else
					bProposed = LoadLots_CP(DMAcadExt.enTopoPurpose.Proposed)
				End If
			Else
				TplnLot.Dispose(enTopoPurpose.Proposed)
			End If

			''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			DMCommon.Debug.MsgBox(miParcelGeoMethod.ToString(), "02_980")

			If bApproved Then

				TplnLanduse.FillColorSchemesDic(DMAcadExt.enTopoPurpose.Approved, ApprMapThemeData.MapThemeID)


				'	TPlanGraph.TplnProject.UpdateLotTable(bMerge, bUnion, bFDO_Overlay, DMAcadExt.enTopoPurpose.Approved)
				'	LoadLusePgonsNew(tApprMapThemeData)
			End If
			DMCommon.Debug.MsgBox(miParcelGeoMethod.ToString(), "02_981")

			If bProposed Then
				TplnLanduse.FillColorSchemesDic(DMAcadExt.enTopoPurpose.Proposed, PropMapThemeData.MapThemeID)
				'	TPlanGraph.TplnProject.UpdateLotTable(bMerge, bUnion, bFDO_Overlay, DMAcadExt.enTopoPurpose.Proposed)
				'	LoadLusePgonsNew(tPropMapThemeData)
			End If



			If bExpro Then

				'	bExpro = LoadExpros()
			End If

			If bZone Then
				bExpro = LoadZones()
			End If
			'DMCommon.Debug.MsgBox("13_410e", bParcel, bApproved, bProposed)
			''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

			If bParcel Then
				TplnParcel.Initialize(ParcelMapThemeData)

				TplnParcel.Reset()

				InitBlockDic()

				'  DMAcadExt.AcadDocument.WriteDebugMessage("#17 ")
				TplnBlock.Initialize(BlockMapThemeData)
				'DMCommon.Debug.MsgBox("13_410e1", bParcel, bApproved, bProposed)
				LoadBlocks()

				'DMCommon.Debug.MsgBox("13_410e2", bParcel, bApproved, bProposed)
				If miParcelGeoMethod = enGeoMethod.Topologia Then
					bParcel = LoadParcels()   '  LoadParcelsCP()	 
					'DMCommon.Debug.MsgBox("13_410e3", bParcel, bApproved, bProposed)
					LoadBlockTopology(False)
					'DMCommon.Debug.MsgBox("13_410e4", bParcel, bApproved, bProposed)
				End If
				'DMCommon.Debug.MsgBox("13_410f1", bParcel, bApproved, bProposed)
				If miParcelGeoMethod = enGeoMethod.ClosedPolygons Then
					bParcel = LoadParcelsCP()

					LoadCentroids()
				End If
				'??????????????????????????
				'DMCommon.Debug.MsgBox("13_410f2", bParcel, bApproved, bProposed)
				TplnParcel.CalculateRegionBlocks(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)


				'   TPlanGraph.TplnProject.CheckLegalArea()
			End If  'If bParcel
			'	DMCommon.Debug.MsgBox("13_410k", bMerhav, bFragment)

			If bMerhav Then
				bMerhav = LoadMerhav()
			End If
			If bFragment Then
				bFragment = LoadFragments()
			End If
			If bUd_Parcel Then
				bUd_Parcel = LoadUd_Parcels()
			End If

			TopoManager.TPlanGraph.TplnPlan.Initialize(PlanMapThemeData)

			If bOwnershipNote Then
				bOwnershipNote = LoadOwnershipNotes()
				bOwnershipNote = LoadFDO_Overlay(DMAcadExt.enTopoPurpose.OwnershipNote)
			End If

			If bExpro Then
				bExpro = LoadExpros()

				bExpro = LoadFDO_Overlay(DMAcadExt.enTopoPurpose.Expro)
			End If


			If bMerge AndAlso bApproved AndAlso (mdicParcels IsNot Nothing) Then
				DMAcadExt.AcadDocument.WriteDebugMessage("bMerge And bApproved")
				bMerge = TPlanGraph.TplnProject.LoadMerge(DMAcadExt.enTopoPurpose.Approved)
			End If

			If bMerge AndAlso bProposed AndAlso (mdicParcels IsNot Nothing) Then
				DMAcadExt.AcadDocument.WriteDebugMessage("bMerge And bProposed")
				bMerge = TPlanGraph.TplnProject.LoadMerge(DMAcadExt.enTopoPurpose.Proposed)
			End If

			If bUnion AndAlso bApproved Then
				DMAcadExt.AcadDocument.WriteDebugMessage("bUnion And bApproved")
				TPlanGraph.TplnProject.LoadUnion(DMAcadExt.enTopoPurpose.Approved)
			End If

			If bUnion AndAlso bProposed Then
				DMAcadExt.AcadDocument.WriteDebugMessage("bUnion And bProposed")
				TPlanGraph.TplnProject.LoadUnion(DMAcadExt.enTopoPurpose.Proposed)
			End If
			'MessageBox.Show(CStr(bApproved) & ":" & CStr(bFDO_Overlay) & ":" & CStr(mdicParcels IsNot Nothing) & vbCrLf & miParcelGeoMethod.ToString(), "04_348f")
			'  DMAcadExt.AcadDocument.WriteDebugMessage("!!04_61:" & CStr(bFDO_Overlay) & ":" & bApproved.ToString() & ":" & miParcelGeoMethod.ToString())
			'DMCommon.Debug.MsgBox("13_132e", bFDO_Overlay, bApproved, DMCommon.Debug.ColCount(mdicParcels), miParcelGeoMethod)

			If bFDO_Overlay AndAlso bApproved AndAlso (mdicParcels IsNot Nothing) Then

				If miParcelGeoMethod = enGeoMethod.Topologia Then

					bFDO_Overlay = TPlanGraph.TplnProject.LoadFDO_Overlay(DMAcadExt.enTopoPurpose.Approved) 'TPlanGraph.TplnProject.LoadFDO_OverlayCP(DMAcadExt.enTopoPurpose.Approved)
					'   DMAcadExt.AcadDocument.WriteDebugMessage("!!04_62:" & CStr(bFDO_Overlay) & ":" & bApproved.ToString() & ":" & miParcelGeoMethod.ToString() & ":" & bFDO_Overlay.ToString())
				End If
				If miParcelGeoMethod = enGeoMethod.ClosedPolygons Then
					bFDO_Overlay = LoadFDO_OverlayCP_New(DMAcadExt.enTopoPurpose.Approved)    'TPlanGraph.TplnProject.LoadFDO_OverlayCP(DMAcadExt.enTopoPurpose.Approved)
				End If
			End If
			'	DMCommon.Debug.MsgBox("13_120f")
			If bFDO_Overlay AndAlso bProposed AndAlso (mdicParcels IsNot Nothing) Then

				If miParcelGeoMethod = enGeoMethod.Topologia Then
					bFDO_Overlay = TPlanGraph.TplnProject.LoadFDO_Overlay(DMAcadExt.enTopoPurpose.Proposed) 'TPlanGraph.TplnProject.LoadFDO_OverlayCP(DMAcadExt.enTopoPurpose.Approved)
				End If
				If miParcelGeoMethod = enGeoMethod.ClosedPolygons Then
					bFDO_Overlay = TPlanGraph.TplnProject.LoadFDO_OverlayCP_New(DMAcadExt.enTopoPurpose.Proposed)    'TPlanGraph.TplnProject.LoadFDO_OverlayCP(DMAcadExt.enTopoPurpose.Approved)
				End If
			End If
			'	DMCommon.Debug.MsgBox("13_120q")
			If ExproMapThemeData.IsNotEmpty Then

				''''''''''''''''''''''''''''''''''''''''''LoadFDO_ExproOverlay_New(DMAcadExt.enTopoPurpose.Expro)
			End If
			If MerhavMapThemeData.IsNotEmpty Then
				'   LoadFDO_MerhavOverlay(DMAcadExt.enTopoPurpose.Approved)
			End If

			If bExproZoneOverlay Then
				zzLoadFDO_ExproZoneOverlay()
			End If
			If bPlan Then
				If bApproved Then
					TopoManager.TPlanGraph.TplnLot.Initialize(ApprMapThemeData)
					PlanToLots(DMAcadExt.enTopoPurpose.Approved)
				End If
				If bProposed Then
					TopoManager.TPlanGraph.TplnLot.Initialize(PropMapThemeData)
					PlanToLots(DMAcadExt.enTopoPurpose.Proposed)
				End If

			End If


			If bExproLotOverlay Then
				If bPlan Then
					zzLoadFDO_ExproLotOverlayBasicPlan()
				Else
					zzLoadFDO_ExproLotOverlay()

				End If
			End If





			'Balance


			If bParcel Then
				'Balance
				'DMCommon.Debug.MsgBox("CalculateParcels_13a", mdicParcels.Count, bFDO_Overlay, bApproved, bProposed, bExpro)
				TPlanGraph.TplnProject.CalculateParcels(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed, bExpro)
				'DMCommon.Debug.MsgBox("CalculateParcels_13a", mdicParcels.Count, bFDO_Overlay, bApproved, bProposed, bExpro)
			End If

			If (bApproved OrElse bProposed) AndAlso mdicRegions.Count > 1 Then
				zzDistribute(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)

				For Each oRegion As TplnRegion In mdicRegions.Values
					oRegion.CalculateOverlayGroups(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
				Next

			Else

				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!CalculateOverlayGroups", "TplnProject.Calculate")
				CalculateOverlayGroups(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
			End If
			'OverlayGroupsToExcel(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)

			If bFDO_Overlay Then
				Dim iOverlayIndex As DMAcadExt.enOverlayIndex = DMAcadExt.enOverlayIndex.ApprFDO_Overlay
				Dim dicLots As TPlanGraph.TplnLots = Lots(DMAcadExt.enTopoPurpose.Approved)

				If bApproved Then
					iOverlayIndex = DMAcadExt.enOverlayIndex.ApprFDO_Overlay
					dicLots = Lots(DMAcadExt.enTopoPurpose.Approved)
				ElseIf bProposed Then
					iOverlayIndex = DMAcadExt.enOverlayIndex.PropFDO_Overlay
					dicLots = Lots(DMAcadExt.enTopoPurpose.Proposed)
				End If

				Dim oRegion As TplnRegion = Nothing
				'   zzCalculate2Area(iOverlayIndex)
				If dicLots IsNot Nothing Then
					For Each oLot As TplnLot In dicLots.Values
						oLot.Calculate2(bMerge, bUnion, bFDO_Overlay)

						If oLot.RegionNo <> 0 Then
							If mdicRegions.TryGetValue(oLot.RegionNo, oRegion) Then
								oRegion.AddAreaset(oLot.AreaSet(iOverlayIndex))
								'??  oRegion.AddLot(oLot)
							End If
						End If

					Next
				End If

			End If


			If mdicRegions IsNot Nothing Then

				Dim tAreaSet As TplnAreaSet
				'Calculate 2 Alt

				'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "Reg Count", mdicRegions.Count)
				For Each oRegion As TplnRegion In mdicRegions.Values

					tAreaSet = oRegion.AreaSet

					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!oRegion!", oRegion.TopoID, oRegion.RegionNo, oRegion.RegionName, tAreaSet.AcadArea, tAreaSet.CalcArea, tAreaSet.CalcArea2, tAreaSet.CalcGroupArea, tAreaSet.RoundedArea)
				Next
			End If

			If bParcel Then ''''''''''''gggggggggggg
				'**************************************************** _429
				TPlanGraph.TplnProject.UpdateParcelTable(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed, bExpro)      '''''''''''''''''''''''''''''   02/06/09
				'System.Windows.Forms.MessageBox.Show("", "PrjCalc_0400")

			End If



			If bApproved Then
				TPlanGraph.TplnProject.UpdateLotTable(bMerge, bUnion, bFDO_Overlay, DMAcadExt.enTopoPurpose.Approved)

				DMCommon.Debug.ExcelLog.SetDataTable(0, "!LotMain", TplnLot.MainDataTable(DMAcadExt.enTopoPurpose.Approved))
				LoadLusePgonsNew(ApprMapThemeData)
			End If

			If bProposed Then

				TPlanGraph.TplnProject.UpdateLotTable(bMerge, bUnion, bFDO_Overlay, DMAcadExt.enTopoPurpose.Proposed)
				LoadLusePgonsNew(PropMapThemeData)
			End If






			If mdicRegions IsNot Nothing Then
				TPlanGraph.TplnRegion.CreateRegionTable()

			End If

			Dim bOverlayExists As Boolean = False

			If bMerge Then
				TplnLot.CalcLanduses(Not bLanduseByPolygons, DMAcadExt.enOverlayMethod.Merge)
				bOverlayExists = True
			End If

			If bUnion Then
				TplnLot.CalcLanduses(Not bLanduseByPolygons, DMAcadExt.enOverlayMethod.Union)
				bOverlayExists = True
			End If

			If bFDO_Overlay Then
				TplnLot.CalcLanduses(Not bLanduseByPolygons, DMAcadExt.enOverlayMethod.FDO_Overlay)
				bOverlayExists = True
			End If
			If Not bOverlayExists Then
				TplnLot.CalcLanduses(bLanduseByPolygons, DMAcadExt.enOverlayMethod.Merge)
			End If

			If bParcel AndAlso bMerge Then  'FFFFFFFFFFFFFFFFFFFFFFFFFF

				If bApproved Then
					TPlanGraph.TplnParcel.CalculateLotsContent(DMAcadExt.enOverlayIndex.ApprMerge)
					UpdateOverlayTable(DMAcadExt.enOverlayIndex.ApprMerge)
					TPlanGraph.TplnParcel.CalculateBlocks(DMAcadExt.enOverlayIndex.ApprMerge)
				End If

				If bProposed Then
					TPlanGraph.TplnParcel.CalculateLotsContent(DMAcadExt.enOverlayIndex.PropMerge)
					UpdateOverlayTable(DMAcadExt.enOverlayIndex.PropMerge)
					TPlanGraph.TplnParcel.CalculateBlocks(DMAcadExt.enOverlayIndex.PropMerge)
				End If
			End If

			If bParcel AndAlso bFDO_Overlay Then
				If bApproved Then
					TPlanGraph.TplnParcel.CalculateLotsContent(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
					UpdateOverlayTable(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)

					TPlanGraph.TplnParcel.CalculateBlocks(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
				End If

				If bProposed Then
					TPlanGraph.TplnParcel.CalculateLotsContent(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
					UpdateOverlayTable(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
					TPlanGraph.TplnParcel.CalculateBlocks(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
				End If
			End If

			If bParcel AndAlso Not bMerge AndAlso Not bFDO_Overlay Then
				TPlanGraph.TplnParcel.CalculateBlocks(DMAcadExt.enOverlayIndex.PropMerge)
			End If

			DMAcadExt.AcadTransaction.CloseModelSpace()
			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.CloseMessage()
			DMAcadExt.AcadDocument.Unlock()
			DMAcadExt.AcadDocument.CloseLog()
			'''''''''''''''''''''''''''''''''''''zzOverlayToExcel(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)

		End Sub
		Public Shared Sub OverlayGroupsToExcel(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean)
			If bMerge OrElse bUnion OrElse bFDO_Overlay Then
				'    Dim taTest(mcolRegionForcedArea.Count - 1) As BalanceArea.ConstArea
				Dim baOverlayArray() As Boolean = UnionPgonArea.GetOverlayArray(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
				For iOverlayIndex As DMAcadExt.enOverlayIndex = 0 To DMAcadExt.enOverlayIndex.OverlayIndexUB
					If baOverlayArray(iOverlayIndex) Then
						If mdicOverlayGroups(iOverlayIndex) IsNot Nothing Then
							For Each oOverlayGroup As TplnOverlayGroup In mdicOverlayGroups(iOverlayIndex).Values
								If oOverlayGroup.IsInPlan Then
									'DMCommon.Debug.ExcelLog.SetNextValue(0, "!OverlayGr", oOverlayGroup.ParcelID, oOverlayGroup.LotID, oOverlayGroup.GroupID, oOverlayGroup.AcadArea, oOverlayGroup.CalcArea, oOverlayGroup.CalcArea2, oOverlayGroup.RoundedArea, oOverlayGroup.CalcGroupArea, oOverlayGroup.CalcGroupArea2)
								End If
							Next
						Else
							System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString(), "01_554d")
						End If
					End If
				Next
			End If
		End Sub

		Public Shared Sub TestLINQ(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean, iRegionNo As Integer)
			If bMerge OrElse bUnion OrElse bFDO_Overlay Then
				'    Dim taTest(mcolRegionForcedArea.Count - 1) As BalanceArea.ConstArea
				Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
				'	Dim baOverlayArray() As Boolean = UnionPgonArea.GetOverlayArray(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
				If mdicOverlayGroups(iOverlayIndex) IsNot Nothing Then
					Dim colOvGroups As IEnumerable(Of TplnOverlayGroup) = From oOverlayGroup As TplnOverlayGroup In mdicOverlayGroups(iOverlayIndex).Values
																							Where oOverlayGroup.GroupID = iRegionNo
																							Select oOverlayGroup
					Dim grouped As IEnumerable(Of IGrouping(Of Integer, TplnOverlayGroup)) = colOvGroups.GroupBy(Function(fb) fb.ParcelID)


					'	Dim o As System.Linq.GroupedEnumerable(Of Integer, TopoManager.TPlanGraph.TplnOverlayGroup)




				Else
					System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString(), "01_554d")
				End If
			End If
		End Sub
		Public Shared Sub OverlayGroupsToExcel(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean, iRegionNo As Integer)
			If bMerge OrElse bUnion OrElse bFDO_Overlay Then
				'    Dim taTest(mcolRegionForcedArea.Count - 1) As BalanceArea.ConstArea
				Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
				'	Dim baOverlayArray() As Boolean = UnionPgonArea.GetOverlayArray(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
				If mdicOverlayGroups(iOverlayIndex) IsNot Nothing Then
					Dim colQuery1 = From oOverlayGroup As TplnOverlayGroup In mdicOverlayGroups(iOverlayIndex).Values
										 Where oOverlayGroup.GroupID = iRegionNo
										 Select oOverlayGroup
										 Group By ParcelID = oOverlayGroup.ParcelID
								 Into Parcels = Group
					'	Dim o As System.Linq.GroupedEnumerable(Of Integer, TopoManager.TPlanGraph.TplnOverlayGroup)
					'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!Type1", colQuery1.GetType())
					'As IEnumerable(Of TplnOverlayGroup)
					Dim oParcels As IEnumerable(Of TplnOverlayGroup)
					Dim iCount As Integer = colQuery1.Count

					For Each oV In colQuery1
						oParcels = oV.Parcels
						For Each oOverlayGroup As TplnOverlayGroup In oParcels
							'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!OverlayGr=" & iRegionNo.ToString(), oV.ParcelID, oOverlayGroup.ParcelID, oOverlayGroup.LotID, oOverlayGroup.GroupID, oParcels.Count, oOverlayGroup.AcadArea, oOverlayGroup.CalcArea, oOverlayGroup.CalcArea2, oOverlayGroup.RoundedArea, oOverlayGroup.CalcGroupArea, oOverlayGroup.CalcGroupArea2)
						Next
					Next

				Else
					System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString(), "01_554d")
				End If
			End If
		End Sub

		'Public Shared Sub OverlayGroupsToExcel(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean, iRegionNo As Integer)
		Public Shared Sub GetInPlanData(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal bEntirety As Boolean, ByVal iDataOption As TPlanGraph.enDataOptions, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, iRegionNo As Integer, ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
			Dim oDataTable As Data.DataTable = zzCreateInPlanTable()
			'	Dim o As SystemLinq.Linq.GroupedEnumerable(Of Int32, Integer)
			'	DMCommon.Debug.MsgBox("!!InPlanData", bEntirety, iRegionNo, iOverlayIndex, mdicOverlayGroups(iOverlayIndex) IsNot Nothing, mdicParcels IsNot Nothing)
			If mdicOverlayGroups(iOverlayIndex) IsNot Nothing AndAlso mdicParcels IsNot Nothing Then
				Dim colQuery1 = From oOverlayGroup As TplnOverlayGroup In mdicOverlayGroups(iOverlayIndex).Values
									 Where oOverlayGroup.SelectGroup(bEntirety, iRegionNo) = True
									 Select oOverlayGroup
									 Group By ParcelID = oOverlayGroup.ParcelID
									 Into Parcels = Group

				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!+Type", bEntirety, colQuery1.Count, colQuery1.GetType())

				Dim oParcels As IEnumerable(Of TplnOverlayGroup)
				Dim iCount As Integer = colQuery1.Count
				Dim iParcelID As Integer
				Dim oParcel As TplnParcel = Nothing
				Dim oRow As DataRow
				'Dim dOverlayGroupArea As Double
				Dim dParcelInArea As Double
				Dim dSumInArea As Double = 0.0
				Dim dSumLegalArea As Double = 0.0


				For Each oV In colQuery1
					iParcelID = oV.ParcelID
					oParcels = oV.Parcels
					If mdicParcels.TryGetValue(iParcelID, oParcel) Then
						oRow = oDataTable.NewRow
						oRow.Item(TplnParcel.BlockFullFieldName) = oParcel.BlockFull
						oRow.Item(TplnParcel.NameFieldName) = oParcel.Name
						oRow.Item(TplnParcel.LegalAreaFieldName) = oParcel.LegalArea(False)
						oRow.Item(TplnParcel.BlockStatusNameFieldName) = oParcel.PlanStateText(iOverlayIndex)
						dParcelInArea = 0.0
						For Each oOverlayGroup As TplnOverlayGroup In oParcels
							dParcelInArea += oOverlayGroup.GetOptionArea(iDataOption)
							'DMCommon.Debug.ExcelLog.SetNextValue(0, "!!OverlayGr=" & iRegionNo.ToString(), oV.ParcelID, oOverlayGroup.ParcelID, oOverlayGroup.LotID, oOverlayGroup.GroupID, oParcels.Count, oOverlayGroup.AcadArea, oOverlayGroup.CalcArea, oOverlayGroup.CalcArea2, oOverlayGroup.RoundedArea, oOverlayGroup.CalcGroupArea, oOverlayGroup.CalcGroupArea2)

						Next
						oRow.Item(TopoReader.msAreaFldName) = dParcelInArea
						oDataTable.Rows.Add(oRow)
						dSumLegalArea += oParcel.LegalArea(False)
						dSumInArea += dParcelInArea
					End If
				Next
				'130126 DMCommon.Debug.ExcelLog.SetDataTable(0, "RepTab #2", oDataTable)

				Dim sOrderBy As String = TplnParcel.BlockFieldName & "," & TplnParcel.BlockAddFieldName & "," & TplnParcel.ParcelOrderFieldName
				oDataView = New DataView(oDataTable, String.Empty, sOrderBy, Data.DataViewRowState.CurrentRows)

				'130126 DMCommon.Debug.ExcelLog.SetDataTable(0, "RepView #2", oDataView)

				Dim iaAcadColumns() As Integer = {0, 1, 2, 3, 4}
				iaColumns = iaAcadColumns
				ReDim oaTotals(1)

				oaTotals(0) = dSumInArea
				oaTotals(1) = dSumLegalArea
			Else
				System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString(), "01_554d")
			End If

		End Sub
		Public Shared Sub GetInPlanParcelByGush(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal bEntirety As Boolean, ByVal iDataOption As TPlanGraph.enDataOptions, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod _
															 , iRegionNo As Integer, hsRegions As HashSet(Of Integer), ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object)
			Const iMaxLength As Integer = 16
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
			Dim oDataTable As Data.DataTable = zzCreateBlockContentTable()

			'DMCommon.Debug.MsgBox("!!InPlanParcelByGush", bEntirety, iRegionNo, iOverlayIndex, mdicOverlayGroups(iOverlayIndex) IsNot Nothing, mdicParcels IsNot Nothing)
			If mdicOverlayGroups(iOverlayIndex) IsNot Nothing AndAlso mdicParcels IsNot Nothing Then
				Dim colOvGroups As IEnumerable(Of TplnOverlayGroup) = From oOverlayGroup As TplnOverlayGroup In mdicOverlayGroups(iOverlayIndex).Values
																						Where oOverlayGroup.SelectGroup(bEntirety, iRegionNo, hsRegions) = True
																						Select oOverlayGroup
				Dim oGroupedByBlock As IEnumerable(Of IGrouping(Of Integer, TplnOverlayGroup)) = colOvGroups.GroupBy(Function(fb) fb.BlockKey)
				Dim iCount As Integer = oGroupedByBlock.Count
				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!-!Type", bEntirety, colOvGroups.Count, colOvGroups.Count, colOvGroups.GetType())

				'	Dim oParcels As IEnumerable(Of TplnOverlayGroup)
				Dim sBlockStatus As String
				Dim iBlockKey As Integer
				Dim iParcelID As Integer

				'Dim iParcelNo As Integer

				Dim oParcel As TplnParcel = Nothing
				Dim oRow As DataRow
				'Dim dOverlayGroupArea As Double
				'	Dim dParcelInArea As Double
				Dim oBlock As TplnBlock = Nothing
				Dim dSumInArea As Double = 0.0
				Dim dSumLegalArea As Double = 0.0
				Dim iState As NumerationPair.enComplexType
				'	Dim iKey As Integer
				Dim oNumerationPair As NumerationPair = Nothing

				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!+!GroupedByBlock", oGroupedByBlock.Count)
				For Each oGrouping As IGrouping(Of Integer, TplnOverlayGroup) In oGroupedByBlock
					iBlockKey = oGrouping.Key
					If mdicBlocks.TryGetValue(iBlockKey, oBlock) Then
						sBlockStatus = oBlock.BlockStatusName
					Else
						sBlockStatus = String.Empty
					End If
					oNumerationPair = New NumerationPair(NumerationPair.enTextDirection.RightToLeft)
					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!+!iKey", oGrouping.Count)
					Dim oGroupedByParcel As IEnumerable(Of IGrouping(Of Integer, TplnOverlayGroup)) = oGrouping.GroupBy(Function(fb) fb.ParcelID)
					For Each oGroupingByParcel As IGrouping(Of Integer, TplnOverlayGroup) In oGroupedByParcel
						iParcelID = oGroupingByParcel.Key
						'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!+!oGroupingByParcel", oGroupingByParcel.Count)
						If mdicParcels.TryGetValue(iParcelID, oParcel) Then
							If bEntirety Then
								iState = oParcel.PlanState(iOverlayIndex)
							Else
								iState = oParcel.RegionRelation(iOverlayIndex, iRegionNo, hsRegions)
							End If
							oNumerationPair.AddComplexNum(oParcel.Name, iState)

						End If
					Next
					If oParcel IsNot Nothing Then
						oRow = oDataTable.NewRow
						oRow.Item(TplnParcel.BlockFullFieldName) = oParcel.BlockFull
						oRow.Item(TplnParcel.BlockStatusNameFieldName) = sBlockStatus

						oRow.Item(TplnParcel.msParcelEntireFieldName) = oNumerationPair.GetPresentation(NumerationPair.enComplexType.Entire, iMaxLength)
						oRow.Item(TplnParcel.msParcelPartialFieldName) = oNumerationPair.GetPresentation(NumerationPair.enComplexType.Partial, iMaxLength)


						oRow.Item(TplnParcel.BlockFieldName) = oParcel.BlockNo                   '42
						oRow.Item(TplnParcel.BlockAddFieldName) = oParcel.BlockAdd                       '43

						oDataTable.Rows.Add(oRow)

					Else
						'130126	DMCommon.Debug.ExcelLog.SetNextValue(0, "!!iParcelID", iParcelID)
					End If

				Next
				If True Then
					Dim sOrderBy As String = TplnParcel.BlockFieldName & "," & TplnParcel.BlockAddFieldName
					oDataView = New DataView(oDataTable, String.Empty, sOrderBy, Data.DataViewRowState.CurrentRows)

					iaColumns = {0, 1, 2, 3}

				End If
			Else
				Windows.Forms.MessageBox.Show(iOverlayIndex.ToString(), "01_554d")
			End If


		End Sub
		Public Shared Sub GetInPlanDataNew(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal bEntirety As Boolean, ByVal bLastColumn As Boolean, ByVal iDataOption As TPlanGraph.enDataOptions, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod _
													  , iRegionNo As Integer, hsRegions As HashSet(Of Integer), bDunam As Boolean, ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
			Dim oDataTable As Data.DataTable = zzCreateInPlanTable()

			DMCommon.Debug.MsgBox("!!InPlanDataNew", bEntirety, iRegionNo, iOverlayIndex, mdicOverlayGroups(iOverlayIndex) IsNot Nothing, mdicParcels IsNot Nothing)
			If mdicOverlayGroups(iOverlayIndex) IsNot Nothing AndAlso mdicParcels IsNot Nothing Then
				Dim colOvGroups As IEnumerable(Of TplnOverlayGroup)
				colOvGroups = From oOverlayGroup As TplnOverlayGroup In mdicOverlayGroups(iOverlayIndex).Values
								  Where oOverlayGroup.SelectGroup(bEntirety, iRegionNo, hsRegions) = True
								  Select oOverlayGroup
				Dim grouped As IEnumerable(Of IGrouping(Of Integer, TplnOverlayGroup)) = colOvGroups.GroupBy(Function(fb) fb.ParcelID)
				Dim iCount As Integer = grouped.Count
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!+!Type", bEntirety, colOvGroups.Count, grouped.Count, colOvGroups.GetType())

				'	Dim oParcels As IEnumerable(Of TplnOverlayGroup)

				Dim iParcelID As Integer
				'Dim iParcelNo As Integer

				Dim oParcel As TplnParcel = Nothing
				Dim oRow As DataRow
				'Dim dOverlayGroupArea As Double
				Dim dParcelInArea As Double
				Dim dSumInArea As Double = 0.0
				Dim dSumLegalArea As Double = 0.0
				Dim iState As NumerationPair.enComplexType
				'	Dim iKey As Integer

				For Each oGrouping As IGrouping(Of Integer, TplnOverlayGroup) In grouped
					iParcelID = oGrouping.Key
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!+!iKey", oGrouping.Count)
					If mdicParcels.TryGetValue(iParcelID, oParcel) Then
						oRow = oDataTable.NewRow
						oRow.Item(TplnParcel.BlockFullFieldName) = oParcel.BlockFull
						oRow.Item(TplnParcel.NameFieldName) = oParcel.Name
						oRow.Item(TplnParcel.LegalAreaFieldName) = oParcel.LegalArea(bDunam)
						If bEntirety Then
							iState = oParcel.PlanState(iOverlayIndex)
						Else
							iState = oParcel.RegionRelation(iOverlayIndex, iRegionNo, hsRegions)
						End If
						oRow.Item(TplnParcel.msParcelPartialFieldName) = TplnParcel.GetPlanStateText(iState)

						dParcelInArea = 0.0
						For Each oOverlayGroup As TplnOverlayGroup In oGrouping
							dParcelInArea += oOverlayGroup.GetOptionArea(iDataOption)
							'DMCommon.Debug.ExcelLog.SetNextValue(0, "!!OverlayGr=" & iRegionNo.ToString(), oParcel.BlockNo, oParcel.Name, iParcelID, oOverlayGroup.ParcelID, oOverlayGroup.LotID, oOverlayGroup.GroupID, oOverlayGroup.AcadArea, oOverlayGroup.CalcArea, oOverlayGroup.CalcArea2, oOverlayGroup.RoundedArea, oOverlayGroup.CalcGroupArea, oOverlayGroup.CalcGroupArea2)
						Next
						If bDunam Then
							oRow.Item(TopoReader.msAreaFldName) = dParcelInArea * 0.001
						Else
							oRow.Item(TopoReader.msAreaFldName) = dParcelInArea
						End If


						oRow.Item(TplnParcel.ParcelOrderFieldName) = oParcel.ParcelNo
						oRow.Item(TplnParcel.BlockFieldName) = oParcel.BlockNo                   '42
						oRow.Item(TplnParcel.BlockAddFieldName) = oParcel.BlockAdd               '43


						oDataTable.Rows.Add(oRow)
						dSumLegalArea += oParcel.LegalArea(bDunam)
						dSumInArea += dParcelInArea
					End If


				Next
				If True Then
					Dim sOrderBy As String = TplnParcel.BlockFieldName & "," & TplnParcel.BlockAddFieldName & "," & TplnParcel.ParcelOrderFieldName
					oDataView = New DataView(oDataTable, String.Empty, sOrderBy, Data.DataViewRowState.CurrentRows)


					'	Dim iaAcadColumns() As Integer '= {0, 1, 2, 3, 4}
					'iaColumns = iaAcadColumns
					If bLastColumn Then
						iaColumns = {0, 1, 2, 3, 4}
					Else
						iaColumns = {0, 1, 2, 3}
					End If
					ReDim oaTotals(1)

					oaTotals(0) = dSumInArea
					oaTotals(1) = dSumLegalArea
				End If
			Else
				Windows.Forms.MessageBox.Show(iOverlayIndex.ToString(), "01_554d")
			End If

		End Sub

		Public Shared Sub GetInPlanDataII(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal bEntirety As Boolean, ByVal bLastColumn As Boolean, ByVal iDataOption As TPlanGraph.enDataOptions, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod _
													  , iRegionNo As Integer, hsRegions As HashSet(Of Integer), bDunam As Boolean, ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
			Dim oDataTable As Data.DataTable = zzCreateInPlanTable()

			DMCommon.Debug.MsgBox("!!InPlanDataNew", bEntirety, iRegionNo, iOverlayIndex, mdicOverlayGroups(iOverlayIndex) IsNot Nothing, mdicParcels IsNot Nothing)
			If mdicOverlayGroups(iOverlayIndex) IsNot Nothing AndAlso mdicParcels IsNot Nothing Then
				Dim colOvGroups As IEnumerable(Of TplnOverlayGroup)
				colOvGroups = From oOverlayGroup As TplnOverlayGroup In mdicOverlayGroups(iOverlayIndex).Values
								  Where oOverlayGroup.SelectGroup(bEntirety, iRegionNo, hsRegions) = True
								  Select oOverlayGroup
				Dim grouped As IEnumerable(Of IGrouping(Of Integer, TplnOverlayGroup)) = colOvGroups.GroupBy(Function(fb) fb.ParcelID)
				Dim iCount As Integer = grouped.Count
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!+!Type", bEntirety, colOvGroups.Count, grouped.Count, colOvGroups.GetType())

				'	Dim oParcels As IEnumerable(Of TplnOverlayGroup)

				Dim iParcelID As Integer
				'Dim iParcelNo As Integer

				Dim oParcel As TplnParcel = Nothing
				Dim oRow As DataRow
				'Dim dOverlayGroupArea As Double
				Dim dParcelInArea As Double
				Dim dSumInArea As Double = 0.0
				Dim dSumLegalArea As Double = 0.0
				Dim iState As NumerationPair.enComplexType
				'	Dim iKey As Integer

				For Each oGrouping As IGrouping(Of Integer, TplnOverlayGroup) In grouped
					iParcelID = oGrouping.Key
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!+!iKey", oGrouping.Count)
					If mdicParcels.TryGetValue(iParcelID, oParcel) Then
						oRow = oDataTable.NewRow
						oRow.Item(TplnParcel.BlockFullFieldName) = oParcel.BlockFull
						oRow.Item(TplnParcel.NameFieldName) = oParcel.Name
						oRow.Item(TplnParcel.LegalAreaFieldName) = oParcel.LegalArea(bDunam)
						If bEntirety Then
							iState = oParcel.PlanState(iOverlayIndex)
						Else
							iState = oParcel.RegionRelation(iOverlayIndex, iRegionNo, hsRegions)
						End If
						oRow.Item(TplnParcel.msParcelPartialFieldName) = TplnParcel.GetPlanStateText(iState)

						dParcelInArea = 0.0
						For Each oOverlayGroup As TplnOverlayGroup In oGrouping
							dParcelInArea += oOverlayGroup.GetOptionArea(iDataOption)
							'DMCommon.Debug.ExcelLog.SetNextValue(0, "!!OverlayGr=" & iRegionNo.ToString(), oParcel.BlockNo, oParcel.Name, iParcelID, oOverlayGroup.ParcelID, oOverlayGroup.LotID, oOverlayGroup.GroupID, oOverlayGroup.AcadArea, oOverlayGroup.CalcArea, oOverlayGroup.CalcArea2, oOverlayGroup.RoundedArea, oOverlayGroup.CalcGroupArea, oOverlayGroup.CalcGroupArea2)
						Next
						If bDunam Then
							oRow.Item(TopoReader.msAreaFldName) = dParcelInArea * 0.001
						Else
							oRow.Item(TopoReader.msAreaFldName) = dParcelInArea
						End If


						oRow.Item(TplnParcel.ParcelOrderFieldName) = oParcel.ParcelNo
						oRow.Item(TplnParcel.BlockFieldName) = oParcel.BlockNo                   '42
						oRow.Item(TplnParcel.BlockAddFieldName) = oParcel.BlockAdd               '43


						oDataTable.Rows.Add(oRow)
						dSumLegalArea += oParcel.LegalArea(bDunam)
						dSumInArea += dParcelInArea
					End If



				Next
				If True Then
					Dim sOrderBy As String = TplnParcel.BlockFieldName & "," & TplnParcel.BlockAddFieldName & "," & TplnParcel.ParcelOrderFieldName
					oDataView = New DataView(oDataTable, String.Empty, sOrderBy, Data.DataViewRowState.CurrentRows)


					'	Dim iaAcadColumns() As Integer '= {0, 1, 2, 3, 4}
					'iaColumns = iaAcadColumns
					If bLastColumn Then
						iaColumns = {0, 1, 2, 3, 4}
					Else
						iaColumns = {0, 1, 2, 3}
					End If
					ReDim oaTotals(1)

					oaTotals(0) = dSumInArea
					oaTotals(1) = dSumLegalArea
				End If
			Else
				Windows.Forms.MessageBox.Show(iOverlayIndex.ToString(), "01_554d")
			End If

		End Sub

		Public Shared Sub GetLanduseDataNewNew(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, ByVal iDataOption As enDataOptions, ByVal bEntirety As Boolean _
															, ByVal iRegionNo As Integer, hsRegions As HashSet(Of Integer), ByRef oDataView As System.Data.DataView, ByRef oaTotals() As System.Object)
			Dim oLanduseTable As System.Data.DataTable
			Dim sLanduseNameApprFieldName As String = TplnParcel.LanduseNameFieldName & "Appr"
			Dim sLanduseNamePropFieldName As String = TplnParcel.LanduseNameFieldName & "Prop"
			Dim sLanduseIDApprFieldName As String = TplnParcel.LanduseIDFieldName & "Appr"
			Dim sLanduseIDPropFieldName As String = TplnParcel.LanduseIDFieldName & "Prop"
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
			Dim dicLanduses As TopoManager.TPlanGraph.TplnLanduses = TopoManager.TPlanGraph.TplnLot.Landuses
			Dim iIndex As Integer = 0
			oLanduseTable = New Data.DataTable("Landuse")
			With oLanduseTable.Columns
				If iTopoPurpose = DMAcadExt.enTopoPurpose.Approved Then
					.Add(sLanduseNameApprFieldName, GetType(System.String))
					.Add(TplnLot.msAreaApprFieldName, GetType(System.Double))
					.Add(TplnLot.msAreaPctApprFieldName, GetType(System.Double))
					.Add(sLanduseIDApprFieldName, GetType(System.Int32))
				ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed Then
					.Add(sLanduseNamePropFieldName, GetType(System.String))
					.Add(TplnLot.msAreaPropFieldName, GetType(System.Double))
					.Add(TplnLot.msAreaPctPropFieldName, GetType(System.Double))
					.Add(sLanduseIDPropFieldName, GetType(System.Int32))
				End If
				.Add(TplnParcel.LanduseOrderFieldName, GetType(System.Int32))
			End With

			Dim colOvGroups As IEnumerable(Of TplnOverlayGroup) = From oOverlayGroup As TplnOverlayGroup In mdicOverlayGroups(iOverlayIndex).Values
																					Where oOverlayGroup.SelectGroup(bEntirety, iRegionNo, hsRegions) = True
																					Select oOverlayGroup


			Dim oGrouped As IEnumerable(Of IGrouping(Of Integer, TplnOverlayGroup)) = colOvGroups.GroupBy(Function(fb) fb.LanduseID)

			Dim iLanduseCount As Integer = oGrouped.Count


			Dim oNewRow As System.Data.DataRow
			'Dim oLanduse As TplnLanduse

			Dim daResAreaAppr(iLanduseCount - 1) As Double
			Dim daResAreaProp(iLanduseCount - 1) As Double


			Dim daPcntAppr(iLanduseCount - 1) As Double
			Dim daPcntProp(iLanduseCount - 1) As Double

			Dim oaLandusesAppr(iLanduseCount) As TplnLanduse
			Dim oaLandusesProp(iLanduseCount) As TplnLanduse

			Dim iIndexAppr As Integer = 0
			Dim iIndexProp As Integer = 0
			'Dim iIndexApprUB As Integer
			'Dim iIndexPropUB As Integer

			Dim dInPlanSumAreaAppr As Double
			Dim dInPlanSumAreaProp As Double
			Dim dLanduseArea As Double
			Dim iLanduseID As Integer
			If iDataOption = enDataOptions.CalcMergeArea Then
				'dInPlanSumAreaAppr = mdInPlanSumCalcAreaAppr
				'dInPlanSumAreaProp = mdInPlanSumCalcAreaProp
			ElseIf iDataOption = enDataOptions.AcadArea Then
				dInPlanSumAreaAppr = Math.Round(dInPlanSumAreaAppr * 0.001, 3, MidpointRounding.AwayFromZero)
				dInPlanSumAreaProp = Math.Round(dInPlanSumAreaProp * 0.001, 3, MidpointRounding.AwayFromZero)
			End If
			dInPlanSumAreaAppr = 0.0
			dInPlanSumAreaProp = 0.0



			For Each oGrouping As IGrouping(Of Integer, TplnOverlayGroup) In oGrouped
				iLanduseID = oGrouping.Key
				dLanduseArea = 0.0

				For Each oOverlayGroup As TplnOverlayGroup In oGrouping
					dLanduseArea += oOverlayGroup.GetOptionArea(iDataOption)
				Next
				dInPlanSumAreaAppr += dLanduseArea
				daResAreaAppr(iIndex) = dLanduseArea

				iIndex += 1
				oNewRow = oLanduseTable.NewRow()
				oNewRow.Item(sLanduseNameApprFieldName) = TplnLot.GetLanduseNameNew(DMAcadExt.enTopoPurpose.Approved, iLanduseID)
				oNewRow.Item(TplnLot.msAreaApprFieldName) = dLanduseArea
				oNewRow.Item(TplnLot.msAreaPctApprFieldName) = 0.0
				oNewRow.Item(sLanduseIDApprFieldName) = iLanduseID
				oLanduseTable.Rows.Add(oNewRow)
			Next

			'   System.Windows.Forms.MessageBox.Show(CStr(iIndexApprUB) & ":" & iIndexPropUB & vbCrLf & daResAreaProp.GetUpperBound(0), "07_340")
			'	ReDim Preserve daResAreaAppr(iIndexApprUB)
			'	ReDim Preserve daResAreaProp(iIndexPropUB)


			iIndexAppr = 0
			iIndexProp = 0

			''''''''''''''''''''''
			''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			Dim oPcntAppr As BalanceArea = Nothing
			Dim oPcntProp As BalanceArea = Nothing

			If dInPlanSumAreaAppr <> 0 Then
				'	iaPcntAppr = Balance(daPcntAppr)


				DMCommon.Functions.DispArray("daResAreaAppr", daResAreaAppr)
				oPcntAppr = New BalanceArea(daResAreaAppr, 100.0, 100.0, True, "Bal_LanduseData_1")

				DMCommon.Debug.MsgBox("oPcntAppr.OutputFloat", oPcntAppr.OutputFloat)
				For iIndex = 0 To oLanduseTable.Rows.Count - 1
					oNewRow = oLanduseTable.Rows.Item(iIndex)
					oNewRow.Item(TplnLot.msAreaPctApprFieldName) = oPcntAppr.OutputItemFloat(iIndex)
				Next

			End If
			'''''''''''''''''''
			'130126 DMCommon.Debug.ExcelLog.SetDataTable(0, "oJointLanduseTable", oLanduseTable)

			Dim sSort As String = TplnParcel.LanduseOrderFieldName 'String.Empty '


			oDataView = New System.Data.DataView(oLanduseTable, String.Empty, sSort, DataViewRowState.CurrentRows)
			oDataView.AllowEdit = False
			oDataView.AllowDelete = False
			oDataView.AllowNew = False
			ReDim oaTotals(1)
			If iTopoPurpose = DMAcadExt.enTopoPurpose.Approved AndAlso dInPlanSumAreaAppr <> 0.0 Then
				oaTotals(0) = 100.0
				oaTotals(1) = dInPlanSumAreaAppr
			ElseIf iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed AndAlso dInPlanSumAreaProp <> 0.0 Then
				oaTotals(0) = 100.0
				oaTotals(1) = dInPlanSumAreaProp
			End If
			'

		End Sub

		Public Shared Sub GetInPlanDataByLot(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal bEntirety As Boolean, ByVal iDataOption As TPlanGraph.enDataOptions, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, iRegionNo As Integer, hsRegion As HashSet(Of Integer), ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
			Dim oDataTable As Data.DataTable = zzCreateLotTable()

			DMCommon.Debug.MsgBox("!!InPlanDataByLot", bEntirety, iRegionNo, iOverlayIndex, mdicOverlayGroups(iOverlayIndex) IsNot Nothing, mdicParcels IsNot Nothing)
			If mdicOverlayGroups(iOverlayIndex) IsNot Nothing AndAlso mdicParcels IsNot Nothing Then
				Dim colOvGroups As IEnumerable(Of TplnOverlayGroup) = From oOverlayGroup As TplnOverlayGroup In mdicOverlayGroups(iOverlayIndex).Values
																						Where oOverlayGroup.SelectGroup(bEntirety, iRegionNo, hsRegion) = True
																						Select oOverlayGroup
				Dim grouped As IEnumerable(Of IGrouping(Of Integer, TplnOverlayGroup)) = colOvGroups.GroupBy(Function(fb) fb.LotID)
				Dim iCount As Integer = grouped.Count
				'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!+!Type", bEntirety, colOvGroups.Count, grouped.Count, colOvGroups.GetType())

				'	Dim oParcels As IEnumerable(Of TplnOverlayGroup)

				Dim iLotID As Integer
				Dim oLot As TplnLot = Nothing
				Dim oRow As DataRow
				'Dim dOverlayGroupArea As Double
				Dim dLotArea As Double
				Dim dSumLotArea As Double = 0.0
				Dim dSumLegalArea As Double = 0.0
				Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
				For Each oGrouping As IGrouping(Of Integer, TplnOverlayGroup) In grouped
					iLotID = oGrouping.Key
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!+!iKey", iKey, oGrouping.Count)
					If dicLots.TryGetValue(iLotID, oLot) Then
						oRow = oDataTable.NewRow
						oRow.Item(TplnLot.NameFieldName) = oLot.Name
						oRow.Item(TplnParcel.LanduseNameFieldName) = oLot.LanduseName            'TplnLot.GetLanduseNameNew(iTopoPurpose,)
						oRow.Item(TplnParcel.LanduseOrderFieldName) = oLot.LanduseOrder
						oRow.Item(TplnLot.LotOrderFieldName) = oLot.Order

						dLotArea = 0.0
						For Each oOverlayGroup As TplnOverlayGroup In oGrouping
							dLotArea += oOverlayGroup.GetOptionArea(iDataOption)
							'DMCommon.Debug.ExcelLog.SetNextValue(0, "!!OverlayGr=" & iRegionNo.ToString(), oOverlayGroup.ParcelID, oOverlayGroup.LotID, oOverlayGroup.GroupID, oOverlayGroup.AcadArea, oOverlayGroup.CalcArea, oOverlayGroup.CalcArea2, oOverlayGroup.RoundedArea, oOverlayGroup.CalcGroupArea, oOverlayGroup.CalcGroupArea2)
						Next
						oRow.Item(TopoReader.msAreaFldName) = dLotArea
						oDataTable.Rows.Add(oRow)
						dSumLotArea += dLotArea
					End If

				Next

				Dim sOrderBy As String = TplnParcel.LanduseOrderFieldName & "," & TplnLot.LotOrderFieldName
				oDataView = New DataView(oDataTable, String.Empty, sOrderBy, Data.DataViewRowState.CurrentRows)

				'130126 DMCommon.Debug.ExcelLog.SetDataTable(0, "!LuseOrder", oDataView)
				Dim iaAcadColumns() As Integer = {0, 1, 2}
				iaColumns = iaAcadColumns
				ReDim oaTotals(0)

				oaTotals(0) = dSumLotArea

			Else
				Windows.Forms.MessageBox.Show(iOverlayIndex.ToString(), "01_554d")
			End If

		End Sub
		Private Structure LotBlockKey
			Public LotID As Integer
			Public BlockNo As Integer
			Public BlockAddNo As Integer

			Public Sub New(oOverlayGroup As TplnOverlayGroup)
				LotID = oOverlayGroup.LotID
				BlockNo = oOverlayGroup.BlockNo
				BlockAddNo = oOverlayGroup.BlockAddNo


			End Sub
		End Structure
		Public Shared Sub GetInPlanDataByLotParcel(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal bEntirety As Boolean, ByVal iDataOption As TPlanGraph.enDataOptions, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod _
																 , iRegionNo As Integer, hsRegions As HashSet(Of Integer), ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object)
			Const iMaxLength As Integer = 20
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)

			Dim oDataTable As Data.DataTable = zzCreateLotsContentTable()

			DMCommon.Debug.MsgBox("!!InPlanDataByLotParcel", bEntirety, iRegionNo, iOverlayIndex, mdicOverlayGroups(iOverlayIndex) IsNot Nothing, mdicParcels IsNot Nothing)
			If mdicOverlayGroups(iOverlayIndex) IsNot Nothing AndAlso mdicParcels IsNot Nothing Then
				Dim colOvGroups As IEnumerable(Of TplnOverlayGroup) = From oOverlayGroup As TplnOverlayGroup In mdicOverlayGroups(iOverlayIndex).Values
																						Where oOverlayGroup.SelectGroup(bEntirety, iRegionNo, hsRegions) = True
																						Select oOverlayGroup
				Dim grouped As IEnumerable(Of IGrouping(Of LotBlockKey, TplnOverlayGroup)) = colOvGroups.GroupBy(Function(fb) New LotBlockKey(fb)) 'fb.LotID And fb.BlockNo And fb.BlockAddNo
				Dim iCount As Integer = grouped.Count
				'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!+!Type", bEntirety, colOvGroups.Count, grouped.Count, colOvGroups.GetType())

				'	Dim oParcels As IEnumerable(Of TplnOverlayGroup)

				'Dim iLotID As Integer
				Dim oLot As TplnLot = Nothing
				Dim oRow As DataRow
				'Dim dOverlayGroupArea As Double
				Dim dLotArea As Double
				Dim dSumLotArea As Double = 0.0
				Dim dSumLegalArea As Double = 0.0

				Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
				Dim oParcel As TplnParcel = Nothing
				Dim oNumerationPair As TopoManager.NumerationPair = Nothing
				Dim iComplexType As NumerationPair.enComplexType
				Dim tLotBlockKey As LotBlockKey
				iaColumns = {0, 1, 2, 3, 4}
				For Each oGrouping As IGrouping(Of LotBlockKey, TplnOverlayGroup) In grouped
					tLotBlockKey = oGrouping.Key
					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "1!+!iKey", iKey, oGrouping.Count)
					If dicLots.TryGetValue(tLotBlockKey.LotID, oLot) Then
						oRow = oDataTable.NewRow
						oRow.Item(TplnLot.NameFieldName) = oLot.Name
						oRow.Item(TopoReader.msAreaFldName) = oLot.AreaSet(DMAcadExt.enOverlayIndex.ApprFDO_Overlay).AcadArea
						'oRow.Item(TplnParcel.LanduseNameFieldName) = oLot.LanduseName
						dLotArea = 0.0
						oNumerationPair = New NumerationPair(NumerationPair.enTextDirection.RightToLeft)

						'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!!OverlayCapt", "ParcelID", "LotID", "GroupID", "AcadArea", "CalcArea", "CalcArea2", "RoundedArea", "CalcGroupArea", "CalcGroupArea2")
						For Each oOverlayGroup As TplnOverlayGroup In oGrouping
							If mdicParcels.TryGetValue(oOverlayGroup.ParcelID, oParcel) Then
								iComplexType = oParcel.LotRelation(iOverlayIndex, oOverlayGroup.LotID)
								oNumerationPair.AddComplexNum(oParcel.Name, iComplexType)



								'DMCommon.Debug.ExcelLog.SetNextValue(0, "!!OverlayGr=" & iRegionNo.ToString(), oOverlayGroup.ParcelID, oOverlayGroup.LotID, oOverlayGroup.GroupID, oOverlayGroup.AcadArea, oOverlayGroup.CalcArea, oOverlayGroup.CalcArea2, oOverlayGroup.RoundedArea, oOverlayGroup.CalcGroupArea, oOverlayGroup.CalcGroupArea2)

							End If
							dLotArea += oOverlayGroup.GetOptionArea(iDataOption)
						Next
						oRow.Item(TopoReader.msAreaFldName) = dLotArea
						oRow.Item(TopoReader.msAreaFldName) = oLot.AreaSet(iOverlayIndex).GetArea(iDataOption)


						oRow.Item(TplnParcel.msParcelEntireFieldName) = oNumerationPair.GetPresentation(NumerationPair.enComplexType.Entire, iMaxLength)
						oRow.Item(TplnParcel.msParcelPartialFieldName) = oNumerationPair.GetPresentation(NumerationPair.enComplexType.Partial, iMaxLength)

						oRow.Item(TplnParcel.BlockFullFieldName) = TplnBlock.GetBlockName(tLotBlockKey.BlockNo, tLotBlockKey.BlockAddNo)
						oRow.Item(TplnParcel.BlockFieldName) = tLotBlockKey.BlockNo
						oRow.Item(TplnParcel.BlockAddFieldName) = tLotBlockKey.BlockAddNo
						oRow.Item(TplnParcel.msLotOrderFieldName) = oLot.Order

						oDataTable.Rows.Add(oRow)
						dSumLotArea += dLotArea
					End If


				Next

				If True Then
					Dim sOrderBy As String = TplnParcel.msLotOrderFieldName & "," & TplnParcel.BlockFieldName & "," & TplnParcel.BlockAddFieldName
					oDataView = New DataView(oDataTable, String.Empty, sOrderBy, Data.DataViewRowState.CurrentRows)


					Dim iaAcadColumns() As Integer = {0, 1, 2}
					iaColumns = iaAcadColumns
					ReDim oaTotals(0)

					oaTotals(0) = dSumLotArea
					DMAcadExt.AcadDocument.WriteMessage("Total Area(rep #10):" & dSumLotArea.ToString())
				End If
			Else
				Windows.Forms.MessageBox.Show(iOverlayIndex.ToString(), "01_554d")
			End If

		End Sub
		Public Shared Sub GetLotData(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal bEntirety As Boolean, ByVal iDataOption As enDataOptions, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod _
											  , iRegionNo As Integer, hsRegions As HashSet(Of Integer), ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			Dim oDataTable As System.Data.DataTable = zzCreateRepLotTable()

			If mdicOverlayGroups(iOverlayIndex) IsNot Nothing AndAlso mdicParcels IsNot Nothing Then
				Dim colOvGroups As IEnumerable(Of TplnOverlayGroup) = From oOverlayGroup As TplnOverlayGroup In mdicOverlayGroups(iOverlayIndex).Values
																						Where oOverlayGroup.SelectGroup(bEntirety, iRegionNo, hsRegions) = True
																						Select oOverlayGroup
				Dim oGroupedByParcel As IEnumerable(Of IGrouping(Of Integer, TplnOverlayGroup)) = colOvGroups.GroupBy(Function(fb) fb.ParcelID)
				Dim iCount As Integer = oGroupedByParcel.Count
				'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!+!Type", bEntirety, colOvGroups.Count, oGroupedByParcel.Count, colOvGroups.GetType())

				'	Dim oParcels As IEnumerable(Of TplnOverlayGroup)

				Dim iParcelID As Integer

				Dim oParcel As TplnParcel = Nothing
				Dim oLot As TplnLot = Nothing
				Dim iLotID As Integer

				Dim oRow As DataRow
				'Dim dOverlayGroupArea As Double
				Dim dParcelInArea As Double
				Dim dLotArea As Double

				Dim dSumInArea As Double = 0.0
				Dim dSumLotArea As Double = 0.0
				Dim dSumLegalArea As Double = 0.0

				Dim iParcelRowIndex As Integer
				For Each oGrouping As IGrouping(Of Integer, TplnOverlayGroup) In oGroupedByParcel
					iParcelID = oGrouping.Key
					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!+!iKey", iKey, oGrouping.Count)
					If mdicParcels.TryGetValue(iParcelID, oParcel) Then

						'	oRow.Item(TplnParcel.BlockStatusNameFieldName) = oParcel.PlanStateText(iOverlayIndex)
						dParcelInArea = 0.0
						iParcelRowIndex = oDataTable.Rows.Count
						Dim oGroupedByLot As IEnumerable(Of IGrouping(Of Integer, TplnOverlayGroup)) = oGrouping.GroupBy(Function(fb) fb.LotID)
						For Each oGrouping1 As IGrouping(Of Integer, TplnOverlayGroup) In oGroupedByLot
							iLotID = oGrouping1.Key
							dLotArea = 0.0
							If dicLots.TryGetValue(iLotID, oLot) Then
								oRow = oDataTable.NewRow
								oRow.Item(TplnParcel.BlockFullFieldName) = oParcel.BlockFull
								oRow.Item(TplnParcel.NameFieldName) = oParcel.Name
								oRow.Item(TplnParcel.LegalAreaFieldName) = oParcel.LegalArea(False)
								For Each oOverlayGroup As TplnOverlayGroup In oGrouping1
									'dParcelInArea += oOverlayGroup.GetOptionArea(iDataOption)
									dLotArea += oOverlayGroup.GetOptionArea(iDataOption)

									'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!!OverlayGr=" & iRegionNo.ToString(), iParcelID, oOverlayGroup.ParcelID, oOverlayGroup.LotID, oOverlayGroup.GroupID, oOverlayGroup.AcadArea, oOverlayGroup.CalcArea, oOverlayGroup.CalcArea2, oOverlayGroup.RoundedArea, oOverlayGroup.CalcGroupArea, oOverlayGroup.CalcGroupArea2)
								Next
								oRow.Item(TplnLot.msAreaApprFieldName) = dLotArea
								dParcelInArea += dLotArea
								oRow.Item(TplnLot.NameFieldName) = oLot.Name
								oRow.Item(TplnParcel.LanduseNameFieldName) = oLot.LanduseName 'TplnLot.GetLanduseNameNew(iTopoPurpose, oLot.LanduseID)

								'DMCommon.Debug.ExcelLog.SetNextValue(0, "!!MainLot", oLot.LanduseID, TplnLot.GetLanduseNameNew(iTopoPurpose, oLot.LanduseID), oLot.LanduseName)
								'oRow.Item(TopoReader.msAreaFldName) = dParcelInArea


								oRow.Item(TplnParcel.BlockFieldName) = oParcel.BlockNo                   '42
								oRow.Item(TplnParcel.BlockAddFieldName) = oParcel.BlockAdd
								oRow.Item(TplnParcel.ParcelOrderFieldName) = oParcel.ParcelNo
								oRow.Item(TplnParcel.msLotOrderFieldName) = oLot.Order


								oDataTable.Rows.Add(oRow)

								dSumLotArea += dLotArea
							End If
						Next
						For iIndex = iParcelRowIndex To oDataTable.Rows.Count - 1
							oRow = oDataTable.Rows.Item(iIndex)
							oRow.Item(TopoReader.msAreaFldName) = dParcelInArea
						Next

						dSumLegalArea += oParcel.LegalArea(False)
						dSumInArea += dParcelInArea
					End If
				Next
				Dim sOrderBy As String = TplnParcel.BlockFieldName & "," & TplnParcel.BlockAddFieldName & "," & TplnParcel.ParcelOrderFieldName
				oDataView = New DataView(oDataTable, String.Empty, sOrderBy, Data.DataViewRowState.CurrentRows)
				'	DMCommon.Debug.ExcelLog.SetDataTable(0, "!Rep 5", oDataView)

				Dim iaAcadColumns() As Integer = {0, 1, 2, 3, 4, 5, 6}
				iaColumns = iaAcadColumns
				ReDim oaTotals(2)

				oaTotals(0) = dSumLotArea
				oaTotals(1) = dSumInArea

				oaTotals(2) = dSumLegalArea
			End If


		End Sub

		Public Shared Sub GetLanduseDataNew(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal bEntirety As Boolean, ByVal iDataOption As enDataOptions, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, iRegionNo As Integer, hsRegions As HashSet(Of Integer), ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex = UnionPgonArea.GetOverlayIndex(iOverlayMethod, iTopoPurpose)
			Dim oDataTable As System.Data.DataTable = zzCreateLanduseTable()

			If mdicOverlayGroups(iOverlayIndex) IsNot Nothing AndAlso mdicParcels IsNot Nothing Then
				Dim colOvGroups As IEnumerable(Of TplnOverlayGroup) = From oOverlayGroup As TplnOverlayGroup In mdicOverlayGroups(iOverlayIndex).Values
																						Where oOverlayGroup.SelectGroup(bEntirety, iRegionNo, hsRegions) = True
																						Select oOverlayGroup

				Dim oGroupedByParcel As IEnumerable(Of IGrouping(Of Integer, TplnOverlayGroup)) = colOvGroups.GroupBy(Function(fb) fb.ParcelID)
				Dim iCount As Integer = oGroupedByParcel.Count
				'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!+!Type", bEntirety, colOvGroups.Count, oGroupedByParcel.Count, colOvGroups.GetType())

				Dim iParcelID As Integer

				Dim oParcel As TplnParcel = Nothing
				Dim iLanduseID As Integer, iLanduseOrder As Integer
				Dim sLanduseName As String = Nothing
				Dim oRow As DataRow
				'Dim dOverlayGroupArea As Double
				Dim dParcelInArea As Double
				Dim dLanduseArea As Double

				Dim dSumInArea As Double = 0.0
				Dim dSumLanduseArea As Double = 0.0
				Dim dSumLegalArea As Double = 0.0
				'Dim iKey As Integer
				Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
				Dim iParcelRowIndex As Integer
				'Dim iLotID As Integer
				'Dim oLot As TplnLot = Nothing
				For Each oGrouping As IGrouping(Of Integer, TplnOverlayGroup) In oGroupedByParcel
					iParcelID = oGrouping.Key
					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!+!iKey", iKey, oGrouping.Count)
					If mdicParcels.TryGetValue(iParcelID, oParcel) Then

						'	oRow.Item(TplnParcel.BlockStatusNameFieldName) = oParcel.PlanStateText(iOverlayIndex)
						dParcelInArea = 0.0
						iParcelRowIndex = oDataTable.Rows.Count
						Dim oGroupedByLanduse As IEnumerable(Of IGrouping(Of Integer, TplnOverlayGroup)) = oGrouping.GroupBy(Function(fb) fb.LanduseID)
						For Each oGrouping1 As IGrouping(Of Integer, TplnOverlayGroup) In oGroupedByLanduse
							oRow = oDataTable.NewRow
							oRow.Item(TplnParcel.BlockFullFieldName) = oParcel.BlockFull
							oRow.Item(TplnParcel.NameFieldName) = oParcel.Name
							oRow.Item(TplnParcel.LegalAreaFieldName) = oParcel.LegalArea(False)
							iLanduseID = oGrouping1.Key
							dLanduseArea = 0.0
							For Each oOverlayGroup As TplnOverlayGroup In oGrouping1
								'dParcelInArea += oOverlayGroup.GetOptionArea(iDataOption)
								dLanduseArea += oOverlayGroup.GetOptionArea(iDataOption)

								'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!!OverlayGr=" & iRegionNo.ToString(), iParcelID, oOverlayGroup.ParcelID, oOverlayGroup.LotID, oOverlayGroup.GroupID, oOverlayGroup.AcadArea, oOverlayGroup.CalcArea, oOverlayGroup.CalcArea2, oOverlayGroup.RoundedArea, oOverlayGroup.CalcGroupArea, oOverlayGroup.CalcGroupArea2)
							Next
							oRow.Item(TplnParcel.LanduseAreaFieldName) = dLanduseArea
							dParcelInArea += dLanduseArea
							'oRow.Item(TopoReader.msAreaFldName) = dParcelInArea
							TplnLot.GetLanduseInfo(iTopoPurpose, iLanduseID, sLanduseName, iLanduseOrder)
							''''''''''''''''''''''''''''''oRow.Item(TplnParcel.LanduseNameFieldName) = TplnLot.GetLanduseNameNew(iTopoPurpose, iLanduseID)
							''''191120
							oRow.Item(TplnParcel.LanduseNameFieldName) = sLanduseName
							oRow.Item(TplnParcel.ParcelOrderFieldName) = oParcel.ParcelNo
							oRow.Item(TplnParcel.BlockFieldName) = oParcel.BlockNo                   '42
							oRow.Item(TplnParcel.BlockAddFieldName) = oParcel.BlockAdd
							oRow.Item(TplnParcel.LanduseOrderFieldName) = iLanduseOrder

							oDataTable.Rows.Add(oRow)

							dSumLanduseArea += dLanduseArea
						Next
						For iIndex = iParcelRowIndex To oDataTable.Rows.Count - 1
							oRow = oDataTable.Rows.Item(iIndex)
							oRow.Item(TopoReader.msAreaFldName) = dParcelInArea
						Next

						dSumLegalArea += oParcel.LegalArea(False)
						dSumInArea += dParcelInArea
					End If
				Next
				Dim sOrderBy As String = TplnParcel.BlockFieldName & "," & TplnParcel.BlockAddFieldName & "," & TplnParcel.ParcelOrderFieldName & "," & TplnParcel.LanduseOrderFieldName
				oDataView = New DataView(oDataTable, String.Empty, sOrderBy, Data.DataViewRowState.CurrentRows)
				'	DMCommon.Debug.ExcelLog.SetDataTable(0, "!Rep 5", oDataView)

				Dim iaAcadColumns() As Integer = {0, 1, 2, 3, 4, 5}
				iaColumns = iaAcadColumns
				ReDim oaTotals(2)

				oaTotals(0) = dSumLanduseArea
				oaTotals(1) = dSumInArea

				oaTotals(2) = dSumLegalArea
			End If


		End Sub
		Private Shared Function zzCreateLanduseTable() As Data.DataTable
			Dim oResTable As Data.DataTable = New Data.DataTable("Landuse")
			With oResTable.Columns
				.Add(TplnParcel.BlockFullFieldName, GetType(System.String))
				.Add(TplnParcel.NameFieldName, GetType(System.String))
				.Add(TplnParcel.LegalAreaFieldName, GetType(System.Double))     '5
				.Add(TopoReader.msAreaFldName, GetType(System.Double))
				.Add(TplnParcel.LanduseAreaFieldName, GetType(System.Double))

				'.Add(TplnParcel.msBlockStatusNameFieldName, GetType(System.String))
				.Add(TplnParcel.LanduseNameFieldName, GetType(System.String))
				.Add(TplnParcel.BlockFieldName, GetType(System.Int32))                        '42
				.Add(TplnParcel.BlockAddFieldName, GetType(System.Int32))                        '43
				.Add(TplnParcel.ParcelOrderFieldName, GetType(System.Int32))
				.Add(TplnParcel.LanduseOrderFieldName, GetType(System.Int32))


			End With
			Return oResTable
		End Function
		Private Shared Function zzCreateRepLotTable() As Data.DataTable
			Dim oResTable As Data.DataTable = New Data.DataTable("Lots")
			With oResTable.Columns
				.Add(TplnParcel.BlockFullFieldName, GetType(System.String))
				.Add(TplnParcel.NameFieldName, GetType(System.String))
				.Add(TplnParcel.LegalAreaFieldName, GetType(System.Double))     '5
				.Add(TopoReader.msAreaFldName, GetType(System.Double))

				.Add(TplnLot.NameFieldName, GetType(System.String))
				.Add(TplnParcel.LanduseNameFieldName, GetType(System.String))
				.Add(TplnLot.msAreaApprFieldName, GetType(System.Double))

				.Add(TplnParcel.BlockFieldName, GetType(System.Int32))                        '42
				.Add(TplnParcel.BlockAddFieldName, GetType(System.Int32))                        '43
				.Add(TplnParcel.ParcelOrderFieldName, GetType(System.Int32))
				.Add(TplnParcel.msLotOrderFieldName, GetType(System.Int64))


			End With
			Return oResTable
		End Function
		Private Shared Function zzCreateLotsContentTable() As Data.DataTable
			Dim oLotsContentTable As Data.DataTable = New Data.DataTable("InPlan")

			oLotsContentTable = New Data.DataTable("LotContents")
			With oLotsContentTable.Columns
				.Add(TplnParcel.msLotNameFieldName, GetType(System.String))

				.Add(TopoReader.msAreaFldName, GetType(System.Double))
				.Add(TplnParcel.BlockFullFieldName, GetType(System.String))

				.Add(TplnParcel.msParcelEntireFieldName, GetType(System.String))
				.Add(TplnParcel.msParcelPartialFieldName, GetType(System.String))

				.Add(TplnParcel.BlockFieldName, GetType(System.Int32))
				.Add(TplnParcel.BlockAddFieldName, GetType(System.Int32))
				.Add(TplnParcel.msLotOrderFieldName, GetType(System.Int32))


			End With
			Return oLotsContentTable
		End Function
		Private Shared Function zzCreateBlockContentTable() As Data.DataTable
			Dim oResTable As System.Data.DataTable
			oResTable = New Data.DataTable("Blocks")
			With oResTable.Columns
				.Add(TplnParcel.BlockFullFieldName, GetType(System.String))
				.Add(TplnParcel.BlockStatusNameFieldName, GetType(System.String))
				.Add(TplnParcel.msParcelEntireFieldName, GetType(System.String))
				.Add(TplnParcel.msParcelPartialFieldName, GetType(System.String))

				.Add(TplnParcel.BlockFieldName, GetType(System.Int32))
				.Add(TplnParcel.BlockAddFieldName, GetType(System.Int32))

			End With
			Return oResTable
		End Function
		Private Shared Function zzCreateInPlanTable() As Data.DataTable
			Dim oResTable As Data.DataTable = New Data.DataTable("InPlan")
			With oResTable.Columns

				.Add(TplnParcel.BlockFullFieldName, GetType(System.String))
				.Add(TplnParcel.NameFieldName, GetType(System.String))
				.Add(TplnParcel.LegalAreaFieldName, GetType(System.Double))     '5
				.Add(TopoReader.msAreaFldName, GetType(System.Double))
				.Add(TplnParcel.msParcelPartialFieldName, GetType(System.String))

				.Add(TplnParcel.BlockFieldName, GetType(System.Int32))                        '42
				.Add(TplnParcel.BlockAddFieldName, GetType(System.Int32))                        '43
				.Add(TplnParcel.ParcelOrderFieldName, GetType(System.Int32))

			End With
			Return oResTable
		End Function

		Private Shared Function zzCreateInPlanTableII() As Data.DataTable
			Dim oResTable As Data.DataTable = New Data.DataTable("InPlan")
			With oResTable.Columns

				.Add(TplnParcel.BlockFullFieldName, GetType(System.String))
				.Add(TplnParcel.NameFieldName, GetType(System.String))
				.Add(TplnParcel.LegalAreaFieldName, GetType(System.Double))     '5
				.Add(TopoReader.msAreaFldName, GetType(System.Double))
				.Add(TplnParcel.msParcelPartialFieldName, GetType(System.String))

				.Add(TplnParcel.BlockFieldName, GetType(System.Int32))                        '42
				.Add(TplnParcel.BlockAddFieldName, GetType(System.Int32))                        '43
				.Add(TplnParcel.ParcelOrderFieldName, GetType(System.Int32))

			End With
			Return oResTable
		End Function

		Private Shared Function zzCreateLotTable() As Data.DataTable
			Dim oResTable As Data.DataTable = New Data.DataTable("Landuses")
			With oResTable.Columns

				.Add(TplnLot.NameFieldName, GetType(System.String))
				.Add(TplnParcel.LanduseNameFieldName, GetType(System.String))         '2
				.Add(TopoReader.msAreaFldName, GetType(System.Double))                  '3
				.Add(TplnParcel.LanduseOrderFieldName, GetType(System.Int32))
				.Add(TplnLot.LotOrderFieldName, GetType(System.Int32))                  '3
				'3
			End With
			Return oResTable
		End Function
		Public Shared Sub CalculateOverlayGroups(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean)
			'''''''''''''''''''''''''''''''''''''''''DMCommon.Debug.MsgBox("09_723", bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
			If bMerge OrElse bUnion OrElse bFDO_Overlay Then
				'    Dim taTest(mcolRegionForcedArea.Count - 1) As BalanceArea.ConstArea
				Dim baOverlayArray() As Boolean = UnionPgonArea.GetOverlayArray(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
				For iOverlayIndex As DMAcadExt.enOverlayIndex = 0 To DMAcadExt.enOverlayIndex.OverlayIndexUB
					If baOverlayArray(iOverlayIndex) Then
						If mdicOverlayGroups(iOverlayIndex) IsNot Nothing Then

							'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!Calculate2New", iOverlayIndex)
							mdicOverlayGroups(iOverlayIndex).Calculate2New(mcolLotForcedArea, mcolRegionForcedArea)
						Else
							System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString(), "01_554d")
						End If

					End If
				Next
			End If
		End Sub
		Private Shared Sub zzDistribute(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean)
			If bMerge OrElse bUnion OrElse bFDO_Overlay Then
				'    Dim taTest(mcolRegionForcedArea.Count - 1) As BalanceArea.ConstArea
				Dim baOverlayArray() As Boolean = UnionPgonArea.GetOverlayArray(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
				Dim oRegion As TplnRegion = Nothing
				For iOverlayIndex As DMAcadExt.enOverlayIndex = 0 To DMAcadExt.enOverlayIndex.OverlayIndexUB
					If baOverlayArray(iOverlayIndex) Then
						If mdicOverlayGroups(iOverlayIndex) IsNot Nothing Then
							For Each oOverlayGroup As TplnOverlayGroup In mdicOverlayGroups(iOverlayIndex).Values

								If oOverlayGroup.IsInPlan AndAlso mdicRegions.TryGetValue(oOverlayGroup.GroupID, oRegion) Then
									oRegion.AddOverlayGroup(iOverlayIndex, oOverlayGroup)
								End If

							Next

						Else
							System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString(), "01_554e")
						End If

					End If
				Next
			End If
		End Sub

		Public Shared Sub CalculateRegion(iRegion As Integer)
			If mdicParcels IsNot Nothing Then
				Dim bMerge As Boolean, bUnion As Boolean, bFDO_Overlay As Boolean
				Dim bApproved As Boolean, bProposed As Boolean, bParcel As Boolean, bBlock As Boolean, bMerhav As Boolean
				bApproved = ApprMapThemeData.IsNotEmpty
				bProposed = PropMapThemeData.IsNotEmpty
				bParcel = ParcelMapThemeData.IsNotEmpty
				bBlock = BlockMapThemeData.IsNotEmpty
				bMerhav = MerhavMapThemeData.IsNotEmpty

				If miOverlayMethod = DMAcadExt.enOverlayMethod.Merge Then
					bMerge = True
				ElseIf miOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay Then
					bFDO_Overlay = True
				End If
				If bApproved Then
					TplnParcel.CreateLanduseTable(DMAcadExt.enTopoPurpose.Approved, True)
				Else
					TplnParcel.DisposeLanduseTable(DMAcadExt.enTopoPurpose.Approved, True)
				End If
				If bProposed Then
					TplnParcel.CreateLanduseTable(DMAcadExt.enTopoPurpose.Proposed, True)
				Else
					TplnParcel.DisposeLanduseTable(DMAcadExt.enTopoPurpose.Proposed, True)
				End If
				'DMCommon.Debug.MsgBox("!CalculateRegion", iRegion)
				zzCalculateOverlayGroups(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed, iRegion)
				UpdateOverlayTable(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
				UpdateOverlayTable(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
				zzCalculateRegionParcel(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed, iRegion)
				zzCalculateRegionLot(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed, iRegion)
				TplnLot.CalcRegionLanduses(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed, iRegion)
				TplnParcel.CreateMainRegionDataTable(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
				'  MessageBox.Show(iRegion.ToString() & vbCrLf & bApproved.ToString(), "04_400d")


				'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "TplnProject", "CalculateRegion", iRegion)
				For Each oParcel As TplnParcel In mdicParcels.Values
					oParcel.AddDataToMainRegionTable(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed, iRegion)
					If bApproved Then
						oParcel.AddDataToLanduseRegionTable(bFDO_Overlay, DMAcadExt.enTopoPurpose.Approved, iRegion)
					End If
					If bProposed Then
						oParcel.AddDataToLanduseRegionTable(bFDO_Overlay, DMAcadExt.enTopoPurpose.Proposed, iRegion)
					End If

				Next
				TplnParcel.CalculateRegionBlocks(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
				'    MessageBox.Show(mdicParcels.Count.ToString() & ":" & TplnParcel.MainDataTable.Rows.Count.ToString() & ":" & TplnParcel.MainRegionDataTable.Rows.Count.ToString(), "05_525")
			End If





		End Sub
		Public Shared Function LoadParcelsCP() As Boolean
			Dim sParcelCPLayer As String = "1602"
			Dim oList As IList(Of Polyline) = DMAcadExt.AcadTransaction.GetAcadPolylines(sParcelCPLayer, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)


			If oList IsNot Nothing Then

				Dim oParcel As TplnParcel
				'	Dim colPolygons As PolygonCollection = oParcelTopology.GetPolygons()
				If mdicParcels Is Nothing Then
					mdicParcels = New TPlanGraph.TplnParcels(False, True)
				Else
					mdicParcels.Clear()
				End If
				'    System.Windows.Forms.MessageBox.Show(CStr("LoadParcelsCP" & vbCrLf & oList.Count) & ":" & CStr(mdicParcels.Count), "01_684")

				For Each oPolygon As Entity In oList

					oParcel = New TplnParcel(oPolygon)
					If oParcel.Correct Then 'TEMP
						'DMAcadExt.AcadDocument.WriteMessage("#11_201:" & oParcel.LegalArea)
						mdicParcels.AddParcel(oParcel)
						'	oParcel.AddDataToMainTable()
						If mdicBlocks IsNot Nothing Then
							mdicBlocks.AddParcel(oParcel)
						End If
					End If
					oPolygon.Dispose()
					oPolygon = Nothing
					'oParcel.Terminate()
					oParcel = Nothing
				Next


				'colPolygons.Clear()
				'	colPolygons.Dispose()

				TplnParcel.CheckLegalArea()
				' System.Windows.Forms.MessageBox.Show(CStr(oList.Count) & ":" & CStr(mdicParcels.Count), "01_685")
				Return True
			Else
				Return False
			End If
		End Function
		Public Shared Sub LoadCentroids()
			Dim colBlockRefs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetBlockRefs("1603", "1603")
			Dim oBlockRef As BlockReference
			Dim oXDataParcel As DMAcadExt.TplnXDataParcel
			Dim iParcelID As Integer
			Dim oParcel As TplnParcel = Nothing
			' System.Windows.Forms.MessageBox.Show(CStr(colBlockRefs.Count), "01_230K")
			For Each tAcObjID As ObjectId In colBlockRefs
				oBlockRef = DMAcadExt.AcadTransaction.GetBlockRef(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
				If oBlockRef IsNot Nothing Then
					oXDataParcel = New DMAcadExt.TplnXDataParcel(oBlockRef.XData)
					iParcelID = oXDataParcel.DataID
					If mdicParcels.TryGetValue(iParcelID, oParcel) Then
						oParcel.AddCentroid(oBlockRef)
					Else
						DMAcadExt.AcadDocument.WriteMessage("#A18: " & iParcelID.ToString())
					End If
				Else
					DMAcadExt.AcadDocument.WriteMessage("#A14: " & tAcObjID.ToString())
				End If
			Next
		End Sub
		Public Shared Function LoadParcelsOldVer() As Boolean


			Dim sParcelTopoName As String = TopoDefs.Item(New DMAcadExt.TopoDefID(enTopoPurpose.Parcel)).Name
			MessageBox.Show("LoadParcels" & vbCrLf & sParcelTopoName, "06_199")
			Dim oParcelTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sParcelTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
			If oParcelTopology IsNot Nothing Then
				Dim oParcel As TplnParcel
				Dim colPolygons As PolygonCollection = oParcelTopology.GetPolygons()
				If mdicParcels Is Nothing Then
					mdicParcels = New TPlanGraph.TplnParcels(False, True)
				Else
					mdicParcels.Clear()
				End If

				If True Then

					For Each oPolygon As Polygon In colPolygons
						'DMAcadExt.AcadDocument.WriteMessageLog("#11_120:" & 
						oParcel = New TplnParcel(oPolygon)
						If oParcel.Correct Then 'TEMP
							mdicParcels.AddParcel(oParcel)
							'	oParcel.AddDataToMainTable()
							If mdicBlocks IsNot Nothing Then
								mdicBlocks.AddParcel(oParcel)
							End If
						End If
						oPolygon.Dispose()
						oPolygon = Nothing
						'oParcel.Terminate()
						oParcel = Nothing
					Next
				End If
				'	System.Windows.Forms.MessageBox.Show(CStr(mdicParcels.Count), "02_203")
				'  colPolygons.Clear()
				'	colPolygons.Dispose()
				colPolygons = Nothing
				oParcelTopology.Close()
				'	oParcelTopology.Dispose()
				oParcelTopology = Nothing
				TplnParcel.CheckLegalArea()
				Return True

			Else
				System.Windows.Forms.MessageBox.Show("Parcel Topology Is Nothing")
				Return False
			End If
		End Function


		Public Shared Function LoadParcels(Optional bMust As Boolean = True) As Boolean
			Dim sParcelTopoName As String
			sParcelTopoName = TplnParcel.GetLineTopoName()

			Dim oParcelTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sParcelTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)

			If oParcelTopology Is Nothing Then
				sParcelTopoName = TplnParcel.GetTopoName()

				DMCommon.Debug.MsgBox("12_220", sParcelTopoName)
				oParcelTopology = TopoManager.TopoCreator.GetOpenedTopology(sParcelTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
			End If

			If oParcelTopology IsNot Nothing Then
				Try
					Dim oParcel As TplnParcel
					Dim colPolygons As PolygonCollection = oParcelTopology.GetPolygons()
					If mdicParcels Is Nothing Then
						mdicParcels = New TPlanGraph.TplnParcels(False, True)
					Else
						mdicParcels.Clear()
					End If

					'!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!System.Windows.Forms.MessageBox.Show(CStr("LoadParcelsNew" & vbCrLf & colPolygons.Count), "01_687c")


					For Each oPolygon As Polygon In colPolygons
						'DMAcadExt.AcadDocument.WriteMessageLog("#11_120:" & 
						oParcel = New TplnParcel(oPolygon)
						If oParcel.Correct Then 'TEMP
							mdicParcels.AddParcel(oParcel)
							'	oParcel.AddDataToMainTable()
							If mdicBlocks IsNot Nothing Then
								mdicBlocks.AddParcel(oParcel)
							End If
						End If

						oPolygon.Dispose()
						oPolygon = Nothing
						'oParcel.Terminate()
						oParcel = Nothing
					Next



					'  colPolygons.Clear()					colPolygons.Dispose()
					colPolygons = Nothing
					oParcelTopology.Close()
					'	oParcelTopology.Dispose()
					oParcelTopology = Nothing
					TplnParcel.CheckLegalArea()
					mdicParcels.SetDoubleNameMsg()
					'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!mdicParcels", DMCommon.Debug.ColCount(mdicParcels))
					Return True
				Catch oEx As Exception
					If oParcelTopology IsNot Nothing Then
						oParcelTopology.Close()
					End If
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "02_217p")
					'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!mdicParcels", oEx.Message, oEx.StackTrace)
					Return False
				End Try

			Else
				If bMust Then
					System.Windows.Forms.MessageBox.Show("Topology '" & sParcelTopoName & "' was not found", "06_250")
				End If

				Return False
			End If 'oParcelTopology IsNot Nothing

		End Function
		Public Shared Function LoadExpros() As Boolean
			TplnExpro.CreateMainDataTable()
			Dim sExproTopoName As String = TplnExpro.GetTopoName()


			Dim oExproTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sExproTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
			If oExproTopology IsNot Nothing Then
				Dim oExpro As TplnExpro
				Dim colPolygons As PolygonCollection = oExproTopology.GetPolygons()
				If mdicExpros Is Nothing Then
					mdicExpros = New TPlanGraph.TplnExpros()
				Else
					mdicExpros.Clear()
				End If


				For Each oPolygon As Polygon In colPolygons

					oExpro = New TplnExpro(oPolygon)
					If oExpro.Correct Then  'TEMP
						mdicExpros.AddExpro(oExpro)
						oExpro.AddDataToMainTable(False, False, False, False, False)
					Else
						System.Windows.Forms.MessageBox.Show(CStr("Not oExpro.Correct" & vbCrLf & colPolygons.Count), "01_220")
					End If

					oPolygon.Dispose()
					oPolygon = Nothing
					'oParcel.Terminate()
					oExpro = Nothing
				Next

				'	DMCommon.Debug.MsgBox("LoadExpros", sExproTopoName, mdicExpros.Count, TplnExpro.MainView.Count)

				'  colPolygons.Clear()
				colPolygons.Dispose()
				colPolygons = Nothing
				oExproTopology.Close()
				'	oParcelTopology.Dispose()
				oExproTopology = Nothing
				'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!mdicExpros", DMCommon.Debug.ColCount(mdicExpros))
				Return True
			Else
				DMCommon.Debug.MsgBox("06_257", "Expro Topology Is Nothing", sExproTopoName)

				Return False
			End If
		End Function

		Public Shared Function LoadZones() As Boolean
			TplnZone.CreateMainDataTable()
			Dim sZoneTopoName As String = TplnZone.GetTopoName()
			'System.Windows.Forms.MessageBox.Show(sExproTopoName, "06_285")
			Dim oZoneTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sZoneTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
			If oZoneTopology IsNot Nothing Then
				Dim oZone As TplnZone
				Dim colPolygons As PolygonCollection = oZoneTopology.GetPolygons()
				If mdicZones Is Nothing Then
					mdicZones = New TPlanGraph.TplnZones()
				Else
					mdicZones.Clear()
				End If


				For Each oPolygon As Polygon In colPolygons
					'DMAcadExt.AcadDocument.WriteMessageLog("#11_120:" & 
					oZone = New TplnZone(oPolygon)
					If oZone.Correct Then  'TEMP
						mdicZones.AddZone(oZone)
						oZone.AddDataToMainTable()
					Else
						System.Windows.Forms.MessageBox.Show(CStr("Not oZone.Correct" & vbCrLf & colPolygons.Count), "01_218")
					End If
					oPolygon.Dispose()
					oPolygon = Nothing
					'oParcel.Terminate()
					oZone = Nothing
				Next


				'   DMCommon.Debug.MsgBox("AddDataToMainTable", TplnExpro.MainView.Count)
				'	System.Windows.Forms.MessageBox.Show(CStr(mdicParcels.Count), "02_203")
				'  colPolygons.Clear()
				'	colPolygons.Dispose()
				colPolygons = Nothing
				oZoneTopology.Close()
				'	oParcelTopology.Dispose()
				oZoneTopology = Nothing

				Return True
			Else
				System.Windows.Forms.MessageBox.Show("Expro Topology Is Nothing", "06_257")
				Return False
			End If
		End Function

		Public Shared Function LoadMerhav() As Boolean

			TplnMerhav.CreateMainDataTable()
			Dim sMerhavTopoName As String = TplnMerhav.GetTopoName()

			' 
			Dim oMerhavTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sMerhavTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)

			If oMerhavTopology IsNot Nothing Then
				Dim oMerhav As TplnMerhav
				Dim colPolygons As PolygonCollection = oMerhavTopology.GetPolygons()
				If mdicMerhav Is Nothing Then
					mdicMerhav = New TPlanGraph.TplnMerhavDic()

				Else
					mdicMerhav.Clear()
				End If


				If mdicBlocks Is Nothing Then
					System.Windows.Forms.MessageBox.Show("mdicBlocks Is Nothing" & vbCrLf & colPolygons.Count.ToString(), "01_683t")

					System.Windows.Forms.MessageBox.Show(mdicBlocks.Count.ToString(), "01_684r")
				End If

				For Each oPolygon As Polygon In colPolygons
					'DMAcadExt.AcadDocument.WriteMessageLog("#11_120:" & 
					oMerhav = New TplnMerhav(oPolygon)
					If oMerhav.Correct Then  'TEMP
						oMerhav.AllBlocks = mdicBlocks
						mdicMerhav.AddMerhav(oMerhav)
						oMerhav.AddDataToMainTable(False, False, False, False, False)
					Else
						System.Windows.Forms.MessageBox.Show(CStr("Not oExpro.Correct" & vbCrLf & colPolygons.Count), "01_219")
					End If
					oPolygon.Dispose()
					oPolygon = Nothing
					'oParcel.Terminate()
					oMerhav = Nothing
				Next
				'   DMCommon.Debug.MsgBox("06_287", False, sMerhavTopoName, mdicMerhav, mdicMerhav.Count)
				'  System.Windows.Forms.MessageBox.Show(CStr("LoadMerhav" & vbCrLf & mdicMerhav.Count), "01_689x")
				'	System.Windows.Forms.MessageBox.Show(CStr(mdicParcels.Count), "02_203")
				'  colPolygons.Clear()
				'	colPolygons.Dispose()
				colPolygons = Nothing
				oMerhavTopology.Close()
				'	oParcelTopology.Dispose()
				oMerhavTopology = Nothing
				' System.Windows.Forms.MessageBox.Show(CStr(mdicMerhav.Count), "06_656")
				Return True
			Else
				System.Windows.Forms.MessageBox.Show("Expro Topology Is Nothing", "06_257a")
				Return False
			End If



		End Function
		Public Shared Function LoadUd_Parcels() As Boolean
			Dim sUd_ParcelTopoName As String = UD_ParcelMapThemeData.TopoName
			Dim sBlocksName As String = UD_ParcelMapThemeData.NodeBlocks

			Dim sBlockName As String = UD_ParcelMapThemeData.NodeBlock
			'''''''''''''''''''''  DMCommon.Debug.MsgBox("12_223", sUd_ParcelTopoName, sBlockName, sBlocksName)
			Return True
		End Function
		Public Shared Function LoadFragments() As Boolean

			Dim sFragmentTopoName As String = FragmentMapThemeData.TopoName
			Dim sBlocksName As String = FragmentMapThemeData.NodeBlocks
			' '''''''''''''''''''  DMCommon.Debug.MsgBox("12_221", sFragmentTopoName, sBlocksName)

			If sBlocksName IsNot Nothing Then

				Dim tNodeAcObjId As ObjectId
				Dim sBlockName As String = FragmentMapThemeData.NodeBlock
				Dim saBlockNames() As String = Split(sBlocksName, ",")
				'   DMCommon.Debug.MsgBox("12_000", sFragmentTopoName, sBlocksName, sBlockName)

				Dim oFragmentTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sFragmentTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)

				If oFragmentTopology IsNot Nothing Then



					Dim oTopoBlock As TplnBlock = Nothing
					'   Dim oFragmentTopoScheme As TopoScheme.tsTopology = New TopoScheme.tsTopology(sFragmentTopoName)
					Dim colNodes As ObjectIdCollection = New ObjectIdCollection()
					'   oFragmentTopoScheme.Load(False, oFragmentTopology)

					For Each oNode As Node In oFragmentTopology.GetNodes()
						Try
							tNodeAcObjId = oNode.Entity
						Catch oMapEx As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx, False, "LoadFragments", False, "Loc=" & oNode.Location.ToString())
							DMAcadExt.AppMessages.AddMessage(True, oNode.Location.X, oNode.Location.Y, "", "Point was not found", False) '
							tNodeAcObjId = ObjectId.Null
						End Try



						If tNodeAcObjId.IsNull Then
							DMAcadExt.AcadDocument.WriteMessage("!Nd=" & oNode.Location.ToString())
						Else
							colNodes.Add(tNodeAcObjId)
						End If

					Next

					For iIndex As Integer = 0 To saBlockNames.GetUpperBound(0)
						sBlockName = saBlockNames(iIndex)
						If Not String.IsNullOrEmpty(sBlockName) Then
							zzBlockRefCheckOutOfList(sBlockName, colNodes)
						End If
					Next




					oFragmentTopology.Close()
				End If
			End If
			Return True
		End Function
		Public Shared Function LoadOwnershipNotes(Optional bMust As Boolean = True) As Boolean

			Dim sOwnershipNoteTopoName As String = TplnOwnershipNote.GetTopoName()
			Dim oOwnershipNoteTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sOwnershipNoteTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)



			If oOwnershipNoteTopology IsNot Nothing Then
				Try
					Dim oOwnershipNote As TplnOwnershipNote
					Dim colPolygons As PolygonCollection = oOwnershipNoteTopology.GetPolygons()
					If mdicOwnershipNotes Is Nothing Then
						mdicOwnershipNotes = New TPlanGraph.TplnOwnershipNotes()
					Else
						mdicOwnershipNotes.Clear()
					End If

					'!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!System.Windows.Forms.MessageBox.Show(CStr("LoadParcelsNew" & vbCrLf & colPolygons.Count), "01_687c")


					For Each oPolygon As Polygon In colPolygons

						oOwnershipNote = New TplnOwnershipNote(oPolygon)
						If oOwnershipNote.Correct Then 'TEMP
							mdicOwnershipNotes.AddOwnershipNote(oOwnershipNote)
							'	oParcel.AddDataToMainTable()
							'	DMCommon.Debug.MsgBox("13_121", mdicOwnershipNotes.Count, oOwnershipNote.TopoID, oOwnershipNote.Paragraph19Area, oOwnershipNote.LeasingArea)
						End If

						oPolygon.Dispose()
						oPolygon = Nothing
						'oParcel.Terminate()

					Next


					'	DMCommon.Debug.MsgBox("02_203own", mdicOwnershipNotes.Count)
					'  colPolygons.Clear()
					colPolygons.Dispose()
					colPolygons = Nothing
					oOwnershipNoteTopology.Close()
					'	oParcelTopology.Dispose()
					oOwnershipNoteTopology = Nothing
					TplnParcel.CheckLegalArea()
					mdicParcels.SetDoubleNameMsg()
					Return True
				Catch oEx As Exception
					If oOwnershipNoteTopology IsNot Nothing Then
						oOwnershipNoteTopology.Close()
					End If
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "02_217")
					Return False
				End Try

			Else
				If bMust Then
					System.Windows.Forms.MessageBox.Show("Topology '" & sOwnershipNoteTopoName & "' was not found", "06_252")
				End If

				Return False
			End If 'oParcelTopology IsNot Nothing
		End Function
		Private Shared Sub zzBlockRefCheckOutOfListOLd(sBlockName As String, ByRef colBlockRefList As ObjectIdCollection)
			Const sMsg1 As String = "נקודת הגבול לא שייכת לטופולוגיה"
			Dim shColor As Short = 31S
			Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(sBlockName)
			Dim colBlocks As ObjectIdCollection = DMAcadExt.AcadTransaction.GetAllBlockRefs(sBlockName)

			Dim oCircleMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Circle)
			oAcadBlock.LoadAllReferences()
			'    mdicMarkEntities = New ObjectIdCollection()
			Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
			If colBlocks IsNot Nothing Then
				For Each tBlockObjID As ObjectId In colBlocks
					If Not colBlockRefList.Contains(tBlockObjID) Then
						tPoint = DMAcadExt.AcadTransaction.GetBlockRefInsPoint(tBlockObjID)
						oCircleMarkBlock.MarkPoint(tPoint, shColor)
						DMAcadExt.AppMessages.AddMessage(True, tPoint.X, tPoint.Y, "", "Block '" & sBlockName & "' Topology 'Fragments'" & vbCrLf & sMsg1, False) '
					End If
				Next


			End If
		End Sub
		Private Shared Sub zzBlockRefCheckOutOfList(sBlockName As String, ByRef colBlockRefList As ObjectIdCollection)
			Const sMsg1 As String = "נקודת הגבול לא שייכת לטופולוגיה"
			Dim shColor As Short = 31S
			Dim oAcadBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(sBlockName)
			Dim tBlockRefData As DMAcadExt.BlockRefData
			Dim sPointName As String
			Dim oCircleMarkBlock As DMAcadExt.MarkBlock = New DMAcadExt.MarkBlock(DMAcadExt.MarkBlock.enMarkBlockType.Circle)
			oAcadBlock.Fields = {"POINT_NAME"}
			oAcadBlock.OpenForRead()
			oAcadBlock.LoadAllReferences()
			Dim colBlocks As ObjectIdCollection = oAcadBlock.BlockRefObjIds
			'    mdicMarkEntities = New ObjectIdCollection()
			Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
			If colBlocks IsNot Nothing Then
				For Each tBlockObjID As ObjectId In colBlocks
					If Not colBlockRefList.Contains(tBlockObjID) Then
						tPoint = DMAcadExt.AcadTransaction.GetBlockRefInsPoint(tBlockObjID)
						oCircleMarkBlock.MarkPoint(tPoint, shColor)
						tBlockRefData = oAcadBlock.GetBlockRefData(tBlockObjID)
						sPointName = tBlockRefData.AttribValues(0)
						DMAcadExt.AppMessages.AddMessage(True, tPoint.X, tPoint.Y, "", "Block '" & sBlockName & "' Topology 'Fragments'" & ", Point " & sPointName & vbCrLf & sMsg1, False) '
					End If
				Next


			End If
		End Sub

		Public Shared Sub InitBlockDic()
			If mdicBlocks Is Nothing Then
				mdicBlocks = New TPlanGraph.TplnBlocks()
			Else
				mdicBlocks.Clear()
			End If
		End Sub
		Public Shared Sub InitRegionDic()
			If mdicRegions Is Nothing Then
				mdicRegions = New TPlanGraph.TplnRegions()
			Else
				mdicRegions.Clear()
			End If
		End Sub
		Public Shared ReadOnly Property Regions As TPlanGraph.TplnRegions
			Get
				Return mdicRegions
			End Get
		End Property

		Public Shared Sub TestBlockDic(sCaption As String)
			Dim s As String
			If mdicBlocks Is Nothing Then
				s = "Is Nothing"
			ElseIf mdicBlocks.Count = 0 Then

				s = "Count = 0"
			Else
				s = "N=" & CStr(mdicBlocks.Count) & vbCrLf
				For Each oBlock As TplnBlock In mdicBlocks.Values
					s &= oBlock.BlockName
					Exit For
				Next

			End If
			MessageBox.Show(s, sCaption)
		End Sub
		Public Shared Sub LoadBlocks()
			Dim sBlockTopologyName As String = TplnBlock.GetTopoName()
			Dim oBlockTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sBlockTopologyName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
			Dim oAllBlockCol As ObjectIdCollection = Nothing
			Dim oBlock As TplnBlock
			Dim oTopoBlock As TplnBlock = Nothing

			Dim sBlockName As String = "1601"

			oAllBlockCol = DMAcadExt.AcadTransaction.GetAllBlockRefs(sBlockName, String.Empty)
			If oAllBlockCol IsNot Nothing Then
				Dim iBlockByParcelCount As Integer = mdicBlocks.Count

				For Each tAcObjID As ObjectId In oAllBlockCol
					oBlock = New TplnBlock(tAcObjID)

					If mdicBlocks.TryGetValue(oBlock.Key, oTopoBlock) Then
						If oTopoBlock.CentroidAcObjID <> oBlock.CentroidAcObjID Then

							If oTopoBlock.BlockNo <> 0 Then
								''''ERROR MSG
								' DMAcadExt.AcadDocument.WriteMessage("Err #1604: '" & oBlock.BlockName & " ' already exists (block 1601)")
								DMAcadExt.AppMessages.AddMessage(True, oTopoBlock.CentroidX, oTopoBlock.CentroidY, "", "Err #1604: '" & oBlock.BlockName & " ' already exists (block 1601)", False)
							Else
								DMAcadExt.AppMessages.AddMessage(True, oTopoBlock.CentroidX, oTopoBlock.CentroidY, "", "מספר גוש לא נמצא", False)
							End If

						End If
					Else
						mdicBlocks.AddBlock(oBlock, False)
					End If
				Next

				'    MessageBox.Show(sBlockName & vbCrLf & CStr(oAllBlockCol.Count) & vbCrLf & CStr(mdicBlocks.Count), "03_509n")

				oAllBlockCol.Dispose()
				oAllBlockCol = Nothing



			End If
		End Sub
		Public Shared Sub LoadRegionTopology()
			Dim oRegion As TplnRegion
			'	Dim sRegionTopologyName As String = TplnRegion.GetTopoName()
			'   Dim oRegionTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sRegionTopologyName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
			Dim oRegionTopology As TopologyModel = zzGetRegionTopology()


			If oRegionTopology IsNot Nothing Then

				Dim colPolygons As PolygonCollection = oRegionTopology.GetPolygons()

				'	DMCommon.Debug.MsgBox("07_143b", colPolygons.Count)
				For Each oRegionPolygon As Polygon In colPolygons
					oRegion = New TplnRegion(oRegionPolygon)
					If mdicRegions.ContainsTopoID(oRegion.TopoID) Then
						DMCommon.Debug.MsgBox("07_142", "RegionTopoID " & oRegion.TopoID.ToString() & " already exists", oRegion.TopoID)
					Else
						mdicRegions.AddRegion(oRegion)
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!oRegion.TopoID", oRegion.TopoID, oRegion.RegionNo, oRegion.RegionName, oRegion.RegionID_Name)
					End If

				Next
			End If




		End Sub
		Public Shared Sub LoadBlockTopology(bUpdateCentroid As Boolean)

			Dim oTopoBlock As TplnBlock
			Dim sBlockName As String = TplnBlock.CentroidBlockName
			Dim oAllBlockCol As ObjectIdCollection = Nothing


			Dim tTopoDefID As DMAcadExt.TopoDefID = GetDissolveID(DMAcadExt.enTopoPurpose.Parcel)
			Dim sBlockTopologyName As String = "Blocks" 'TopoDefs.Item(tTopoDefID).Name
			Dim sParcelTopologyName As String = "Parcels" 'TopoDefs.Item(tTopoDefID).Name

			Dim oBlockTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sBlockTopologyName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)


			If oBlockTopology IsNot Nothing Then
				Dim oParcelTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sParcelTopologyName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
				Dim oParcelPgon As Polygon = Nothing
				Dim oParcel As TplnParcel
				Dim colPolygons As PolygonCollection = oBlockTopology.GetPolygons()
				If oParcelTopology IsNot Nothing Then
					For Each oBlockPolygon As Polygon In colPolygons
						oTopoBlock = New TplnBlock(oBlockPolygon)
						Try
							oParcelPgon = oParcelTopology.FindPolygon(oBlockPolygon.Centroid)
						Catch oMapEx As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "LoadLusePgons-LotPgon", False, "CenterDisPgon=" & DMAcadExt.TPlnPoint.DispPoint(oBlockPolygon.Centroid))
							'	Return
						End Try

						If oParcelPgon IsNot Nothing Then
							oParcel = GetParcel(oParcelPgon.ID, "CCCCC")
							If oParcel IsNot Nothing Then
								oTopoBlock.UpdateDataByParcel(oParcel, bUpdateCentroid)
							End If
							mdicBlocks.AddBlock(oTopoBlock, True)
						End If
						oBlockPolygon.Dispose()
						oBlockPolygon = Nothing
					Next
					oParcelTopology.Close()
					oParcelTopology = Nothing
				End If

				oBlockTopology.Close()
				oBlockTopology = Nothing
			End If



		End Sub
		Public Shared Sub LoadBlocksOldVer()
			Dim oBlock As TplnBlock
			Dim oTopoBlock As TplnBlock
			Dim sBlockName As String = TplnBlock.CentroidBlockName
			Dim oAllBlockCol As ObjectIdCollection = Nothing


			Dim tTopoDefID As DMAcadExt.TopoDefID = GetDissolveID(DMAcadExt.enTopoPurpose.Parcel)
			Dim sBlockTopologyName As String = "Blocks" 'TopoDefs.Item(tTopoDefID).Name
			Dim oBlockTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sBlockTopologyName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)


			If False And oBlockTopology IsNot Nothing Then
				Dim oParcelTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology("Blocks", Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
				Dim oParcelPgon As Polygon = Nothing
				Dim oParcel As TplnParcel
				Dim colPolygons As PolygonCollection = oBlockTopology.GetPolygons()
				If oParcelTopology IsNot Nothing Then
					For Each oPolygon As Polygon In colPolygons
						oTopoBlock = New TplnBlock(oPolygon)
						Try
							oParcelPgon = oParcelTopology.FindPolygon(oPolygon.Centroid)
						Catch oMapEx As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "LoadLusePgons-LotPgon", False, "CenterDisPgon=" & DMAcadExt.TPlnPoint.DispPoint(oPolygon.Centroid))
							'	Return
						End Try

						If oParcelPgon IsNot Nothing Then
							oParcel = GetParcel(oParcelPgon.ID, "LoadBlocks")
							If oParcel IsNot Nothing Then
								oTopoBlock.UpdateDataByParcel(oParcel, False)
							End If
							mdicBlocks.AddBlock(oTopoBlock, False)
						End If
						oPolygon.Dispose()
						oPolygon = Nothing
					Next
					oParcelTopology.Close()
					oParcelTopology = Nothing
				End If

				oBlockTopology.Close()
				oBlockTopology = Nothing
			End If

			If True Then    ''''''''''''temp
				MessageBox.Show(sBlockName, "03_504")
				oAllBlockCol = DMAcadExt.AcadTransaction.GetAllBlockRefs(sBlockName, String.Empty)
				If oAllBlockCol IsNot Nothing Then
					Dim iBlockByParcelCount As Integer = mdicBlocks.Count
					'MessageBox.Show(sBlockName & vbCrLf & CStr(oAllBlockCol.Count) & vbCrLf & CStr(mdicBlocks.Count), "03_509")
					For Each tAcObjID As ObjectId In oAllBlockCol
						oBlock = New TplnBlock(tAcObjID)

						If mdicBlocks IsNot Nothing AndAlso mdicBlocks.ContainsKey(oBlock.Key) Then

							oTopoBlock = mdicBlocks.Item(oBlock.Key)
							oTopoBlock.LegalArea = oBlock.LegalArea

							oTopoBlock.BlockStatus = oBlock.BlockStatus
						Else
							mdicBlocks.AddBlock(oBlock, False)
						End If
					Next
					If iBlockByParcelCount <> mdicBlocks.Count Then
						MessageBox.Show(sBlockName & " - " & CStr(oAllBlockCol.Count) & vbCrLf & CStr(iBlockByParcelCount) & vbCrLf & CStr(mdicBlocks.Count), "03_512")
					End If

					oAllBlockCol.Dispose()
					oAllBlockCol = Nothing
				End If
			End If

		End Sub


		Public Shared Function LoadLotsOldVer(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Boolean
			Dim oLotTopology As TopologyModel = DMAcadExt.AcadMapApp.GetTopology(TopoDefs.GetTopoName(iTopoPurpose))

			If oLotTopology IsNot Nothing Then
				Dim oLot As TplnLot
				Dim dicLots As TPlanGraph.TplnLots

				Try
					If oLotTopology.Status = Status.Closed Then
						oLotTopology.Open(Autodesk.Gis.Map.Topology.OpenMode.ForWrite)
					End If
				Catch oMapEx As Autodesk.Gis.Map.MapException
					System.Windows.Forms.MessageBox.Show("Cannot open  topology '" & TPlanGraph.TplnLot.TopoName(iTopoPurpose) & "'", "TplnProject - LoadLotsOldVer")
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadLots_01")
					Return False
				End Try

				If oLotTopology.Status <> Status.Closed Then
					Dim colPolygons As PolygonCollection
					Try
						colPolygons = oLotTopology.GetPolygons()
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadLots_02")
						Return False
					End Try
					dicLots = New TPlanGraph.TplnLots
					TplnLot.CreateMainDataTable(iTopoPurpose)
					'    DMAcadExt.AcadDocument.WriteDebugMessage("01_030: " & CStr(colPolygons.Count))
					For Each oPolygon As Polygon In colPolygons
						oLot = New TplnLot(iTopoPurpose, oPolygon)
						If oLot.Correct Then
							dicLots.AddLot(oLot)
							TplnLot.AddLotToLandusesOldVer(oLot)
						End If
						oPolygon.Dispose()
						oPolygon = Nothing

						'oLot.Terminate()
						oLot = Nothing
					Next
					'	colPolygons.Dispose()
					colPolygons = Nothing
					oLotTopology.Close()
					oLotTopology = Nothing

					If iTopoPurpose = enTopoPurpose.Proposed Then
						mdicLotsProp = dicLots
						mdicLotsProp.PlanStatus = enTopoPurpose.Proposed
					ElseIf iTopoPurpose = enTopoPurpose.Approved Then
						mdicLotsAppr = dicLots
						mdicLotsAppr.PlanStatus = enTopoPurpose.Approved
					Else
						System.Windows.Forms.MessageBox.Show("Err #2893", "TplnProject - LoadLots")
					End If
					Return True
				Else
					System.Windows.Forms.MessageBox.Show("Lot(" & iTopoPurpose.ToString() & ") Topology Is Nothing", "TplnProject - LoadLots")
					Return False
				End If
			Else
				System.Windows.Forms.MessageBox.Show("Lot(" & iTopoPurpose.ToString() & ") Topology Is Nothing", "TplnProject - LoadLots_3")
				Return False
			End If
		End Function
		Public Shared Function LoadPlans() As Boolean
			Dim oPlanTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(TplnPlan.GetTopoName(), Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)

			'System.Windows.Forms.MessageBox.Show("LoadPlans" & vbCrLf & CStr(oPlanTopology IsNot Nothing), "#2758A")
			If oPlanTopology IsNot Nothing Then
				Dim oPlan As TplnPlan
				'    Dim dicLots As TPlanGraph.TplnLots

				If oPlanTopology.Status <> Status.Closed Then
					Dim colPolygons As PolygonCollection
					Try
						colPolygons = oPlanTopology.GetPolygons()
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadLots_02")
						Return False
					End Try
					mdicPlans = New TPlanGraph.TplnPlans(False, False)



					TplnPlan.CreateMainDataTable()
					DMAcadExt.AcadDocument.WriteDebugMessage("01_030: " & CStr(colPolygons.Count))

					For Each oPolygon As Polygon In colPolygons

						oPlan = New TplnPlan(oPolygon)

						'DMCommon.Debug.MsgBox("!New Plan", oPlan.TopoID, oPlan.Name)
						If oPlan.Correct Then

							mdicPlans.AddPlan(oPlan)



						End If
						oPolygon.Dispose()
						oPolygon = Nothing

						'oLot.Terminate()
						oPlan = Nothing
					Next

					'	colPolygons.Dispose()
					colPolygons = Nothing
					oPlanTopology.Close()
					oPlanTopology = Nothing

					'  System.Windows.Forms.MessageBox.Show(mdicPlans.Count.ToString(), "01_845")
					Return True
				Else
					System.Windows.Forms.MessageBox.Show("Topology '" & TplnPlan.GetTopoName() & "' was not found", "TplnProject - LoadPlans")
					Return False
				End If
			Else
				System.Windows.Forms.MessageBox.Show("Topology '" & TplnPlan.GetTopoName() & "' was not found", "TplnProject - LoadPlan_3n")
				Return False
			End If
		End Function

		'ccdicTopoPolygons = moPgonSet.TopoPolygons

		Public Shared Sub PlanToLots(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose)

			Dim oPlanTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(TplnPlan.GetTopoName(), Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			Dim oPlan As TplnPlan
			Dim dicLots As TPlanGraph.TplnLots
			Dim oPlanPgon As Polygon = Nothing
			If oPlanTopology IsNot Nothing Then

				dicLots = Lots(iTopoPurpose)
				'DMCommon.Debug.MsgBoxLoop("13_801C", iTopoPurpose, DMCommon.Debug.ColCount(dicLots), DMCommon.Debug.ColCount(mdicPlans))

				For Each oLot As TplnLot In dicLots.Values
					Try
						oPlanPgon = oPlanTopology.FindPolygon(oLot.CentroidPoint3d)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx, False, "PlanToLots", False, "CenterDisPgon=" & DMAcadExt.TPlnPoint.DispPoint(oLot.CentroidPoint3d))
						DMAcadExt.AcadDocument.WriteDebugMessage("^477 " & "CenterDisPgon=" & DMAcadExt.TPlnPoint.DispPoint(oLot.CentroidPoint3d))
						'	Return
					End Try

					If oPlanPgon IsNot Nothing Then
						oPlan = GetPlan(oPlanPgon.ID, "DDDD")
						If oPlan IsNot Nothing Then
							'DMCommon.Debug.MsgBox("13_801B", oPlanPgon.ID, oPlan.Name)
							oLot.UpdatePlan(oPlanPgon.ID, oPlan.Name)
							'	DMCommon.Debug.MsgBox("13_801G", dicLots.Count, oLot.BasicPlanID, oPlan.TopoID, oLot.PlanName)
						End If
					End If
				Next
				oPlanTopology.Close()
				If dicLots IsNot Nothing Then
					'	DMCommon.Debug.MsgBoxLoop("13_801EndOfPlanToLots", dicLots.Count)
				End If
			Else
				System.Windows.Forms.MessageBox.Show("Topology '" & TplnPlan.GetTopoName() & "' was not found", "TplnProject - UpdateLotsByPlans")
			End If
		End Sub
		Public Shared Function LoadLots(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, Optional bMust As Boolean = True) As Boolean
			Dim oRegion As TplnRegion = Nothing
			Dim oLotTopology As TopologyModel = DMAcadExt.AcadMapApp.GetTopology(TplnLot.GetTopoName(iTopoPurpose))

			If oLotTopology IsNot Nothing Then
				Dim iOverlayIndex As DMAcadExt.enOverlayIndex
				Dim oLot As TplnLot
				Dim dicLots As TPlanGraph.TplnLots
				If iTopoPurpose = enTopoPurpose.Approved Then
					iOverlayIndex = DMAcadExt.enOverlayIndex.ApprFDO_Overlay
				ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
					iOverlayIndex = DMAcadExt.enOverlayIndex.PropFDO_Overlay
				Else
					System.Windows.Forms.MessageBox.Show("", "LoadLotsNew_1723")
					Return False
				End If
				Try
					If oLotTopology.Status = Status.Closed Then
						oLotTopology.Open(Autodesk.Gis.Map.Topology.OpenMode.ForWrite)
					End If
				Catch oMapEx As Autodesk.Gis.Map.MapException
					System.Windows.Forms.MessageBox.Show("Cannot open  topology '" & TPlanGraph.TplnLot.GetTopoName(iTopoPurpose) & "'", "TplnProject - LoadLots")
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "01:LoadLotsTplnProject")
					Return False
				End Try


				If oLotTopology.Status <> Status.Closed Then
					Dim colPolygons As PolygonCollection
					Try
						colPolygons = oLotTopology.GetPolygons()
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, " 02:LoadLots_TplnProject")
						Return False
					End Try
					dicLots = New TPlanGraph.TplnLots
					TplnLot.CreateMainDataTable(iTopoPurpose)
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!loadLots", "!--------", "!--------", "!--------", "!--------")
					'       DMAcadExt.AcadDocument.WriteDebugMessage("01_030: " & CStr(colPolygons.Count))
					mcolLotForcedArea = New System.Collections.ObjectModel.Collection(Of BalanceArea.ConstArea)

					For Each oPolygon As Polygon In colPolygons
						oLot = New TplnLot(iTopoPurpose, oPolygon)
						' DMAcadExt.AcadDocument.WriteDebugMessage("^460 " & oLot.ForcedArea.ToString())
						'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!LoadLotsA", oLot.TopoID, oLot.Name, oLot.LanduseID, oLot.RegionNo, oLot.ForcedArea)
						If oLot.ForcedArea <> 0.0 Then
							mcolLotForcedArea.Add(New BalanceArea.ConstArea(oLot.TopoID, oLot.ForcedArea))
						End If
						'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!LotID", oLot.TopoID, oLot.Name, oLot.RegionNo, oLot.LanduseID)
						'	DMCommon.Debug.ExcelLog.SetNextValue(0, "oRegion.ByLot", oLot.RegionNo, oLot.Name)
						'      DMCommon.Debug.MsgBox("08_778", oLot.Region, mdicRegions Is Nothing)
						If oLot.RegionNo <> 0 Then

							'DMCommon.Debug.ExcelLog.SetNextValue(0, "!oLot.RegionNo", oLot.RegionNo, oLot.InPlan, mdicRegions.Count, iOverlayIndex)
							If mdicRegions IsNot Nothing AndAlso Not mdicRegions.TryGetValue(oLot.RegionNo, oRegion) Then
								oRegion = New TplnRegion(oLot.RegionNo)

								mdicRegions.Add(oLot.RegionNo, oRegion)



							End If
							If oRegion IsNot Nothing Then
								'DMCommon.Debug.ExcelLog.SetNextValue(0, "", oLot.TopoID, oLot.Name, oLot.RegionNo, oLot.LanduseID, oLot.AreaSet(iOverlayIndex).AcadArea, oLot.AreaSet(iOverlayIndex).CalcArea, oLot.AreaSet(iOverlayIndex).CalcArea2, oLot.AreaSet(iOverlayIndex).CalcGroupArea, oLot.AreaSet(iOverlayIndex).RoundedArea)
								oRegion.AddAreaset(oLot.AreaSet(iOverlayIndex))
								oRegion.AddLot(oLot)
								'DMCommon.Debug.ExcelLog.SetNextValue(0, "!oRegion.BB", oRegion.BoundingBox.Coordinates)
							End If

						End If

						If oLot.Correct Then
							dicLots.AddLot(oLot)
							'  DMAcadExt.AcadDocument.WriteDebugMessage("dicLots!Count=" & CStr(dicLots.Count) & "; mdicRegions!Count=" & CStr(mdicRegions.Count) & "; A=" & oLot.AcadArea(False))
							'DMCommon.ExcelLog.SetNextArray(oLot.AreaSet(DMAcadExt.enOverlayIndex.ApprMerge).ToArray, 14)
							'DMCommon.ExcelLog.SetNextArray(oLot.AreaSet(DMAcadExt.enOverlayIndex.PropMerge).ToArray, 14)
							'DMCommon.ExcelLog.SetNextArray(oLot.AreaSet(DMAcadExt.enOverlayIndex.ApprUnion).ToArray, 14)
							'DMCommon.ExcelLog.SetNextArray(oLot.AreaSet(DMAcadExt.enOverlayIndex.PropUnion).ToArray, 14)
							'DMCommon.ExcelLog.SetNextArray(oLot.AreaSet(DMAcadExt.enOverlayIndex.ApprFDO_Overlay).ToArray, 14)
							'DMCommon.ExcelLog.SetNextArray(oLot.AreaSet(DMAcadExt.enOverlayIndex.PropFDO_Overlay).ToArray, 14)

							TplnLot.AddLotToLanduses(oLot)
						Else
							DMAcadExt.AcadDocument.WriteMessage("$Err#27 Lot is not correct" & oLot.CenterPosition.Coordinates & " , Name " & oLot.Name & "; ")
						End If
						oPolygon.Dispose()
						oPolygon = Nothing

						'oLot.Terminate()
						oLot = Nothing
					Next
					dicLots.SetDoubleNameMsg()
					'	colPolygons.Dispose()
					colPolygons = Nothing
					oLotTopology.Close()
					oLotTopology = Nothing
					If iTopoPurpose = enTopoPurpose.Approved Then
						mdicLotsAppr = dicLots
						mdicLotsAppr.PlanStatus = enTopoPurpose.Approved
					ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
						mdicLotsProp = dicLots
						mdicLotsProp.PlanStatus = enTopoPurpose.Proposed
					Else
						System.Windows.Forms.MessageBox.Show("Err #2893", "TplnProject - LoadLots")
					End If

					Return True
				Else
					System.Windows.Forms.MessageBox.Show("Lot(" & iTopoPurpose.ToString() & ") Topology Is Nothing", "TplnProject - LoadLots")
					Return False
				End If
				'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!EndloadLots", "!++++++++++++", "!++++++++++++", "!++++++++++++", "!++++++++++++", "!++++++++++++")
			Else
				If oLotTopology IsNot Nothing Then
					oLotTopology.Close()
				End If

				Return False
			End If
		End Function

		Public Shared Function LoadLots_CP(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Boolean
			Dim oRegion As TplnRegion = Nothing
			'     Dim iRow As Integer
			'   Dim oLotTopology As TopologyModel = DMAcadExt.AcadMapApp.GetTopology(TplnLot.GetTopoName(iTopoPurpose))
			'    System.Windows.Forms.MessageBox.Show("LoadLotsNew" & vbCrLf & iTopoPurpose.ToString() & vbCrLf & CStr(TplnLot.GetTopoName(iTopoPurpose)) & vbCrLf & CStr(oLotTopology IsNot Nothing), "#2750 ")
			If TopoPolygons IsNot Nothing Then
				Dim iOverlayIndex As DMAcadExt.enOverlayIndex
				'  Dim oLot As TplnLot
				Dim dicLots As TPlanGraph.TplnLots = New TPlanGraph.TplnLots()

				If iTopoPurpose = enTopoPurpose.Approved Then
					iOverlayIndex = DMAcadExt.enOverlayIndex.ApprFDO_Overlay
				ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
					iOverlayIndex = DMAcadExt.enOverlayIndex.PropFDO_Overlay
				Else
					System.Windows.Forms.MessageBox.Show("", "LoadLotsNew_1723")
					Return False
				End If


				For Each oLot As TopoManager.TPlanGraph.TplnLot In TopoPolygons.Values
					'  DMCommon.ExcelLog.SetNextValue(iRow, 12, oLot.Correct, oLot.AcadArea(False))
					If oLot.Correct Then
						dicLots.AddLot(oLot)
						TplnLot.AddLotToLanduses(oLot)
					End If


				Next
				'  System.Windows.Forms.MessageBox.Show(dicLots.Values.Count.ToString(), "22_002")
				If iTopoPurpose = enTopoPurpose.Approved Then
					mdicLotsAppr = dicLots
					mdicLotsAppr.PlanStatus = enTopoPurpose.Approved
				ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
					mdicLotsProp = dicLots
					mdicLotsProp.PlanStatus = enTopoPurpose.Proposed
				Else
					System.Windows.Forms.MessageBox.Show("Err #2893", "TplnProject - LoadLots")
				End If


				dicLots.SetDoubleNameMsg()
				TplnLot.CreateMainDataTable(iTopoPurpose)
				Return True

				'	colPolygons.Dispose()
			Else
				Return False
			End If
		End Function

		Public Shared Sub Terminate()
			If Not moTransaction Is Nothing Then
				Try
					moTransaction.Commit()
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnProject - Terminate_1")
					Try
						moTransaction.Abort()
					Catch oExA As Exception
						System.Windows.Forms.MessageBox.Show(oExA.Message, "TplnProject - Terminate_2")
					End Try

				Finally
					moTransaction = Nothing
				End Try
			End If


		End Sub
		Public Shared Function GetBaseID(ByVal iTopoID As enTopoPurpose) As DMAcadExt.TopoDefID
			Dim stTopoDefID As DMAcadExt.TopoDefID
			Select Case iTopoID
				Case enTopoPurpose.Parcel, enTopoPurpose.Approved, enTopoPurpose.Proposed, enTopoPurpose.AdditionalA
					stTopoDefID.ID = CType(iTopoID, Integer)
					Return stTopoDefID
				Case Else
					stTopoDefID.ID = 0
					Return stTopoDefID
			End Select

		End Function
		Public Shared Function GetAdditionalID(ByVal iTopoID As DMAcadExt.enTopoPurpose) As DMAcadExt.TopoDefID
			Dim stTopoDefID As DMAcadExt.TopoDefID
			Select Case iTopoID
				Case DMAcadExt.enTopoPurpose.Parcel, DMAcadExt.enTopoPurpose.Approved, DMAcadExt.enTopoPurpose.Proposed
					stTopoDefID.ID = CType(iTopoID, Integer) + 64
					Return stTopoDefID
				Case Else
					stTopoDefID.ID = 0
					Return stTopoDefID
			End Select

		End Function
		Public Shared Function GetDissolveID(ByVal iTopoID As DMAcadExt.enTopoPurpose) As DMAcadExt.TopoDefID
			Dim stTopoDefID As DMAcadExt.TopoDefID
			Select Case iTopoID
				Case DMAcadExt.enTopoPurpose.Parcel, DMAcadExt.enTopoPurpose.Approved, DMAcadExt.enTopoPurpose.Proposed
					stTopoDefID.ID = CType(iTopoID, Integer) + 72
					Return stTopoDefID
				Case Else
					stTopoDefID.ID = 0
					Return stTopoDefID
			End Select

		End Function
		Public Shared Function GetUnionID_BBB(ByVal iSourceTopoID As enTopoPurpose, ByVal iOverlayTopoID As enTopoPurpose) As Integer
			If iSourceTopoID <> enTopoPurpose.Undefined And iOverlayTopoID <> enTopoPurpose.Undefined Then
				Return miTopoRange * miTopoRange * iSourceTopoID + iOverlayTopoID
			Else
				Return -1
			End If

		End Function
		Public Shared Function GetMergeID(ByVal iSourceTopoID As enTopoPurpose, ByVal iOverlayTopoID As enTopoPurpose) As Integer
			If iSourceTopoID <> enTopoPurpose.Undefined And iOverlayTopoID <> enTopoPurpose.Undefined Then
				Return miTopoRange * iSourceTopoID + iOverlayTopoID
			Else
				Return -1
			End If

		End Function
		Public Shared Function GetUnionID_B(ByVal iSourceTopoID As enTopoPurpose, ByVal iOverlayTopoID As enTopoPurpose) As Integer
			If iSourceTopoID <> enTopoPurpose.Undefined And iOverlayTopoID <> enTopoPurpose.Undefined Then
				Return miTopoRange * miTopoRange + miTopoRange * iSourceTopoID + iOverlayTopoID
			Else
				Return -1
			End If

		End Function

		Public Shared Function TopoIsUnion_B(ByVal iTopoID As Integer) As Boolean
			Return (iTopoID > miTopoRange * miTopoRange) And (iTopoID < 2 * miTopoRange * miTopoRange)
		End Function
		Public Shared Function TopoIsMergeAAA(ByVal iTopoID As Integer) As Boolean
			Return (iTopoID > miTopoRange) And (iTopoID < miTopoRange * miTopoRange)
		End Function

		Public Shared Function GetUnionLayerAAA(ByVal iTopoID As Integer) As String
			If iTopoID > miTopoRange Then
				Dim iSourceID, iOverlay As Integer
				iSourceID = Math.DivRem(iTopoID, miTopoRange, iOverlay)
				Return "Tpln" & zzGetTempLayerName(iSourceID) & zzGetTempLayerName(iOverlay)
			Else
				Return String.Empty
			End If
		End Function

		Public Shared Function GetBlock(ByVal iBlockNo As Integer, ByVal iBlockAddNo As Integer) As TplnBlock
			If mdicBlocks IsNot Nothing Then

				'MessageBox.Show(CStr(mdicBlocks.Count), "07_220")
				If mdicBlocks.ContainsKey(1000 * iBlockNo + iBlockAddNo) Then
					Return mdicBlocks.Item(1000 * iBlockNo + iBlockAddNo)
				Else
					Return Nothing
				End If
			Else
				Return Nothing
			End If
		End Function
		Public Shared Function GetBlockAttrib(ByVal tAcObjID As ObjectId) As AttributeCollection

			Dim colAttributes As AttributeCollection
			Dim oBlockRef As BlockReference = DMAcadExt.AcadTransaction.GetBlockRef(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
			If oBlockRef IsNot Nothing Then
				Try
					colAttributes = oBlockRef.AttributeCollection()
					Return colAttributes
				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "GetBlockAttrib")
					Return Nothing
				End Try
			Else
				Return Nothing
			End If


		End Function
		Public Shared Function GetAttribTextAAA(ByVal oAcObjID As ObjectId, ByVal iaAttribIndexes() As Integer) As String()
			Dim iValuesUB As Integer = -1

			Dim colAttributes As AttributeCollection = GetBlockAttrib(oAcObjID)

			If colAttributes IsNot Nothing Then

				Try
					iValuesUB = iaAttribIndexes.GetUpperBound(0)
				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "GetAttribText-1")
				End Try
				Dim oAttribObjID As ObjectId
				If iValuesUB >= 0 Then
					Dim saOutText(iValuesUB) As String
					Dim oDBObject As DBObject
					Dim oAttribRef As AttributeReference

					For iAttribIndex As Integer = 0 To iValuesUB
						Try
							oAttribObjID = colAttributes.Item(iaAttribIndexes(iAttribIndex))
							oDBObject = moTransaction.GetObject(oAttribObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)

							oAttribRef = DirectCast(oDBObject, AttributeReference)
							saOutText(iAttribIndex) = oAttribRef.TextString
						Catch oEx As System.Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message, "GetAttribText-2")
						End Try
					Next
					Terminate()
					Return saOutText
				Else
					System.Windows.Forms.MessageBox.Show("iValuesUB =-1", "GetAttribText-3")
					Return Nothing
				End If
			Else
				System.Windows.Forms.MessageBox.Show("Centroid Attributes were not found", "GetAttribText")
				Return Nothing

			End If
		End Function
		Public Shared ReadOnly Property Blocks As TPlanGraph.TplnBlocks
			Get
				Return mdicBlocks
			End Get
		End Property
		Public Shared Sub LoadLotsSrc(ByVal iStatus As DMAcadExt.enTopoPurpose)
			Dim oLotTopology As TopologyModel = DMAcadExt.AcadMapApp.GetTopology(TopoDefs.GetTopoName(iStatus))
			If oLotTopology IsNot Nothing Then
				Dim oLot As TplnLot
				Dim dicLots As TPlanGraph.TplnLots
				If oLotTopology.Status = Status.Closed Then
					Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock = Nothing
					Try
						oDocLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, "CommandLine", "", True)
						If oLotTopology.Status = Status.Closed Then
							oLotTopology.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
						End If
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadLots_01")
						If oDocLock IsNot Nothing Then
							oDocLock.Dispose()
							oDocLock = Nothing
						End If
						Exit Sub
					End Try

					If oLotTopology.Status = Status.OpenForRead Then
						Dim colPolygons As PolygonCollection
						Try
							colPolygons = oLotTopology.GetPolygons()
						Catch oMapEx As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadLots_2")
							If oDocLock IsNot Nothing Then
								oDocLock.Dispose()
								oDocLock = Nothing
							End If
							Exit Sub
						End Try

						dicLots = New TPlanGraph.TplnLots
						TplnLot.CreateMainDataTable(iStatus)
						For Each oPolygon As Polygon In colPolygons
							oLot = New TplnLot(iStatus, oPolygon)
							If oLot.Correct Then
								dicLots.AddLot(oLot)
								'oLot.AddDataToTable()
							End If
						Next
						oLotTopology.Close()
						oLotTopology = Nothing

						oDocLock.Dispose()
						oDocLock = Nothing
						If iStatus = enTopoPurpose.Proposed Then
							mdicLotsProp = dicLots
							mdicLotsProp.PlanStatus = enTopoPurpose.Proposed
						ElseIf iStatus = enTopoPurpose.Approved Then
							mdicLotsAppr = dicLots
							mdicLotsAppr.PlanStatus = enTopoPurpose.Approved
						Else
						End If
					End If
				End If
			Else
				System.Windows.Forms.MessageBox.Show("Lot(" & iStatus.ToString() & ") Topology Is Nothing")
			End If
		End Sub



		Public Shared Function GetBlockLegalArea() As System.Data.DataView

			Const sSumParcelLegalAreaFieldName As String = "SumParcelLegalArea"
			Const sDiffAreaFieldName As String = "DiffArea"

			Dim oBlock As TplnBlock

			Dim oBlockLegalAreaTable As System.Data.DataTable

			oBlockLegalAreaTable = New System.Data.DataTable("BlockLegalArea")

			oBlockLegalAreaTable.Columns.Add(TplnParcel.BlockFieldName, GetType(System.Int32))

			oBlockLegalAreaTable.Columns.Add(TplnParcel.LegalAreaFieldName, GetType(System.Double))
			oBlockLegalAreaTable.Columns.Add(sSumParcelLegalAreaFieldName, GetType(System.Double))
			oBlockLegalAreaTable.Columns.Add(sDiffAreaFieldName, GetType(System.Double))

			Dim oNewRow As System.Data.DataRow
			If mdicBlocks IsNot Nothing Then
				For Each oKeyValuePair As System.Collections.Generic.KeyValuePair(Of Integer, TplnBlock) In mdicBlocks
					oBlock = oKeyValuePair.Value
					oNewRow = oBlockLegalAreaTable.NewRow()
					With oNewRow
						.Item(TplnParcel.BlockFieldName) = oBlock.BlockNo
						.Item(TplnParcel.LegalAreaFieldName) = oBlock.LegalArea
						.Item(sSumParcelLegalAreaFieldName) = oBlock.SumParcelLegalArea
						.Item(sDiffAreaFieldName) = oBlock.DifLegalArea
					End With
					oBlockLegalAreaTable.Rows.Add(oNewRow)
				Next
			End If
			Dim oOutDataView As System.Data.DataView
			Dim sSort As String = TplnParcel.BlockFieldName & "," & TplnParcel.BlockAddFieldName
			oOutDataView = New System.Data.DataView(oBlockLegalAreaTable, String.Empty, sSort, DataViewRowState.CurrentRows)
			oOutDataView.AllowEdit = False
			oOutDataView.AllowDelete = False
			oOutDataView.AllowNew = False
			Return oOutDataView
		End Function
		Public Shared Sub LotContentAreaView(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex, ByVal bSortByLot As Boolean, ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer)

			Dim dicLots As TPlanGraph.TplnLots = Lots(UnionPgonArea.GetTopoPurpose(iOverlayIndex))
			'  mdicParcels

			Dim oLotContentAreaTable As System.Data.DataTable = New System.Data.DataTable("LotContentArea")

			With oLotContentAreaTable.Columns
				.Add(TplnParcel.msLotNameFieldName, GetType(System.String))
				.Add(TplnParcel.BlockFieldName, GetType(System.Int32))
				.Add(TplnParcel.NameFieldName, GetType(System.String))

				.Add(TplnParcel.LegalAreaFieldName, GetType(System.Double))
				.Add(TopoReader.msAreaFldName, GetType(System.Double))

				.Add(TplnParcel.msInLotAreaFieldName, GetType(System.Double))
				.Add(TplnParcel.msInLotCalcAreaFieldName, GetType(System.Double))
				.Add("FillerA", GetType(System.String))
				.Add(TplnParcel.msLotOrderFieldName, GetType(System.Int32))
				.Add(TplnParcel.ParcelOrderFieldName, GetType(System.Int32))
			End With

			Dim oUnionGroup As TplnUnionGroup
			Dim oNewRow As System.Data.DataRow
			Dim oLot As TplnLot
			Dim oParcel As TplnParcel


			Dim tUnionKey As UnionKey
			'      Dim itest As Integer = 0
			For Each oKeyValuePair As System.Collections.Generic.KeyValuePair(Of UnionKey, TplnUnionGroup) In mdicUnionGroupsAppr

				oUnionGroup = oKeyValuePair.Value
				tUnionKey = oUnionGroup.TUnionKey
				If (tUnionKey.LotTopoID <> 0) AndAlso (dicLots.ContainsKey(tUnionKey.LotTopoID)) Then
					oLot = dicLots.Item(tUnionKey.LotTopoID)
				Else
					If tUnionKey.LotTopoID <> 0 Then
						DMAcadExt.AcadDocument.WriteMessage("LotTopoID=" & CStr(tUnionKey.LotTopoID) & " was not found")
					End If
					oLot = Nothing
				End If
				If mdicParcels.ContainsKey(tUnionKey.ParcelTopoID) Then
					oParcel = mdicParcels.Item(tUnionKey.ParcelTopoID)
				Else
					DMAcadExt.AcadDocument.WriteMessage("ParcelTopoID=" & CStr(tUnionKey.ParcelTopoID) & " was not found")
					oParcel = Nothing
				End If
				If oLot IsNot Nothing AndAlso oLot.InPlan AndAlso oParcel IsNot Nothing Then
					'  If itest > 25 Then Exit For
					' itest += 1
					oNewRow = oLotContentAreaTable.NewRow()
					With oNewRow
						.Item(TplnParcel.msLotNameFieldName) = oLot.Name
						.Item(TplnParcel.BlockFieldName) = oParcel.BlockNo
						.Item(TplnParcel.NameFieldName) = oParcel.Name

						.Item(TplnParcel.LegalAreaFieldName) = oParcel.LegalOrAcadArea(True)


						.Item(TopoReader.msAreaFldName) = oParcel.AcadArea(True)
						.Item(TplnParcel.msInLotAreaFieldName) = oUnionGroup.AcadArea(True)
						.Item(TplnParcel.msInLotCalcAreaFieldName) = oUnionGroup.CalcArea(False)

						.Item(TplnParcel.msLotOrderFieldName) = oLot.Order
						.Item(TplnParcel.ParcelOrderFieldName) = oParcel.Order

					End With
					oLotContentAreaTable.Rows.Add(oNewRow)
				End If
			Next
			DMAcadExt.AcadDocument.CloseMessage()
			Dim oOutDataView As System.Data.DataView
			Dim sSort As String
			If bSortByLot Then
				sSort = TplnParcel.msLotOrderFieldName & "," & TplnParcel.BlockFieldName & "," & TplnParcel.ParcelOrderFieldName
			Else
				Dim iaAcadColumns() As Integer = {1, 2, 3, 4, 7, 0, 5}
				iaColumns = iaAcadColumns
				sSort = TplnParcel.BlockFieldName & "," & TplnParcel.ParcelOrderFieldName & "," & TplnParcel.msLotOrderFieldName
			End If

			oOutDataView = New System.Data.DataView(oLotContentAreaTable, String.Empty, sSort, DataViewRowState.CurrentRows)
			oOutDataView.AllowEdit = False
			oOutDataView.AllowDelete = False
			oOutDataView.AllowNew = False
			oDataView = oOutDataView

		End Sub
		' ByVal iDataOption As enDataOptions, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, iRegionNo As Integer, hsRegions As HashSet(Of Integer),

		Public Shared Sub LotContentAreaByParcelNew(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex, ByVal bEntirety As Boolean, ByVal iDataOptions As TPlanGraph.enDataOptions, iRegionNo As Integer, hsRegions As HashSet(Of Integer), ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object)
			'	Const iMaxLength As Integer = 16
			Dim iTopoPurpose As DMAcadExt.enTopoPurpose = UnionPgonArea.GetTopoPurpose(iOverlayIndex) ' DMAcadExt.enTopoPurpose.Approved
			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			'  mdicParcels

			Dim oLotContentAreaTable As System.Data.DataTable

			oLotContentAreaTable = New System.Data.DataTable("LotContentArea")
			With oLotContentAreaTable.Columns
				.Add(TplnParcel.msLotNameFieldName, GetType(System.String))
				.Add(TplnParcel.LanduseNameFieldName, GetType(System.String))
				.Add(TopoReader.msAreaFldName, GetType(System.Double))
				.Add(TplnParcel.BlockFieldName, GetType(System.Int32))
				.Add(TplnParcel.NameFieldName, GetType(System.Int32))
				.Add(TplnParcel.LegalAreaFieldName, GetType(System.Double))     '5
				.Add(TplnParcel.PgonAreaFieldName, GetType(System.Double))
				.Add(TplnParcel.msLotOrderFieldName, GetType(System.Int32))
				.Add(TopoReader.msTopoIDFldName, GetType(System.Int32))
			End With

			'	Dim oUnionGroup As TplnUnionGroup
			Dim oNewRow As System.Data.DataRow = Nothing
			Dim oLot As TplnLot
			Dim oParcel As TplnParcel
			Dim dSumArea As Double = 0.0
			Dim dSumLotArea As Double = 0.0
			Dim sTest As String = ""
			If mdicOverlayGroups Is Nothing Then
				sTest = "mdicOverlayGroups Is Nothing"
			Else
				sTest = CStr(mdicOverlayGroups.GetUpperBound(0))
			End If
			'	MessageBox.Show(sTest, "01_251")
			sTest = ""
			For i As Integer = 0 To mdicOverlayGroups.GetUpperBound(0)
				If mdicOverlayGroups(i) Is Nothing Then
					sTest &= "Nothing"
				Else
					sTest &= CStr(mdicOverlayGroups(i).Count)
				End If
				sTest &= vbCrLf
			Next
			sTest &= iOverlayIndex.ToString() & ":" & CStr(CInt(iOverlayIndex))
			'		MessageBox.Show(sTest, "01_252")
			Dim dicOverlayGroups As TPlanGraph.TplnOverlayGroups = mdicOverlayGroups(iOverlayIndex)

			'	Dim oOverlayGroup As TplnOverlayGroup
			Dim iLotID, iParcelID As Integer
			Dim sLotName As String = String.Empty
			Dim iCurrentLotID As Integer = 0
			Dim iCurrentLotIDArea As Integer = 0
			Dim iCurrentBlock As Integer = 0
			'	Dim oNumeration As TopoManager.NumerationPair.Numeration = Nothing
			Dim tAreaSet As TplnAreaSet

			'      Dim iTest As Integer = 0
			For Each oOverlayGroup As TplnOverlayGroup In dicOverlayGroups.Values
				If oOverlayGroup.SelectGroup(bEntirety, iRegionNo, hsRegions) Then

					iLotID = oOverlayGroup.LotID
					iParcelID = oOverlayGroup.ParcelID


					If (iLotID <> 0) AndAlso (dicLots.ContainsKey(iLotID)) Then
						oLot = dicLots.Item(iLotID)
					Else
						If iLotID <> 0 Then
							DMAcadExt.AcadDocument.WriteMessage("LotTopoID=" & CStr(iLotID) & " was not found")
						End If
						oLot = Nothing
					End If
					If mdicParcels.ContainsKey(iParcelID) Then
						oParcel = mdicParcels.Item(iParcelID)
					Else
						DMAcadExt.AcadDocument.WriteMessage("ParcelTopoID=" & CStr(iParcelID) & " was not found")
						oParcel = Nothing
					End If
					If oLot IsNot Nothing AndAlso oLot.InPlan AndAlso oParcel IsNot Nothing Then
						oNewRow = oLotContentAreaTable.NewRow()
						With oNewRow
							.Item(TplnParcel.msLotNameFieldName) = oLot.Name
							.Item(TplnParcel.LanduseNameFieldName) = TplnLot.GetLanduseNameNew2(iTopoPurpose, oLot.LanduseID)

							tAreaSet = oLot.InPlanAreaSet(iOverlayIndex)
							.Item(TopoReader.msAreaFldName) = tAreaSet.GetArea(iDataOptions)
							dSumLotArea += tAreaSet.GetArea(iDataOptions)

							.Item(TplnParcel.BlockFieldName) = oParcel.BlockNo

							.Item(TplnParcel.NameFieldName) = oParcel.ParcelNo

							.Item(TplnParcel.LegalAreaFieldName) = oParcel.LegalArea(False)

							.Item(TplnParcel.PgonAreaFieldName) = oOverlayGroup.AreaSet.GetArea(iDataOptions)
							dSumArea += oOverlayGroup.AreaSet.GetArea(iDataOptions)

							.Item(TplnParcel.msLotOrderFieldName) = oLot.Order
							'	.Item(TplnParcel.msParcelOrderFieldName) = oParcel.Order
							'	.Item(TplnParcel.msLanduseIDFieldName) = oLot.LanduseID
							.Item(TopoReader.msTopoIDFldName) = oLot.TopoID
							oLotContentAreaTable.Rows.Add(oNewRow)
						End With
					End If
				End If
			Next
			Dim sSort As String
			DMAcadExt.AcadDocument.CloseMessage()

			ReDim oaTotals(1)

			oaTotals(0) = dSumArea
			oaTotals(1) = dSumLotArea

			sSort = TplnParcel.msLotOrderFieldName & "," & TplnParcel.BlockFieldName & "," & TplnParcel.NameFieldName

			oDataView = New Data.DataView(oLotContentAreaTable, String.Empty, sSort, DataViewRowState.CurrentRows)

			oDataView.AllowEdit = False
			oDataView.AllowDelete = False
			oDataView.AllowNew = False

			'	DMCommon.Debug.ExcelLog.SetDataTable(0, "LotContent", oDataView)
		End Sub




		Public Shared Sub LotContentAreaByParcel(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex, ByVal iDataOptions As TPlanGraph.enDataOptions, ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object)
			'	Const iMaxLength As Integer = 16
			Dim iTopoPurpose As DMAcadExt.enTopoPurpose = UnionPgonArea.GetTopoPurpose(iOverlayIndex) ' DMAcadExt.enTopoPurpose.Approved
			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			'  mdicParcels

			Dim oLotContentAreaTable As System.Data.DataTable

			oLotContentAreaTable = New System.Data.DataTable("LotContentArea")
			With oLotContentAreaTable.Columns
				.Add(TplnParcel.msLotNameFieldName, GetType(System.String))
				.Add(TplnParcel.LanduseNameFieldName, GetType(System.String))
				.Add(TopoReader.msAreaFldName, GetType(System.Double))
				.Add(TplnParcel.BlockFieldName, GetType(System.Int32))
				.Add(TplnParcel.NameFieldName, GetType(System.Int32))
				.Add(TplnParcel.LegalAreaFieldName, GetType(System.Double))     '5
				.Add(TplnParcel.PgonAreaFieldName, GetType(System.Double))
				.Add(TplnParcel.msLotOrderFieldName, GetType(System.Int32))
				.Add(TopoReader.msTopoIDFldName, GetType(System.Int32))
			End With

			'	Dim oUnionGroup As TplnUnionGroup
			Dim oNewRow As System.Data.DataRow = Nothing
			Dim oLot As TplnLot
			Dim oParcel As TplnParcel
			Dim dSumArea As Double = 0.0
			Dim sTest As String = ""
			If mdicOverlayGroups Is Nothing Then
				sTest = "mdicOverlayGroups Is Nothing"
			Else
				sTest = CStr(mdicOverlayGroups.GetUpperBound(0))
			End If
			'	MessageBox.Show(sTest, "01_251")
			sTest = ""
			For i As Integer = 0 To mdicOverlayGroups.GetUpperBound(0)
				If mdicOverlayGroups(i) Is Nothing Then
					sTest &= "Nothing"
				Else
					sTest &= CStr(mdicOverlayGroups(i).Count)
				End If
				sTest &= vbCrLf
			Next
			sTest &= iOverlayIndex.ToString() & ":" & CStr(CInt(iOverlayIndex))
			'		MessageBox.Show(sTest, "01_252")
			Dim dicOverlayGroups As TPlanGraph.TplnOverlayGroups = mdicOverlayGroups(iOverlayIndex)

			'	Dim oOverlayGroup As TplnOverlayGroup
			Dim iLotID, iParcelID, iBlockID As Integer
			Dim sLotName As String = String.Empty
			Dim iCurrentLotID As Integer = 0
			Dim iCurrentLotIDArea As Integer = 0
			Dim iCurrentBlock As Integer = 0
			'	Dim oNumeration As TopoManager.NumerationPair.Numeration = Nothing
			Dim tAreaSet As TplnAreaSet

			'      Dim itest As Integer = 0
			For Each oOverlayGroup As TplnOverlayGroup In dicOverlayGroups.Values


				iLotID = oOverlayGroup.LotID
				iParcelID = oOverlayGroup.ParcelID


				If (iLotID <> 0) AndAlso (dicLots.ContainsKey(iLotID)) Then
					oLot = dicLots.Item(iLotID)
				Else
					If iLotID <> 0 Then
						DMAcadExt.AcadDocument.WriteMessage("LotTopoID=" & CStr(iLotID) & " was not found")
					End If
					oLot = Nothing
				End If
				If mdicParcels.ContainsKey(iParcelID) Then
					oParcel = mdicParcels.Item(iParcelID)
				Else
					DMAcadExt.AcadDocument.WriteMessage("ParcelTopoID=" & CStr(iParcelID) & " was not found")
					oParcel = Nothing
				End If
				If oLot IsNot Nothing AndAlso oLot.InPlan AndAlso oParcel IsNot Nothing Then
					oNewRow = oLotContentAreaTable.NewRow()
					With oNewRow
						.Item(TplnParcel.msLotNameFieldName) = oLot.Name
						.Item(TplnParcel.LanduseNameFieldName) = TplnLot.GetLanduseNameNew2(iTopoPurpose, oLot.LanduseID)
						tAreaSet = oLot.InPlanAreaSet(iOverlayIndex)

						.Item(TopoReader.msAreaFldName) = tAreaSet.GetArea(iDataOptions)
						'		dSumArea += oLot.AcadArea(False)

						.Item(TplnParcel.BlockFieldName) = oParcel.BlockNo

						.Item(TplnParcel.NameFieldName) = oParcel.ParcelNo

						.Item(TplnParcel.LegalAreaFieldName) = oParcel.LegalArea(False) * 1000.0

						.Item(TplnParcel.PgonAreaFieldName) = oOverlayGroup.AreaSet.GetArea(iDataOptions) * 1000.0


						.Item(TplnParcel.msLotOrderFieldName) = oLot.Order
						'	.Item(TplnParcel.msParcelOrderFieldName) = oParcel.Order
						'	.Item(TplnParcel.msLanduseIDFieldName) = oLot.LanduseID
						.Item(TopoReader.msTopoIDFldName) = oLot.TopoID
						oLotContentAreaTable.Rows.Add(oNewRow)
					End With
				End If
			Next
			Dim sSort As String
			DMAcadExt.AcadDocument.CloseMessage()

			ReDim oaTotals(1)

			oaTotals(0) = dSumArea  'dSumInPlanProp

			sSort = TplnParcel.msLotOrderFieldName & "," & TplnParcel.BlockFieldName & "," & TplnParcel.NameFieldName

			oDataView = New Data.DataView(oLotContentAreaTable, String.Empty, sSort, DataViewRowState.CurrentRows)

			oDataView.AllowEdit = False
			oDataView.AllowDelete = False
			oDataView.AllowNew = False

			'130126 DMCommon.Debug.ExcelLog.SetDataTable(0, "LotContent", oDataView)
		End Sub
		Public Shared Sub LotContentByBlockAreaView(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex, ByVal iDataOptions As TPlanGraph.enDataOptions, ByRef oDataView As Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object)
			Const iMaxLength As Integer = 16
			Dim iTopoPurpose As DMAcadExt.enTopoPurpose = UnionPgonArea.GetTopoPurpose(iOverlayIndex) ' DMAcadExt.enTopoPurpose.Approved
			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			'  mdicParcels

			Dim oLotContentAreaTable As System.Data.DataTable

			oLotContentAreaTable = New System.Data.DataTable("LotContentArea")
			With oLotContentAreaTable.Columns
				.Add(TplnParcel.msLotNameFieldName, GetType(System.String))
				.Add(TplnParcel.BlockFieldName, GetType(System.Int32))
				.Add(TplnParcel.NameFieldName, GetType(System.String))
				.Add(TplnParcel.LanduseNameFieldName, GetType(System.String))
				.Add(TopoReader.msAreaFldName, GetType(System.Double))

				.Add(TplnParcel.msLotOrderFieldName, GetType(System.Int32))
				.Add(TplnParcel.ParcelOrderFieldName, GetType(System.Int32))
				.Add(TplnParcel.LanduseIDFieldName, GetType(System.Int32))
				.Add(TopoReader.msTopoIDFldName, GetType(System.Int32))


			End With

			'	Dim oUnionGroup As TplnUnionGroup
			Dim oNewRow As System.Data.DataRow = Nothing
			Dim oLot As TplnLot
			Dim oParcel As TplnParcel
			Dim dSumArea As Double = 0.0
			Dim sTest As String = ""
			If mdicOverlayGroups Is Nothing Then
				sTest = "mdicOverlayGroups Is Nothing"
			Else
				sTest = CStr(mdicOverlayGroups.GetUpperBound(0))
			End If
			'	MessageBox.Show(sTest, "01_251")
			sTest = ""
			For i As Integer = 0 To mdicOverlayGroups.GetUpperBound(0)
				If mdicOverlayGroups(i) Is Nothing Then
					sTest &= "Nothing"
				Else
					sTest &= CStr(mdicOverlayGroups(i).Count)
				End If
				sTest &= vbCrLf
			Next
			sTest &= iOverlayIndex.ToString() & ":" & CStr(CInt(iOverlayIndex))
			'		MessageBox.Show(sTest, "01_252")
			Dim dicOverlayGroups As TPlanGraph.TplnOverlayGroups = mdicOverlayGroups(iOverlayIndex)

			Dim oOverlayGroup As TplnOverlayGroup
			Dim iLotID, iParcelID, iBlockID As Integer
			Dim sLotName As String = String.Empty
			Dim iCurrentLotID As Integer = 0
			Dim iCurrentLotIDArea As Integer = 0
			Dim iCurrentBlock As Integer = 0
			Dim oNumeration As TopoManager.NumerationPair.Numeration = Nothing
			Dim tAreaSet As TplnAreaSet

			'      Dim itest As Integer = 0
			For Each oKeyValuePair As System.Collections.Generic.KeyValuePair(Of ULong, TplnOverlayGroup) In dicOverlayGroups

				oOverlayGroup = oKeyValuePair.Value
				iLotID = oOverlayGroup.LotID
				iParcelID = oOverlayGroup.ParcelID


				If (iLotID <> 0) AndAlso (dicLots.ContainsKey(iLotID)) Then
					oLot = dicLots.Item(iLotID)
				Else
					If iLotID <> 0 Then
						DMAcadExt.AcadDocument.WriteMessage("LotTopoID=" & CStr(iLotID) & " was not found")
					End If
					oLot = Nothing
				End If
				If mdicParcels.ContainsKey(iParcelID) Then
					oParcel = mdicParcels.Item(iParcelID)
				Else
					DMAcadExt.AcadDocument.WriteMessage("ParcelTopoID=" & CStr(iParcelID) & " was not found")
					oParcel = Nothing
				End If
				If oLot IsNot Nothing AndAlso oLot.InPlan AndAlso oParcel IsNot Nothing Then
					oNewRow = oLotContentAreaTable.NewRow()
					With oNewRow
						.Item(TplnParcel.msLotNameFieldName) = oLot.Name
						.Item(TplnParcel.BlockFieldName) = oParcel.BlockNo

						.Item(TplnParcel.NameFieldName) = oParcel.Name
						.Item(TplnParcel.LanduseNameFieldName) = TplnLot.GetLanduseNameNew2(iTopoPurpose, oLot.LanduseID)

						tAreaSet = oLot.InPlanAreaSet(iOverlayIndex)
						.Item(TopoReader.msAreaFldName) = tAreaSet.GetArea(iDataOptions)
						'		dSumArea += oLot.AcadArea(False)

						.Item(TplnParcel.msLotOrderFieldName) = oLot.Order
						.Item(TplnParcel.ParcelOrderFieldName) = oParcel.Order
						.Item(TplnParcel.LanduseIDFieldName) = oLot.LanduseID
						.Item(TopoReader.msTopoIDFldName) = oLot.TopoID
						oLotContentAreaTable.Rows.Add(oNewRow)
					End With
				End If
			Next

			Dim oOutDataView As System.Data.DataView
			Dim sSort As String
			sSort = TopoReader.msTopoIDFldName & "," & TplnParcel.BlockFieldName & "," & TplnParcel.ParcelOrderFieldName & " DESC"
			'''''''''''''''''''''DMCommon.Debug.ExcelLog.SetDataTable(0, "ExproTable", TplnParcel.ExproTable)
			oOutDataView = New System.Data.DataView(oLotContentAreaTable, String.Empty, sSort, DataViewRowState.CurrentRows)
			oOutDataView.AllowEdit = True
			oOutDataView.AllowDelete = True
			oOutDataView.AllowNew = False
			Dim oDataRowView As DataRowView = Nothing
			Dim oOKDataRowView As DataRowView = Nothing
			Dim sParcelName As String

			iLotID = 0
			iBlockID = 0

			NumerationPair.SetGroupDelim(NumerationPair.enTextDirection.RightToLeft)

			For iIndex As Integer = oOutDataView.Count - 1 To 0 Step -1
				oDataRowView = oOutDataView.Item(iIndex)
				iLotID = DirectCast(oDataRowView.Item(TopoReader.msTopoIDFldName), Integer)
				iBlockID = DirectCast(oDataRowView.Item(TplnParcel.BlockFieldName), Integer)
				sParcelName = DirectCast(oDataRowView.Item(TplnParcel.NameFieldName), String)


				If iLotID <> iCurrentLotID OrElse iBlockID <> iCurrentBlock Then
					If iLotID <> iCurrentLotIDArea Then
						dSumArea += DirectCast(oDataRowView.Item(TopoReader.msAreaFldName), Double)
						iCurrentLotIDArea = iLotID
					End If

					iCurrentLotID = iLotID
					iCurrentBlock = iBlockID
					If (oOKDataRowView IsNot Nothing) AndAlso (oNumeration IsNot Nothing) Then

						oOKDataRowView.Item(TplnParcel.NameFieldName) = oNumeration.GetPresentationByRows(iMaxLength)
						oNumeration = Nothing
					End If
					oNumeration = New TopoManager.NumerationPair.Numeration(TopoManager.NumerationPair.enComplexType.Undefined, NumerationPair.enTextDirection.RightToLeft)
					oNumeration.Add(sParcelName)

					oOKDataRowView = oDataRowView
				Else
					If oNumeration IsNot Nothing Then
						oNumeration.Add(sParcelName)
					End If
					oDataRowView.Delete()
				End If
			Next

			If oOKDataRowView IsNot Nothing Then
				If oNumeration IsNot Nothing Then
					oOKDataRowView.Item(TplnParcel.NameFieldName) = oNumeration.GetPresentationByRows(iMaxLength)
					'	dSumArea += DirectCast(oDataRowView.Item(TopoReader.msAreaFldName), Double)
				End If

			End If
			DMAcadExt.AcadDocument.CloseMessage()

			ReDim oaTotals(1)

			oaTotals(0) = dSumArea  'dSumInPlanProp

			sSort = TplnParcel.msLotOrderFieldName & "," & TplnParcel.BlockFieldName

			oDataView = New System.Data.DataView(oLotContentAreaTable, String.Empty, sSort, DataViewRowState.CurrentRows)

			oDataView.AllowEdit = False
			oDataView.AllowDelete = False
			oDataView.AllowNew = False


		End Sub
		Public Shared Sub CalculateParcels(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean, ByVal bExpro As Boolean)
			Dim oParcel As TplnParcel
			Dim iOverlayMethod As DMAcadExt.enOverlayMethod
			Dim iTopoPurpose As DMAcadExt.enTopoPurpose
			Dim bRegionsExist As Boolean
			If mdicRegions IsNot Nothing AndAlso mdicRegions.Count > 1 Then
				bRegionsExist = True
			Else
				bRegionsExist = False
			End If
			'	DMCommon.Debug.MsgBox("13_207", bMerge, bUnion, bFDO_Overlay, bApproved, bProposed, bExpro, DMCommon.Debug.ColCount(mdicParcels), DMCommon.Debug.ColCount(mdicRegions))

			If mdicParcels IsNot Nothing Then
				'	DMCommon.Debug.MsgBox("CalculateParcels_13", mdicParcels.Count, bFDO_Overlay, bApproved, bProposed, bExpro)
				If bMerge Then
					iOverlayMethod = DMAcadExt.enOverlayMethod.Merge
				ElseIf bUnion Then
					iOverlayMethod = DMAcadExt.enOverlayMethod.Union
				ElseIf bFDO_Overlay Then
					iOverlayMethod = DMAcadExt.enOverlayMethod.FDO_Overlay
				Else
					Return
				End If
				If bExpro Then
					iTopoPurpose = DMAcadExt.enTopoPurpose.Expro
				ElseIf bApproved Then
					iTopoPurpose = DMAcadExt.enTopoPurpose.Approved
				ElseIf bProposed Then
					iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed
				Else
					iTopoPurpose = DMAcadExt.enTopoPurpose.Undefined
				End If

				For Each oParcel In mdicParcels.Values
					mdicBlocks.AddParcel(oParcel)
					'	DMCommon.Debug.ExcelLog.SetNextValue(0, "Parcel.Calc", bExpro, oParcel.BlockNo, oParcel.Name, iOverlayMethod, iTopoPurpose)
					oParcel.CalculateArea(iOverlayMethod, iTopoPurpose, bRegionsExist)


					If False Then
						If bApproved Then
							If bMerge Then
								oParcel.CalculateArea(DMAcadExt.enOverlayMethod.Merge, DMAcadExt.enTopoPurpose.Approved, bRegionsExist)
							End If
							If bUnion Then
								oParcel.CalculateArea(DMAcadExt.enOverlayMethod.Union, DMAcadExt.enTopoPurpose.Approved, bRegionsExist)
							End If
							If bFDO_Overlay Then

								oParcel.CalculateArea(DMAcadExt.enOverlayMethod.FDO_Overlay, DMAcadExt.enTopoPurpose.Approved, bRegionsExist)

							End If
						End If
						If bProposed Then
							If bMerge Then
								oParcel.CalculateArea(DMAcadExt.enOverlayMethod.Merge, DMAcadExt.enTopoPurpose.Proposed, bRegionsExist)
							End If
							If bUnion Then
								oParcel.CalculateArea(DMAcadExt.enOverlayMethod.Union, DMAcadExt.enTopoPurpose.Proposed, bRegionsExist)
							End If
							If bFDO_Overlay Then
								oParcel.CalculateArea(DMAcadExt.enOverlayMethod.FDO_Overlay, DMAcadExt.enTopoPurpose.Proposed, bRegionsExist)
							End If
						End If
						If bExpro Then
							oParcel.CalculateArea(DMAcadExt.enOverlayMethod.FDO_Overlay, DMAcadExt.enTopoPurpose.Expro, False)
						End If
					End If 'false
				Next
			Else
				MessageBox.Show("mdicParcels Is Nothing", "01_279")
				''  Topology isn't opened
			End If
			'   DMAcadExt.AcadDocument.WriteMessage("%$:+" & CStr(mdicParcels.Count))
			'	DMCommon.Debug.MsgBox("13_207c", "CalculateParcels", bMerge, bUnion, bFDO_Overlay, bApproved, bProposed, bExpro)

		End Sub
		Public Shared Sub UpdateParcelTable(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean, ByVal bExpro As Boolean)
			Dim oParcel As TplnParcel

			TplnParcel.CreateMainDataTable(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
			'	TplnParcel.CreateLanduseTables(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)

			If bApproved Then
				TplnParcel.CreateLanduseTable(DMAcadExt.enTopoPurpose.Approved, False)
			Else
				TplnParcel.DisposeLanduseTable(DMAcadExt.enTopoPurpose.Approved, False)
			End If

			If bProposed Then
				TplnParcel.CreateLanduseTable(DMAcadExt.enTopoPurpose.Proposed, False)
			Else
				TplnParcel.DisposeLanduseTable(DMAcadExt.enTopoPurpose.Proposed, False)
			End If

			If bExpro Then
				TplnParcel.CreateExproTables()
			End If

			'     
			If mdicParcels IsNot Nothing Then
				TplnParcel.CreatePolygonTables(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
				'  MessageBox.Show("mdicParcels IsNot Nothing", "05_501")
				For Each oParcel In mdicParcels.Values
					oParcel.Calculate2(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
					oParcel.AddDataToMainTable(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed) '**************************************************** _429
					oParcel.AddDataToPolygonTables(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
					'	DMAcadExt.AcadDocument.WriteMessage("###266-- " & oParcel.BlockFull & "," & oParcel.Block)

					If bApproved Then
						oParcel.AddDataToLanduseTable(bMerge, bUnion, bFDO_Overlay, DMAcadExt.enTopoPurpose.Approved)
					End If
					If bProposed Then '
						oParcel.AddDataToLanduseTable(bMerge, bUnion, bFDO_Overlay, DMAcadExt.enTopoPurpose.Proposed)
					End If
					If bExpro Then
						oParcel.AddDataToExproTable()

						oParcel.AddDataToExproTableByCol()

						'''''''''''''''''''''''''''''''''	oParcel.AddDataToExproTablePgonsNew()
					End If
				Next
				If TplnParcel.ExproTable IsNot Nothing Then
					''''''''''''''''	DMCommon.Debug.ExcelLog.SetDataTable(0, "ExproTable", TplnParcel.ExproTable)

				End If
				If TplnParcel.ExproTablePgons IsNot Nothing Then
					'	DMCommon.Debug.ExcelLog.SetDataTable(0, "ExproTablePgons", TplnParcel.ExproTablePgons)
				End If
				'DMCommon.Debug.MsgBox("05_700K", TplnParcel.ExproTable.Rows.Count)
				'DMCommon.Debug.MsgBox("05_700L", TplnParcel.ExproTableByCol.Rows.Count, TplnParcel.ExproTableByCol.Columns.Count)
			Else
				MessageBox.Show("mdicParcels Is Nothing", "01_234")
				''  Topology isn't opened
			End If
			'	DMCommon.Debug.MsgBox("13_132c", "UpdateParcelTable!!!", bExpro, mdicParcels.Count, TplnParcel.MainDataTable.Rows.Count, iTestExproCount)

		End Sub
		Public Shared Sub GetOverlayData(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex, ByVal iOptions As enDataOptions, ByVal iOverlayMethod As DMAcadExt.enOverlayMethod, iRegion As Integer, ByRef oDataView As System.Data.DataView, ByRef iaColumns() As Integer, ByRef oaTotals() As System.Object)
			Const sCalcColName As String = "ParcelAreaCalculated"
			Dim iCalcColName As Integer
			Dim oTable As System.Data.DataTable = moOverlayTables(iOverlayIndex)
			' Dim sAreaFldName As String
			Dim iTopoPurpose As DMAcadExt.enTopoPurpose = UnionPgonArea.GetTopoPurpose(iOverlayIndex) ' DMAcadExt.enTopoPurpose.Approved
			'	MessageBox.Show(iTopoPurpose.ToString() & ":" & CStr(moLanduseApprTable Is Nothing) & ":" & CStr(moLandusePropTable Is Nothing) & vbCrLf & CStr(oLanduseTable Is Nothing), "05_120")
			Dim sSort As String = TplnParcel.BlockFieldName & "," & TplnParcel.ParcelOrderFieldName & "," & TplnParcel.LanduseOrderFieldName & "," & TplnParcel.msLotOrderFieldName

			Dim sFilter As String
			If oTable IsNot Nothing Then
				'	MessageBox.Show(iTopoPurpose.ToString() & ":" & CStr(oLanduseTable.Rows.Count), "05_121")
			End If
			If oTable IsNot Nothing AndAlso oTable.Rows.Count <> 0 Then
				'	MessageBox.Show(iTopoPurpose.ToString() & ":" & CStr(moLandusePropTable Is Nothing) & ":" & CStr(moLanduseApprTable Is Nothing) & vbCrLf & CStr(oLanduseTable.Rows.Count), "05_122")

				If Not oTable.Columns.Contains(sCalcColName) Then
					oTable.Columns.Add(sCalcColName, GetType(System.Double))
				End If

				iCalcColName = oTable.Columns.Count - 1
				Dim iAreaColIndex As Integer

				'	Dim iLegalAreaColIndex As Integer = 4

				Select Case iOptions
					Case enDataOptions.AcadArea
						iAreaColIndex = 6


					Case enDataOptions.CalcMergeArea, enDataOptions.Default
						iAreaColIndex = 7


					Case enDataOptions.CalcMergeArea2
						If iRegion = 0 Then
							iAreaColIndex = 8
						Else
							iAreaColIndex = 10
						End If

					Case enDataOptions.RoundedArea
						iAreaColIndex = 9
				End Select
				Dim iaAcadColumns() As Integer = {0, 1, 5, iCalcColName, 2, 4, iAreaColIndex} '{0, 1, 5, iCalcColName, 2, 3, iAreaColIndex}	 '
				iaColumns = iaAcadColumns
				If iRegion = 0 Then
					sFilter = TplnParcel.msInPlanFieldName & " = True"
				Else
					sFilter = TplnLot.msRegionFieldName & " = " & Convert.ToString(iRegion)
				End If


				oDataView = New System.Data.DataView(oTable, sFilter, sSort, DataViewRowState.CurrentRows)
				oDataView.AllowEdit = True
				oDataView.AllowDelete = False
				oDataView.AllowNew = False
				Dim dSumParcel As Double = 0.0
				Dim dSum0 As Double = 0.0
				Dim dSum1 As Double = 0.0
				Dim dSum2 As Double = 0.0
				Dim iPrevRowIndex As Integer = 0
				Dim oPrevDataRowView As DataRowView = Nothing
				Dim oDataRowView As DataRowView = Nothing

				Dim iPrevBlock As Integer = 0
				Dim iPrevBlockAdd As Integer = 0

				Dim iPrevParcelOrder As Integer = 0

				Dim iCurrentBlock As Integer
				Dim iCurrentBlockAdd As Integer

				Dim iCurrentParcelOrder As Integer
				Dim iLanduseID As Integer

				'  DMCommon.ExcelLog.SetDataTable(oDataView, 0)
				For iRowIndex As Integer = 0 To oDataView.Count - 1

					oDataRowView = oDataView.Item(iRowIndex)
					iLanduseID = DMCommon.Functions.CIntN(oDataRowView.Item(TplnParcel.LanduseIDFieldName))
					'''''''''''	oDataRowView.Item(TplnParcel.msLanduseNameFieldName) = TplnLot.GetLanduseNameNew(iTopoPurpose, iLanduseID)
					dSum0 += DMCommon.Functions.CDblN(oDataRowView.Item(iAreaColIndex))

					iCurrentBlock = DirectCast(oDataRowView.Item(TplnParcel.BlockFieldName), Integer)
					iCurrentBlockAdd = DirectCast(oDataRowView.Item(TplnParcel.BlockAddFieldName), Integer)

					iCurrentParcelOrder = DirectCast(oDataRowView.Item(TplnParcel.ParcelOrderFieldName), Integer)

					If iPrevBlock <> iCurrentBlock OrElse iPrevBlockAdd <> iCurrentBlockAdd OrElse iPrevParcelOrder <> iCurrentParcelOrder Then
						For iPrevIndex As Integer = iPrevRowIndex To iRowIndex - 1
							oPrevDataRowView = oDataView.Item(iPrevIndex)
							oPrevDataRowView.Item(sCalcColName) = dSumParcel
						Next
						dSum1 += dSumParcel

						oPrevDataRowView = oDataRowView
						iPrevRowIndex = iRowIndex
						dSum2 += DMCommon.Functions.CDblN(oDataRowView.Item(TplnParcel.LegalAreaFieldName))


						iPrevBlock = iCurrentBlock
						iPrevBlockAdd = iCurrentBlockAdd
						iPrevParcelOrder = iCurrentParcelOrder
						dSumParcel = DMCommon.Functions.CDblN(oDataRowView.Item(iAreaColIndex))
					Else
						dSumParcel += DMCommon.Functions.CDblN(oDataRowView.Item(iAreaColIndex))
					End If
				Next

				For iPrevIndex As Integer = iPrevRowIndex To oDataView.Count - 1
					oPrevDataRowView = oDataView.Item(iPrevIndex)
					oPrevDataRowView.Item(sCalcColName) = dSumParcel
				Next
				dSum1 += dSumParcel

				ReDim oaTotals(2)
				oaTotals(0) = dSum0
				oaTotals(1) = dSum1
				oaTotals(2) = dSum2

			End If
		End Sub
		Public Shared ReadOnly Property OverlayGroupView(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As System.Data.DataView
			Get
				Dim s As String = ""
				For iIndex As Integer = 0 To moOverlayTables.GetUpperBound(0)
					s &= CStr(moOverlayTables(iIndex) Is Nothing) & vbCrLf
				Next
				DMCommon.Debug.MsgBox("05_335", s)
				If moOverlayTables(iOverlayIndex) IsNot Nothing Then
					Dim sSort As String = TplnParcel.BlockFieldName & "," & TplnParcel.ParcelOrderFieldName & "," & TplnParcel.msLotOrderFieldName
					'	MessageBox.Show(iOverlayIndex.ToString() & vbCrLf & CStr(moOverlayTables(iOverlayIndex) Is Nothing), "05_340")
					Dim oDataView As System.Data.DataView = New System.Data.DataView(moOverlayTables(iOverlayIndex), String.Empty, sSort, DataViewRowState.CurrentRows)
					oDataView.AllowEdit = False
					oDataView.AllowDelete = False
					oDataView.AllowNew = False
					Return oDataView
				Else
					MessageBox.Show(iOverlayIndex.ToString(), "TplnProject- PolygonView")
					Return Nothing
				End If
			End Get
		End Property
		'	050-2066983 IZHAR
		Public Shared Sub UpdateOverlayTable(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex)
			If iOverlayIndex <> DMAcadExt.enOverlayIndex.Undefined Then
				zzFillOverlayGroupTable(iOverlayIndex)
			End If
		End Sub
		Public Shared Sub UpdateLotTable(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose)

			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			If dicLots IsNot Nothing Then
				' MessageBox.Show(iTopoPurpose.ToString() & vbCrLf & CStr(bMerge) & ":" & CStr(bUnion) & ":" & CStr(bFDO_Overlay), "03_333")
				TplnLot.ClearDataTable(iTopoPurpose)

				For Each oLot As TplnLot In dicLots.Values
					''''''	oLot.TestAreaSet(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)

					oLot.Calculate2(bMerge, bUnion, bFDO_Overlay)
					oLot.AddDataToMainTable(bMerge, bUnion, bFDO_Overlay, False, False)
					''''		oLot.TestAreaSet(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
				Next
			Else
				'' Topology isn't opened
				''  System.Windows.Forms.MessageBox.Show("Lot " & iStatus.ToString() & " Dictionary is Nothing", "UpdateLotTable")
			End If
		End Sub
		Public Shared Sub LoadLusePgonsNew(tMapThemeData As DMAcadExt.MapThemeData)
			Dim iProjectCode As Integer = TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode
			Dim iDetailNo As Integer = TPlServerDB.ServerDB.CurrentProjectDB.DetailNo
			Dim iTopoPurpose As DMAcadExt.enTopoPurpose = tMapThemeData.TopoPurpose


			TPlanGraph.TplnLusePgon.CreateLusePgonDataTable(iTopoPurpose)

			Dim sLotTopoName As String = TplnLot.GetTopoName(iTopoPurpose) ' TopoDefs.GetTopoName(iTopoPurpose)
			Dim oLotTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sLotTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			If oLotTopology IsNot Nothing Then
				Dim oLusePgonTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(TplnLot.GetDissolveTopoName(iTopoPurpose), Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
				Dim oLotPgon As Polygon = Nothing
				Dim oLot As TplnLot
				Dim oLusePolygon As TplnLusePgon
				Dim iLanduseID As Integer
				Dim tLanduseData As LanduseData = Nothing
				Dim dicLusePgons As IDictionary(Of Integer, TplnLusePgon)

				If oLusePgonTopology IsNot Nothing Then

					Dim dicLots As TPlanGraph.TplnLots = TplnProject.Lots(iTopoPurpose)
					If dicLots IsNot Nothing Then
						Dim colPolygons As PolygonCollection = oLusePgonTopology.GetPolygons()
						Dim oLanduse As TplnLanduse = Nothing
						'''' 	Dim tColorScheme As DMAcadExt.ColorScheme

						Dim tPgonCenterPoint As Autodesk.AutoCAD.Geometry.Point3d
						Dim dicLanduseData As IDictionary(Of Integer, LanduseData) = TplnProject.GetPrjLuseSet(iProjectCode, iDetailNo, tMapThemeData.MapThemeID)
						dicLusePgons = zzGetLusePgons(iTopoPurpose, True)
						'		MessageBox.Show(CStr(colPolygons.Count), "04_240")
						For Each oPolygon As Polygon In colPolygons
							Try
								tPgonCenterPoint = oPolygon.Centroid
								oLusePolygon = New TplnLusePgon(iTopoPurpose, oPolygon)
								oPolygon.Dispose()
								oPolygon = Nothing
							Catch oMapEx As Autodesk.Gis.Map.MapException
								DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "LusePgons", False, "CenterDisPgon=" & DMAcadExt.TPlnPoint.DispPoint(tPgonCenterPoint))
								Return
							End Try

							Try
								oLotPgon = oLotTopology.FindPolygon(tPgonCenterPoint)
							Catch oMapEx As Autodesk.Gis.Map.MapException
								DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "LoadLusePgons-LotPgon", False, "CenterDisPgon=" & DMAcadExt.TPlnPoint.DispPoint(tPgonCenterPoint))
								'	Return
							End Try

							If oLotPgon IsNot Nothing AndAlso dicLots.ContainsKey(oLotPgon.ID) Then ''''Lena xotela 
								oLot = dicLots.Item(oLotPgon.ID)
								iLanduseID = oLot.LanduseID

								If iLanduseID <> 0 Then
									oLusePolygon.LanduseID = iLanduseID
									oLusePolygon.LanduseName = oLot.LanduseName
									If dicLanduseData.TryGetValue(iLanduseID, tLanduseData) Then
										TPlanGraph.TplnLusePgon.AddDataToLusePgonTable(iTopoPurpose, oLusePolygon, iLanduseID, tLanduseData.LuseColorScheme.Name)
									Else
										TPlanGraph.TplnLusePgon.AddDataToLusePgonTable(iTopoPurpose, oLusePolygon, 0, String.Empty)
									End If
									'	tColorScheme = TplnLot.GetColorScheme(iTopoPurpose, iLanduseID, 1.0)
									'	If tColorScheme.IsInstance Then

									'tPlanGraph.TplnLusePgon.AddDataToLusePgonTable(iTopoPurpose, oLusePolygon, iLanduseID, tColorScheme.Name)
									'	Else
									'	TPlanGraph.TplnLusePgon.AddDataToLusePgonTable(iTopoPurpose, oLusePolygon, 0, String.Empty)
									'DMAcadExt.AcadDocument.WriteDebugMessage("$$$$6 " & CStr(oLotPgon.ID))
									'	End If
								Else
									TPlanGraph.TplnLusePgon.AddDataToLusePgonTable(iTopoPurpose, oLusePolygon, 0, String.Empty)
									'DMAcadExt.AcadDocument.WriteMessage("####345-- " & "iLanduseID = 0")
								End If
								dicLusePgons.Add(oLusePolygon.TopoID, oLusePolygon)
							Else
								DMAcadExt.AcadDocument.WriteMessage("####77-- " & CStr(oLotPgon.ID) & " - was not found")
							End If
							If oPolygon IsNot Nothing Then
								oPolygon.Dispose()
								oPolygon = Nothing
							End If
						Next
					Else
						MessageBox.Show(CStr("Lot Collect was not fount"), "04_611a")
					End If
					oLusePgonTopology.Close()
				End If
				oLotTopology.Close()
			End If
		End Sub
		Public Shared Sub LoadLusePgons(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose)
			TPlanGraph.TplnLusePgon.CreateLusePgonDataTable(iTopoPurpose)
			Dim tTopoDefID As DMAcadExt.TopoDefID = GetDissolveID(iTopoPurpose)
			Dim oTopoDef As DMAcadExt.TopoDef = TopoDefs.Item(tTopoDefID)
			Dim sLotTopoName As String = TopoDefs.GetTopoName(iTopoPurpose)
			Dim oLotTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sLotTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			If oLotTopology IsNot Nothing Then
				Dim oLusePgonTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(oTopoDef.Name, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
				Dim oLotPgon As Polygon = Nothing
				Dim oLot As TplnLot
				Dim oLusePolygon As TplnLusePgon
				Dim iLanduseID As Integer
				Dim dicLusePgons As IDictionary(Of Integer, TplnLusePgon)
				If oLusePgonTopology IsNot Nothing Then
					Dim dicLots As TPlanGraph.TplnLots = TplnProject.Lots(iTopoPurpose)
					If dicLots IsNot Nothing Then
						Dim colPolygons As PolygonCollection = oLusePgonTopology.GetPolygons()
						Dim oLanduse As TplnLanduse = Nothing
						Dim tColorScheme As DMAcadExt.ColorScheme
						Dim tPgonCenterPoint As Autodesk.AutoCAD.Geometry.Point3d
						dicLusePgons = zzGetLusePgons(iTopoPurpose, True)

						For Each oPolygon As Polygon In colPolygons
							Try
								tPgonCenterPoint = oPolygon.Centroid
								oLusePolygon = New TplnLusePgon(iTopoPurpose, oPolygon)
								oPolygon.Dispose()
								oPolygon = Nothing
							Catch oMapEx As Autodesk.Gis.Map.MapException
								DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "LusePgons", False, "CenterDisPgon=" & DMAcadExt.TPlnPoint.DispPoint(tPgonCenterPoint))
								Return
							End Try

							Try
								oLotPgon = oLotTopology.FindPolygon(tPgonCenterPoint)
							Catch oMapEx As Autodesk.Gis.Map.MapException
								DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "LoadLusePgons-LotPgon", False, "CenterDisPgon=" & DMAcadExt.TPlnPoint.DispPoint(tPgonCenterPoint))
								'	Return
							End Try

							If oLotPgon IsNot Nothing AndAlso dicLots.ContainsKey(oLotPgon.ID) Then ''''Lena(xotela)
								oLot = dicLots.Item(oLotPgon.ID)
								iLanduseID = oLot.LanduseID
								If iLanduseID <> 0 Then
									oLusePolygon.LanduseID = iLanduseID
									oLusePolygon.LanduseName = oLot.LanduseName

									tColorScheme = TplnLot.GetColorScheme(iTopoPurpose, iLanduseID, 1.0)
									If tColorScheme.IsInstance Then

										TPlanGraph.TplnLusePgon.AddDataToLusePgonTable(iTopoPurpose, oLusePolygon, iLanduseID, tColorScheme.Name)
									Else
										TPlanGraph.TplnLusePgon.AddDataToLusePgonTable(iTopoPurpose, oLusePolygon, 0, String.Empty)
										'DMAcadExt.AcadDocument.WriteDebugMessage("$$$$6 " & CStr(oLotPgon.ID))
									End If
								Else
									TPlanGraph.TplnLusePgon.AddDataToLusePgonTable(iTopoPurpose, oLusePolygon, 0, String.Empty)
									'DMAcadExt.AcadDocument.WriteMessage("####345-- " & "iLanduseID = 0")
								End If
								dicLusePgons.Add(oLusePolygon.TopoID, oLusePolygon)
							Else
								DMAcadExt.AcadDocument.WriteMessage("####77-- " & CStr(oLotPgon.ID) & " - was not found")
							End If
							If oPolygon IsNot Nothing Then
								oPolygon.Dispose()
								oPolygon = Nothing
							End If
						Next
						If iTopoPurpose = enTopoPurpose.Approved Then
							mdicLusePgonsAppr = dicLusePgons

						ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
							mdicLusePgonsProp = dicLusePgons

						Else
							System.Windows.Forms.MessageBox.Show("Err #2812", "TplnProject - LoadLuses")
						End If
					End If
					oLusePgonTopology.Close()
				End If
				oLotTopology.Close()
			End If
		End Sub

		Public Shared Sub LoadLusePgons(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, sLotTopoName As String, sLuseTopoName As String)
			TPlanGraph.TplnLusePgon.CreateLusePgonDataTable(iTopoPurpose)

			Dim oLotTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sLotTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			If oLotTopology IsNot Nothing Then
				Dim oLusePgonTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(sLuseTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
				Dim oLotPgon As Polygon = Nothing
				Dim oLot As TplnLot
				Dim oLusePolygon As TplnLusePgon
				Dim iLanduseID As Integer
				Dim dicLusePgons As IDictionary(Of Integer, TplnLusePgon)
				If oLusePgonTopology IsNot Nothing Then
					Dim dicLots As TPlanGraph.TplnLots = TplnProject.Lots(iTopoPurpose)
					If dicLots IsNot Nothing Then
						Dim colPolygons As PolygonCollection = oLusePgonTopology.GetPolygons()
						Dim oLanduse As TplnLanduse = Nothing
						Dim tColorScheme As DMAcadExt.ColorScheme
						Dim tPgonCenterPoint As Autodesk.AutoCAD.Geometry.Point3d
						dicLusePgons = zzGetLusePgons(iTopoPurpose, True)

						For Each oPolygon As Polygon In colPolygons
							Try
								tPgonCenterPoint = oPolygon.Centroid
								oLusePolygon = New TplnLusePgon(iTopoPurpose, oPolygon)
								oPolygon.Dispose()
								oPolygon = Nothing
							Catch oMapEx As Autodesk.Gis.Map.MapException
								DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "LusePgons", False, "CenterDisPgon=" & DMAcadExt.TPlnPoint.DispPoint(tPgonCenterPoint))
								Return
							End Try

							Try
								oLotPgon = oLotTopology.FindPolygon(tPgonCenterPoint)
							Catch oMapEx As Autodesk.Gis.Map.MapException
								DMAcadExt.AcadErrCode.ShowMapError(oMapEx, True, "LoadLusePgons-LotPgon", False, "CenterDisPgon=" & DMAcadExt.TPlnPoint.DispPoint(tPgonCenterPoint))
								'	Return
							End Try

							If oLotPgon IsNot Nothing AndAlso dicLots.ContainsKey(oLotPgon.ID) Then ''''Lena(xotela)
								oLot = dicLots.Item(oLotPgon.ID)
								iLanduseID = oLot.LanduseID
								If iLanduseID <> 0 Then
									oLusePolygon.LanduseID = iLanduseID
									oLusePolygon.LanduseName = oLot.LanduseName

									tColorScheme = TplnLot.GetColorScheme(iTopoPurpose, iLanduseID, 1.0)
									If tColorScheme.IsInstance Then

										TPlanGraph.TplnLusePgon.AddDataToLusePgonTable(iTopoPurpose, oLusePolygon, iLanduseID, tColorScheme.Name)
									Else
										TPlanGraph.TplnLusePgon.AddDataToLusePgonTable(iTopoPurpose, oLusePolygon, 0, String.Empty)
										'DMAcadExt.AcadDocument.WriteDebugMessage("$$$$6 " & CStr(oLotPgon.ID))
									End If
								Else
									TPlanGraph.TplnLusePgon.AddDataToLusePgonTable(iTopoPurpose, oLusePolygon, 0, String.Empty)
									'DMAcadExt.AcadDocument.WriteMessage("####345-- " & "iLanduseID = 0")
								End If
								dicLusePgons.Add(oLusePolygon.TopoID, oLusePolygon)
							Else
								DMAcadExt.AcadDocument.WriteMessage("####77-- " & CStr(oLotPgon.ID) & " - was not found")
							End If
							If oPolygon IsNot Nothing Then
								oPolygon.Dispose()
								oPolygon = Nothing
							End If
						Next
					End If
					oLusePgonTopology.Close()
				End If
				oLotTopology.Close()
			End If
		End Sub
		Public Shared Sub LoadUnion(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose)

			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			Dim dicUnionPgons As TPlanGraph.TplnOverlayPgons
			Dim oODTable As Autodesk.Gis.Map.ObjectData.Table
			Dim iParcelTopoID As Integer
			Dim oParcel As TplnParcel
			Dim iLotTopoID As Integer
			Dim oLot As TplnLot
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex
			Dim tUnionTopoDefID As DMAcadExt.TopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Parcel, iTopoPurpose, DMAcadExt.enOverlayMethod.Union)
			Dim oOverlayODRecordset As TplnOverlayODRecordset
			Dim oUnionTopoDef As DMAcadExt.TopoDef = TopoDefs.Item(tUnionTopoDefID)
			Dim oSourceTopoDef As DMAcadExt.TopoDef = TopoDefs.Item(tUnionTopoDefID.SourceID)
			Dim oOverlayTopoDef As DMAcadExt.TopoDef = TopoDefs.Item(tUnionTopoDefID.OverlayID)
			Dim tOverlayOD As OverlayOD
			If iTopoPurpose = enTopoPurpose.Approved Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.ApprUnion
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.PropUnion
			End If
			DMCommon.Debug.MsgBox("13_196", iTopoPurpose, iOverlayIndex)
			mdicOverlayGroups(iOverlayIndex) = New TPlanGraph.TplnOverlayGroups

			oODTable = ODEditor.GetODTable(oUnionTopoDef.ODTableName)
			If oODTable IsNot Nothing Then
				oOverlayODRecordset = New TplnOverlayODRecordset(oSourceTopoDef.Name, oOverlayTopoDef.Name, oUnionTopoDef.ODTableName)

				Dim oUnionTopology As TopologyModel = DMAcadExt.AcadMapApp.GetTopology(oUnionTopoDef.Name)
				If oUnionTopology IsNot Nothing Then
					Dim oUnionPgon As TplnOverlayPgon
					If oUnionTopology.Status = Status.Closed Then
						Try
							oUnionTopology.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
						Catch oMapEx As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadUnion_21")
							Return
						End Try


						If iTopoPurpose = enTopoPurpose.Proposed Then
							mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropUnion) = New TPlanGraph.TplnOverlayPgons
							dicUnionPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropUnion)
							mdicUnionGroupsAppr = New TplnUnionGroups()
						ElseIf iTopoPurpose = enTopoPurpose.Approved Then
							mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprUnion) = New TPlanGraph.TplnOverlayPgons
							dicUnionPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprUnion)
							mdicUnionGroupsProp = New TplnUnionGroups()
						Else
							System.Windows.Forms.MessageBox.Show("Err 11511", "TplnProject - LoadUnion_56", MessageBoxButtons.OK, MessageBoxIcon.Stop)
							Exit Sub
						End If

						Dim colPolygons As PolygonCollection = oUnionTopology.GetPolygons()
						For Each oPolygon As Polygon In colPolygons
							tOverlayOD = oOverlayODRecordset.GetOverlayOD(oPolygon.Entity)
							oUnionPgon = New TplnOverlayPgon(oPolygon, DMAcadExt.enOverlayMethod.Union, iTopoPurpose, tOverlayOD)
							iParcelTopoID = oUnionPgon.ParcelTopoID
							iLotTopoID = oUnionPgon.LotTopoID

							If iParcelTopoID = 0 Then
								System.Windows.Forms.MessageBox.Show("Parcel = 0 => " & CStr(iLotTopoID), "LoadUnion")
							ElseIf mdicParcels.ContainsKey(iParcelTopoID) Then
								oParcel = mdicParcels.Item(iParcelTopoID)
								If iLotTopoID = 0 Then
									oParcel.OutPlan = True
								ElseIf dicLots.ContainsKey(iLotTopoID) Then
									oLot = dicLots.Item(iLotTopoID)
									If oLot.InPlan Then
										oParcel.InPlan(iOverlayIndex) = True
									Else
										oParcel.OutPlan(iOverlayIndex) = True
									End If
									oLot.AddOverlayPgon(DMAcadExt.enOverlayMethod.Union, oUnionPgon)
								Else
									System.Windows.Forms.MessageBox.Show("Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus) & vbCrLf & " - was not found", "LoadUnion" & iTopoPurpose.ToString())
								End If
								oParcel.AddOverlayPgon(iTopoPurpose, DMAcadExt.enOverlayMethod.Union, oUnionPgon)
							Else
								System.Windows.Forms.MessageBox.Show("Parcel:" & CStr(iParcelTopoID) & "- was not found", "LoadUnion")
							End If
							'	dicUnionPgons.Add(oUnionPgon.TopoID, oUnionPgon)  04/02/13
							dicUnionPgons.Add(oUnionPgon)
							Try
								If iTopoPurpose = enTopoPurpose.Approved Then
									mdicUnionGroupsAppr.AddUnionPgon(enOverlayMethod.Union, oUnionPgon)
								ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
									mdicUnionGroupsProp.AddUnionPgon(enOverlayMethod.Union, oUnionPgon)
								End If

							Catch oEx As Exception
								System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnProject - LoadUnion")
							End Try
							oPolygon.Dispose()
							oPolygon = Nothing
						Next
						oUnionTopology.Close()
						oUnionTopology = Nothing
					End If
				End If
			Else
				System.Windows.Forms.MessageBox.Show("oODTable Is Nothing", "LoadUnion")
			End If
		End Sub

		Public Shared Sub LoadUnionASrc(ByVal iStatus As DMAcadExt.enTopoPurpose)

			Dim dicLots As TPlanGraph.TplnLots = Lots(iStatus)
			Dim oUnionCentroid As Autodesk.AutoCAD.Geometry.Point3d
			Dim iParcelTopoID As Integer
			Dim oParcel As TplnParcel
			Dim iLotTopoID As Integer
			Dim oLot As TplnLot

			Dim oLotTopology As TopologyModel = DMAcadExt.AcadMapApp.GetTopology(TPlanGraph.TplnLot.TopoName(iStatus))
			''''Isprav
			Dim oParcelTopology As TopologyModel = DMAcadExt.AcadMapApp.GetTopology(TopoDefs.Item(New DMAcadExt.TopoDefID(enTopoPurpose.Parcel)).Name)
			Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock = Nothing
			Dim iTestStatus As Autodesk.Gis.Map.Topology.Status
			If oLotTopology IsNot Nothing AndAlso oParcelTopology IsNot Nothing Then
				iTestStatus = oParcelTopology.Status
				oDocLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, "CommandLine", "", True)
				If oLotTopology.Status = Status.Closed Then
					Try
						oLotTopology.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadUnionA_1")
						If oDocLock IsNot Nothing Then
							oDocLock.Dispose()
							oDocLock = Nothing
						End If
						Exit Sub
					End Try
				End If

				If oParcelTopology.Status = Status.Closed Then
					Try
						oParcelTopology.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadUnionA_2")
						If oDocLock IsNot Nothing Then
							oDocLock.Dispose()
							oDocLock = Nothing
						End If
						Exit Sub
					End Try
				End If


				Dim oUnionTopology As TopologyModel = DMAcadExt.AcadMapApp.GetTopology(TplnLot.MergeTopoName(iStatus))
				If oUnionTopology Is Nothing Then
					oLotTopology.Close()
					oParcelTopology.Close()
					If oDocLock IsNot Nothing Then
						oDocLock.Dispose()
						oDocLock = Nothing
					End If
				Else
					Dim oUnionPgon As TplnOverlayPgon
					Dim oLotPgon As Polygon = Nothing
					Dim oParcelPgon As Polygon = Nothing

					If oUnionTopology.Status = Status.Closed Then
						Try
							oUnionTopology.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
						Catch oMapEx As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadUnionA-21")
							Try
								If oParcelTopology.Status <> Status.Closed Then
									oParcelTopology.Close()
								End If
							Catch oEx As Exception
								DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadUnionA-24")
							End Try
							If oDocLock IsNot Nothing Then
								oDocLock.Dispose()
								oDocLock = Nothing
							End If
							Exit Sub
						End Try
						If oUnionTopology.Status <> Status.Closed Then
							Dim colPolygons As PolygonCollection
							Try
								colPolygons = oUnionTopology.GetPolygons()
							Catch oMapEx As Autodesk.Gis.Map.MapException
								DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadUnionA-8")
								Try
									If oParcelTopology.Status = Status.Closed Then
										oParcelTopology.Close()
									End If
								Catch oMapExA As Autodesk.Gis.Map.MapException
									DMAcadExt.AcadErrCode.ShowMapError(oMapExA.ErrorCode, False, "TplnProject - LoadUnionA-25")
								End Try
								Try
									If oUnionTopology.Status <> Status.Closed Then
										oUnionTopology.Close()
									End If
								Catch oMapExB As Autodesk.Gis.Map.MapException
									DMAcadExt.AcadErrCode.ShowMapError(oMapExB.ErrorCode, False, "TplnProject - LoadUnionA-26")
								End Try
								If oDocLock IsNot Nothing Then
									oDocLock.Dispose()
									oDocLock = Nothing
								End If
								Exit Sub
							End Try

							For Each oPolygon As Polygon In colPolygons
								Try
									oUnionCentroid = oPolygon.Centroid
								Catch oMapEx As Autodesk.Gis.Map.MapException
									DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadUnionA-9")
								End Try

								Try
									oLotPgon = oLotTopology.FindPolygon(oUnionCentroid)
									iLotTopoID = zzGetTopoID(oLotPgon)
								Catch oMapEx As Autodesk.Gis.Map.MapException
									If oMapEx.ErrorCode <> 3 Then
										DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadUnionA_11")
									End If
									iLotTopoID = 0
								End Try
								Try
									oParcelPgon = oParcelTopology.FindPolygon(oUnionCentroid)
									iParcelTopoID = zzGetTopoID(oParcelPgon)
									If iParcelTopoID = 0 Then
										System.Windows.Forms.MessageBox.Show("iParcelTopoID = 0", "!!!")
									End If
								Catch oMapEx As Autodesk.Gis.Map.MapException
									If oMapEx.ErrorCode <> 3 Then
										DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadUnionA-12")
									End If
									iLotTopoID = 0
									iParcelTopoID = 0
								End Try

								oUnionPgon = New TplnOverlayPgon(oPolygon, DMAcadExt.enOverlayMethod.Merge, iStatus, iLotTopoID, iParcelTopoID)
								If iParcelTopoID = 0 Then
									Dim sTest As String = "Parcel = 0 => " & CStr(iLotTopoID)
									sTest &= ":" & CStr(oUnionCentroid.X) & "," & oUnionCentroid.Y

									DMAcadExt.AcadDocument.WriteMessage(sTest)

								ElseIf mdicParcels.ContainsKey(iParcelTopoID) Then
									oParcel = mdicParcels.Item(iParcelTopoID)
									If iLotTopoID = 0 Then
										oParcel.OutPlan = True
									ElseIf dicLots.ContainsKey(iLotTopoID) Then
										oLot = dicLots.Item(iLotTopoID)
										If oLot.InPlan Then
											oParcel.InPlan = True
											oUnionPgon.LotOut = False
										Else
											oParcel.OutPlan = True
											oUnionPgon.LotOut = True
										End If
										oLot.AddOverlayPgon(DMAcadExt.enOverlayMethod.Merge, oUnionPgon)
									Else
										System.Windows.Forms.MessageBox.Show("Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus) & vbCrLf & " - was not found", "LoadUnion" & iStatus.ToString())
									End If
									oParcel.AddOverlayPgon(iStatus, DMAcadExt.enOverlayMethod.Merge, oUnionPgon)
								Else
									System.Windows.Forms.MessageBox.Show("Parcel:" & CStr(iParcelTopoID) & "- was not found", "LoadUnion")
								End If
							Next
							oLotTopology.Close()
							'oLotTopology.Dispose()
							oLotTopology = Nothing

							oParcelTopology.Close()
							'oParcelTopology.Dispose()
							oParcelTopology = Nothing

							oUnionTopology.Close()
							' oUnionTopology.Dispose()
							oUnionTopology = Nothing

							If oDocLock IsNot Nothing Then
								oDocLock.Dispose()
								oDocLock = Nothing
							End If
						End If
					End If
				End If
			Else
				System.Windows.Forms.MessageBox.Show("oODTable Is Nothing", "LoadUnion")
			End If
		End Sub
		Public Shared Sub LoadUnionAAA(ByVal iStatus As DMAcadExt.enTopoPurpose)
			Dim oUnionTopology As TopologyModel = DMAcadExt.AcadMapApp.GetTopology(TPlanGraph.TplnLot.UnionTopoName(iStatus))
			If oUnionTopology IsNot Nothing Then
				Dim dicLots As TPlanGraph.TplnLots
				Try
					If oUnionTopology.Status = Status.Closed Then
						oUnionTopology.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
					End If
				Catch oMapEx As Autodesk.Gis.Map.MapException
					System.Windows.Forms.MessageBox.Show("Cannot open  topology '" & TPlanGraph.TplnLot.GetTopoName(iStatus) & "'", "TplnProject - LoadUnionNew")
					DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadUnionNew_01")
					Return
				End Try
				If oUnionTopology.Status <> Status.Closed Then
					Dim colPolygons As PolygonCollection
					Try
						colPolygons = oUnionTopology.GetPolygons()
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - LoadUnionNew_02")
						Return
					End Try
					dicLots = New TPlanGraph.TplnLots

					TplnLot.CreateMainDataTable(iStatus)

					For Each oPolygon As Polygon In colPolygons



					Next


					If iStatus = enTopoPurpose.Proposed Then
						mdicLotsProp = dicLots
						mdicLotsProp.PlanStatus = enTopoPurpose.Proposed
					ElseIf iStatus = enTopoPurpose.Approved Then
						mdicLotsAppr = dicLots
						mdicLotsAppr.PlanStatus = enTopoPurpose.Approved

					Else
						System.Windows.Forms.MessageBox.Show("Err:12893", "TplnProject - LoadUnionNew")
					End If
					Return
				Else
					System.Windows.Forms.MessageBox.Show("Lot(" & iStatus.ToString() & ") Topology Is Nothing", "TplnProject - LoadUnionNew")
					Return
				End If
			Else
				System.Windows.Forms.MessageBox.Show("Lot(" & iStatus.ToString() & ") Topology Is Nothing", "TplnProject - LoadUnionNew")
				Return
			End If
		End Sub
		Public Shared Function LoadMerge(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Boolean
			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			Dim dicOverlayPgons As TPlanGraph.TplnOverlayPgons
			Dim tUnionCentroid As Autodesk.AutoCAD.Geometry.Point3d
			Dim iParcelTopoID As Integer
			Dim oParcel As TplnParcel
			Dim iLotTopoID As Integer
			Dim oLot As TplnLot
			Dim sLotTopoName As String
			Dim tLotTopoDefID As DMAcadExt.TopoDefID = New DMAcadExt.TopoDefID(iTopoPurpose)
			Dim oLotTopoDef As DMAcadExt.TopoDef = TopoManager.TopoDefs.Item(tLotTopoDefID)
			Dim oLotAdditionalTopoDef As DMAcadExt.TopoDef = TopoManager.TopoDefs.Item(tLotTopoDefID.AdditionalID)
			Dim sParcelTopoName As String
			Dim tParcelTopoDefID As DMAcadExt.TopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Parcel)
			Dim oParcelTopoDef As DMAcadExt.TopoDef = TopoManager.TopoDefs.Item(tParcelTopoDefID)
			Dim oParcelAdditionalTopoDef As DMAcadExt.TopoDef = TopoManager.TopoDefs.Item(tParcelTopoDefID.AdditionalID)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex
			Dim iTestCounter As Integer = 0
			If iTopoPurpose = enTopoPurpose.Approved Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.ApprMerge
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.PropMerge
			End If
			mdicOverlayGroups(iOverlayIndex) = New TPlanGraph.TplnOverlayGroups


			If DMAcadExt.AcadTransaction.LinkExists(oLotAdditionalTopoDef.LinkLayers) Then
				sLotTopoName = oLotAdditionalTopoDef.Name
			Else
				sLotTopoName = oLotTopoDef.Name
			End If
			Dim oLotTopology As TopologyModel = DMAcadExt.AcadMapApp.GetTopology(sLotTopoName)

			If DMAcadExt.AcadTransaction.LinkExists(oParcelAdditionalTopoDef.LinkLayers) Then
				sParcelTopoName = oParcelAdditionalTopoDef.Name
			Else
				sParcelTopoName = oParcelTopoDef.Name
			End If
			''''Isprav
			Dim oParcelTopology As TopologyModel = DMAcadExt.AcadMapApp.GetTopology(sParcelTopoName)

			If oLotTopology IsNot Nothing AndAlso oParcelTopology IsNot Nothing Then
				DMAcadExt.AcadDocument.WriteDebugMessage("!!!" & sParcelTopoName & "," & oParcelTopology.Status.ToString() & "," & oLotTopology.Status.ToString())
				If oLotTopology.Status = Status.Closed Then
					Try
						oLotTopology.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "Tpr - LoadMerge_1", oLotTopology.Name)
						Return False
					End Try
				End If

				If oParcelTopology.Status = Status.Closed Then
					Try
						oParcelTopology.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "Tpr - LoadMerge_2")
						oLotTopology.Close()
						Return False
					End Try
				End If
				'	System.Windows.Forms.MessageBox.Show(oParcelTopology.Status.ToString() & ":" & oParcelTopology.Name, "25_500")
				Dim oMergeTopology As TopologyModel = TopoManager.TopoCreator.GetOpenedTopology(TplnLot.MergeTopoName(iTopoPurpose), Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, True)
				If oMergeTopology Is Nothing OrElse oMergeTopology.Status = Status.Closed Then
					oLotTopology.Close()
					oParcelTopology.Close()
					MessageBox.Show("Topology '" & TplnLot.MergeTopoName(iTopoPurpose) & "' is not proper", "", MessageBoxButtons.OK, MessageBoxIcon.Stop)
					Return False
				Else
					Dim oOverlayPgon As TplnOverlayPgon
					Dim oLotPgon As Polygon = Nothing
					Dim oParcelPgon As Polygon = Nothing
					Dim colMergePolygons As PolygonCollection
					Try
						colMergePolygons = oMergeTopology.GetPolygons()
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "Tpr - LoadMerge_8")
						Try
							If oParcelTopology.Status <> Status.Closed Then
								oParcelTopology.Close()
								oParcelTopology = Nothing
							End If
						Catch oMapExA As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapExA.ErrorCode, True, "Tpr - LoadMerge_25b")
						End Try
						Try
							If oLotTopology.Status <> Status.Closed Then
								oLotTopology.Close()
								oLotTopology = Nothing
							End If
						Catch oMapExA As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapExA.ErrorCode, True, "Tpr - LoadMerge_25c")
						End Try
						Try
							If oMergeTopology.Status <> Status.Closed Then
								oMergeTopology.Close()
								oMergeTopology = Nothing
							End If
						Catch oMapExB As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapExB.ErrorCode, False, "Tpr - LoadMerge_26")
						End Try
						Return False
					End Try 'End of  Catch oMapEx As Autodesk.Gis.Map.MapException

					If iTopoPurpose = enTopoPurpose.Proposed Then
						mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropMerge) = New TPlanGraph.TplnOverlayPgons
						dicOverlayPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropMerge)
						mdicUnionGroupsProp = New TplnUnionGroups()
					ElseIf iTopoPurpose = enTopoPurpose.Approved Then
						mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprMerge) = New TPlanGraph.TplnOverlayPgons
						dicOverlayPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprMerge)
						mdicUnionGroupsAppr = New TplnUnionGroups()
					Else
						System.Windows.Forms.MessageBox.Show("Err 11511", "Tpr - LoadMerge_55", MessageBoxButtons.OK, MessageBoxIcon.Stop)
						Return False
					End If
					'	'	'	'	''''''''''''''''''''''''''''''''''''''''''''''''''
					''''''''''''''''''''''''''''''''''''
					For Each oPolygon As Polygon In colMergePolygons
						oParcel = Nothing
						oLot = Nothing
						iTestCounter += 1
						Try
							tUnionCentroid = oPolygon.Centroid
						Catch oMapEx As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "Tpr - LoadMerge_9")
						End Try

						Try
							oLotPgon = oLotTopology.FindPolygon(tUnionCentroid)
							iLotTopoID = zzGetTopoID(oLotPgon)
							oLotPgon.Dispose()
							oLotPgon = Nothing
						Catch oMapEx As Autodesk.Gis.Map.MapException
							If oMapEx.ErrorCode <> 3 Then
								DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "Tpr - LoadMerge_11")
							End If
							iLotTopoID = 0
						End Try

						Try
							oParcelPgon = oParcelTopology.FindPolygon(tUnionCentroid)
							iParcelTopoID = zzGetTopoID(oParcelPgon)
							If iParcelTopoID = 0 Then
								DMAcadExt.AcadDocument.WriteMessage("ParcelTopoID = 0")
							End If
							'If iParcelTopoID = 15152 Then
							'	DMAcadExt.AcadDocument.WriteMessage("46_41: i=" & CStr(iTestCounter) & "# " & CStr(iLotTopoID))
							'End If
							oParcelPgon.Dispose()
							oParcelPgon = Nothing
						Catch oMapEx As Autodesk.Gis.Map.MapException
							If oMapEx.ErrorCode <> 3 Then
								DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "Tpr - LoadMerge_12")
							End If
							iLotTopoID = 0
							iParcelTopoID = 0
						End Try
						'If iParcelTopoID = 15152 Then
						'	DMAcadExt.AcadDocument.WriteMessage("51_06: i=" & CStr(iTestCounter) & "# " & CStr(iLotTopoID) & "!::!" & CStr(oPolygon.ID))
						'End If
						oOverlayPgon = New TplnOverlayPgon(oPolygon, DMAcadExt.enOverlayMethod.Merge, iTopoPurpose, iLotTopoID, iParcelTopoID)
						If iParcelTopoID = 0 Then
							Dim sTestA As String = "Parcel = 0 => " & CStr(iLotTopoID)
							sTestA &= ":" & CStr(tUnionCentroid.X) & "," & tUnionCentroid.Y ''& vbCrLf
							DMAcadExt.AcadDocument.WriteMessage(sTestA)
						ElseIf mdicParcels.ContainsKey(iParcelTopoID) Then
							oParcel = mdicParcels.Item(iParcelTopoID)
							'If iParcelTopoID = 15152 Then
							'	DMAcadExt.AcadDocument.WriteMessage("!51_13: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex))
							'End If
							If iLotTopoID = 0 Then
								oParcel.OutPlan(iOverlayIndex) = True
							ElseIf dicLots.ContainsKey(iLotTopoID) Then
								oLot = dicLots.Item(iLotTopoID)
								If oLot.InPlan Then
									oParcel.InPlan(iOverlayIndex) = True
									oOverlayPgon.LotOut = False
								Else
									oParcel.OutPlan(iOverlayIndex) = True
									oOverlayPgon.LotOut = True
								End If
								oLot.AddOverlayPgon(DMAcadExt.enOverlayMethod.Merge, oOverlayPgon)
							Else
								DMAcadExt.AcadDocument.WriteMessage("LoadMerge " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus) & vbCrLf & " - was not found", "LoadMerge" & iTopoPurpose.ToString())
							End If

							oParcel.AddOverlayPgon(iTopoPurpose, DMAcadExt.enOverlayMethod.Merge, oOverlayPgon)
						Else
							DMAcadExt.AcadDocument.WriteMessage("LoadMerge " & "Parcel:" & CStr(iParcelTopoID) & "- was not found", "LoadMerge")
						End If
						'If iParcelTopoID = 15152 Then
						'	DMAcadExt.AcadDocument.WriteMessage("51_12: " & CStr(oUnionPgon.LotTopoID) & "!::!" & oUnionPgon.TopoID)
						'End If
						'dicOverlayPgons.Add(oOverlayPgon.TopoID, oOverlayPgon)  04/02/13
						dicOverlayPgons.Add(oOverlayPgon)
						'		MessageBox.Show(CStr(oUnionPgon.ParcelTopoID) & ":" & CStr(oUnionPgon.LotTopoID & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing) & vbCrLf & CStr(oLot Is Nothing)), "02_124")
						Try
							'DMAcadExt.AcadDocument.WriteMessageLog("011 " & CStr(iParcelTopoID) & ":" & CStr(iLotTopoID))
							If mdicOverlayGroups(iOverlayIndex).AddOverlayPgon(oOverlayPgon, "LoadMerge") Then
								If oParcel IsNot Nothing Then
									'If iParcelTopoID = 15152 Then
									'	DMAcadExt.AcadDocument.WriteMessage("51_66: i=" & CStr(iTestCounter) & "# " & CStr(oUnionPgon.LotTopoID) & "!::!" & oUnionPgon.TopoID)
									'	DMAcadExt.AcadDocument.WriteMessage("!51_72: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex) & "!::!" & oUnionPgon.TopoID)
									'End If
									oParcel.AddOverlay(iOverlayIndex, iLotTopoID)
									If iLotTopoID <> 0 Then
										oParcel.AddLot(iTopoPurpose, DMAcadExt.enOverlayMethod.Merge, iLotTopoID, oLot.InPlan, oLot.RegionNo, oLot.LanduseID)
									End If
									'If iParcelTopoID = 15152 Then
									'	DMAcadExt.AcadDocument.WriteMessage("!51_80: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex))
									'End If
								End If
								If iLotTopoID <> 0 AndAlso oLot IsNot Nothing Then
									oLot.AddOverlay(iOverlayIndex, iParcelTopoID)
								End If
							End If
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "Tpr - LoadMerge_37")
						End Try

						'	MessageBox.Show(mdicOverlayGroups(iOverlayIndex).GetTest6(), "02_115")
						Try

							If iTopoPurpose = enTopoPurpose.Approved Then
								mdicUnionGroupsAppr.AddUnionPgon(enOverlayMethod.Merge, oOverlayPgon)
							ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
								mdicUnionGroupsProp.AddUnionPgon(enOverlayMethod.Merge, oOverlayPgon)
							End If

						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message, "Tpr - LoadMerge_3")
						End Try
						oPolygon.Dispose()
						oPolygon = Nothing
						'If iParcelTopoID = 15152 Then
						'	DMAcadExt.AcadDocument.WriteMessage("!51_86: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex))
						'End If
					Next

					'''''''''''''''''''''''''''''''
					Try
						oLotTopology.Close()
						'	oLotTopology.Dispose()
						oLotTopology = Nothing
						oParcelTopology.Close()
						'	oParcelTopology.Dispose()
						oParcelTopology = Nothing

						oMergeTopology.Close()
						'	oMergeTopology.Dispose()
						oMergeTopology = Nothing
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message, "Tpr - LoadMerge_4")
					End Try
				End If
				Return True
			Else
				System.Windows.Forms.MessageBox.Show("Lot Topology Or Parcel Topology  was not found", "LoadMerge")
				Return False
			End If
		End Function
		Public Shared Function LoadFDO_OverlayCP_New(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Boolean
			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			Dim dicOverlayPgons As TPlanGraph.TplnOverlayPgons

			Dim iParcelTopoID As Integer
			Dim oParcel As TplnParcel
			Dim iLotTopoID As Integer
			Dim oLot As TplnLot

			Dim iOverlayIndex As DMAcadExt.enOverlayIndex
			Dim sPolylineLayer As String
			Dim iTestCounter As Integer = 0
			Dim tLayerDef As DMAcadExt.AcadLayerDef
			Dim sTableName As String
			If iTopoPurpose = enTopoPurpose.Approved Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.ApprFDO_Overlay
				tLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.OverlayFDO_LotKParcel)
				sTableName = "LotKParcel"
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.PropFDO_Overlay
				tLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.OverlayFDO_LotMParcel)
				sTableName = "LotMParcel"
			Else
				System.Windows.Forms.MessageBox.Show("", "LoadFDO_1317")
				Return False
			End If
			Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.CreateLayer(tLayerDef, True)
			sPolylineLayer = tLayerDef.Name
			mdicOverlayGroups(iOverlayIndex) = New TPlanGraph.TplnOverlayGroups

			Dim oOverlayPgon As TplnOverlayPgon
			Dim oLotPgon As Polygon = Nothing
			Dim oParcelPgon As Polygon = Nothing
			Dim lstPolygons As IDictionary(Of Integer, DMAcadExt.MPolygonOverlay)
			If iTopoPurpose = enTopoPurpose.Approved Then
				mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprFDO_Overlay) = New TPlanGraph.TplnOverlayPgons
				dicOverlayPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
				mdicUnionGroupsAppr = New TplnUnionGroups()
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropFDO_Overlay) = New TPlanGraph.TplnOverlayPgons
				dicOverlayPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
				mdicUnionGroupsProp = New TplnUnionGroups()
			Else
				System.Windows.Forms.MessageBox.Show("Err 11511", "Tpr - LoadFDO_Overlay_55", MessageBoxButtons.OK, MessageBoxIcon.Stop)
				Return False
			End If
			''''''''''''''''''''''''''''''''''''''''''''''''''
			''''''''''''''''''''''''''''''''''''
			Dim oResBuffer As ResultBuffer = Nothing
			Dim iFeatureID As Integer

			lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines2MPolygons(sPolylineLayer, XDataAppName, mbAddOverlayExists)

			System.Windows.Forms.MessageBox.Show("LoadFDO_OverlayCP_New" & vbCrLf & iTopoPurpose.ToString() & vbCrLf & iOverlayIndex.ToString() & vbCrLf & sPolylineLayer & vbCrLf & CStr(lstPolygons.Count) & vbCrLf & CStr(mdicParcels.Count), "04_150bn")
			Dim sMsg As String = ""
			Dim dicEnt As ObjectIdCollection
			Dim tFirstEnt As ObjectId
			Dim oFirstObj As Autodesk.AutoCAD.DatabaseServices.DBObject
			Dim sFirstHandle As String = "**"
			Dim iTestEnt As Integer = 0
			Dim iTestPrc As Integer = 0
			Dim iTestLot As Integer = 0
			Dim iTestPrc_2 As Integer = 0
			For Each oPolygon As DMAcadExt.MPolygonOverlay In lstPolygons.Values
				oParcel = Nothing
				oLot = Nothing
				iFeatureID = oPolygon.FeatureID
				iParcelTopoID = oPolygon.SourceID
				iLotTopoID = oPolygon.OverlayID
				dicEnt = oPolygon.Entities
				iTestEnt += dicEnt.Count

				If iParcelTopoID = 2 Then
					iTestPrc_2 += 1

					If dicEnt.Count > 0 Then
						tFirstEnt = dicEnt.Item(0)
						oFirstObj = DMAcadExt.AcadTransaction.GetDBObject(tFirstEnt, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
						sFirstHandle = oFirstObj.Handle.ToString()
						'DMAcadExt.AcadDocument.WriteMessage("#49 " & CStr(iFeatureID) & ":" & CStr(iParcelTopoID) & ":" & CStr(iLotTopoID) & "*" & sFirstHandle)
					End If
				End If
				iTestCounter += 1
				'	sHandle = oPolygon.Handle.ToString()

				If True Then

					If iTestCounter < 0 Then
						DMAcadExt.AcadDocument.WriteDebugMessage("45** " & CStr(iFeatureID) & ":" & CStr(iParcelTopoID) & ":" & CStr(iLotTopoID) & "*" & sMsg)
					End If
					'	System.Windows.Forms.MessageBox.Show(CStr(iLotTopoID) & vbCrLf & CStr(iParcelTopoID), "04_256")
					oOverlayPgon = New TplnOverlayPgon(oPolygon, oPolygon.Entities, iFeatureID, DMAcadExt.enOverlayMethod.FDO_Overlay, iTopoPurpose)
					' 
					If iParcelTopoID = 0 Then
						Dim sTestA As String = "Parcel = 0 => " & CStr(iLotTopoID) & "," & CStr(oPolygon.FeatureID) & "," & CStr(oPolygon.Area)
						'	sTestA &= ":" & CStr(tUnionCentroid.X) & "," & tUnionCentroid.Y ''& vbCrLf
						DMAcadExt.AcadDocument.WriteMessage(sTestA)
					ElseIf mdicParcels.ContainsKey(iParcelTopoID) Then
						oParcel = mdicParcels.Item(iParcelTopoID)
						'If iParcelTopoID = 15152 Then
						'	DMAcadExt.AcadDocument.WriteMessage("!51_13: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex))
						'End If
						If iLotTopoID = 0 Then
							oParcel.OutPlan(iOverlayIndex) = True
						ElseIf dicLots.ContainsKey(iLotTopoID) Then
							oLot = dicLots.Item(iLotTopoID)
							If oLot.InPlan Then
								oParcel.InPlan(iOverlayIndex) = True
								oOverlayPgon.LotOut = False
							Else
								oParcel.OutPlan(iOverlayIndex) = True
								oOverlayPgon.LotOut = True
							End If
							'DMAcadExt.AcadDocument.WriteMessage("!51_34: i=" & CStr(iTestCounter) & "# " & oOverlayPgon.LotTopoID & "||" & CStr(oOverlayPgon.ParcelTopoID))	'oParcel.TestOverlayList(iOverlayIndex) & ":" & 
							'	System.Windows.Forms.MessageBox.Show(CStr(oOverlayPgon.LotTopoID) & vbCrLf & CStr(oOverlayPgon.ParcelTopoID) & vbCrLf & CStr(oLot.TopoID), "04_257")
							oLot.AddOverlayPgon(DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)
							iTestLot += 1
						Else
							oParcel.OutPlan = True
							DMAcadExt.AcadDocument.WriteMessage("LoadFDO_OverlayI " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus) & vbCrLf & " - was not found", "LoadFDO_Overlay" & iTopoPurpose.ToString())
						End If
						'	DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay! " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus))
						oParcel.AddOverlayPgon(iTopoPurpose, DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)
					Else
						DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay Parcel:" & CStr(iParcelTopoID) & "- was not found", "LoadFDO_Overlay")
					End If
					If iOverlayIndex = DMAcadExt.enOverlayIndex.PropFDO_Overlay Then
						''''''''''''	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & ":" & CStr(iLotTopoID), "05_704")
					End If
					Try
						If dicOverlayPgons.ContainsKey(oOverlayPgon.TopoID) Then

						End If

						'	dicOverlayPgons.Add(oOverlayPgon.TopoID, oOverlayPgon)  04/02/13
						dicOverlayPgons.Add(oOverlayPgon)
					Catch oEx As Exception
						MessageBox.Show(oEx.Message & vbCrLf & CStr(iTestCounter) & vbCrLf & CStr(oOverlayPgon.ParcelTopoID) & ":" & CStr(oOverlayPgon.TopoID) & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing), "02_114")
					End Try

					'		MessageBox.Show(CStr(oUnionPgon.ParcelTopoID) & ":" & CStr(oUnionPgon.LotTopoID & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing) & vbCrLf & CStr(oLot Is Nothing)), "02_113")
					Try
						If mdicOverlayGroups(iOverlayIndex).AddOverlayPgon(oOverlayPgon, "LoadFDO_OverlayCP_New") Then
							If oParcel IsNot Nothing Then
								oParcel.AddOverlay(iOverlayIndex, iLotTopoID)
								If iLotTopoID <> 0 Then
									''''''''''''''	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & ":" & CStr(iLotTopoID), "05_703a")
									oParcel.AddLot(iTopoPurpose, DMAcadExt.enOverlayMethod.FDO_Overlay, iLotTopoID, oLot.InPlan, oLot.RegionNo, oLot.LanduseID)
									iTestPrc += 1
								End If
								'If iParcelTopoID = 15152 Then
								'	DMAcadExt.AcadDocument.WriteMessage("!51_80: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex))
								'End If
							End If
							If (iLotTopoID <> 0) AndAlso (oLot IsNot Nothing) Then
								oLot.AddOverlay(iOverlayIndex, iParcelTopoID)
							End If
						End If
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "Tpr - LoadFDO_Overlay_31")
					End Try
					'	MessageBox.Show(mdicOverlayGroups(iOverlayIndex).GetTest6(), "02_115")
					Try
						If iTopoPurpose = enTopoPurpose.Approved Then
							mdicUnionGroupsAppr.AddUnionPgon(enOverlayMethod.Merge, oOverlayPgon)
						ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
							mdicUnionGroupsProp.AddUnionPgon(enOverlayMethod.Merge, oOverlayPgon)
						End If
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(oEx.Message, "Tpr - LoadMerge_3")
					End Try
					oPolygon.Dispose()
					oPolygon = Nothing

				Else
					System.Windows.Forms.MessageBox.Show("ODRec is Nothing" & vbCrLf & CStr(iTestCounter), "04_300")
					Dim tPoint As DMAcadExt.TPlnPoint = DMAcadExt.TPlnPoint.GetEntity2dCenter(oPolygon)
					DMAcadExt.AcadDocument.WriteMessage("E23: " & CStr(tPoint.X) & "," & CStr(tPoint.Y))
					Exit For
				End If
				If iTestCounter = 380 Then
					''''''''''Exit For
				End If
			Next
			'	System.Windows.Forms.MessageBox.Show(CStr(iTestEnt) & vbCrLf & CStr(iTestLot) & vbCrLf & CStr(iTestPrc) & vbCrLf & "Prc #2 " & CStr(iTestPrc_2), "04_160")
			'''''''''''''''''''''''''''''''
			Return True
		End Function
		Public Shared Function LoadFDO_OverlayCP(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Boolean
			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			Dim dicOverlayPgons As TPlanGraph.TplnOverlayPgons
			Dim tUnionCentroid As Autodesk.AutoCAD.Geometry.Point3d
			Dim iParcelTopoID As Integer
			Dim oParcel As TplnParcel
			Dim iLotTopoID As Integer
			Dim oLot As TplnLot
			Dim sLotTopoName As String
			Dim tLotTopoDefID As DMAcadExt.TopoDefID = New DMAcadExt.TopoDefID(iTopoPurpose)
			Dim oLotTopoDef As DMAcadExt.TopoDef = TopoManager.TopoDefs.Item(tLotTopoDefID)
			Dim oLotAdditionalTopoDef As DMAcadExt.TopoDef = TopoManager.TopoDefs.Item(tLotTopoDefID.AdditionalID)
			Dim sParcelTopoName As String
			Dim tParcelTopoDefID As DMAcadExt.TopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Parcel)
			Dim oParcelTopoDef As DMAcadExt.TopoDef = TopoManager.TopoDefs.Item(tParcelTopoDefID)
			Dim oParcelAdditionalTopoDef As DMAcadExt.TopoDef = TopoManager.TopoDefs.Item(tParcelTopoDefID.AdditionalID)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex
			Dim sPolylineLayer As String
			Dim iTestCounter As Integer = 0
			Dim tLayerDef As DMAcadExt.AcadLayerDef
			Dim sTableName As String
			If iTopoPurpose = enTopoPurpose.Approved Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.ApprFDO_Overlay
				tLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.OverlayFDO_LotKParcel)
				sTableName = "LotKParcel"
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.PropFDO_Overlay
				tLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.OverlayFDO_LotMParcel)
				sTableName = "LotMParcel"
			Else
				System.Windows.Forms.MessageBox.Show("", "LoadFDO_1317")
				Return False
			End If
			Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.CreateLayer(tLayerDef, True)
			sPolylineLayer = tLayerDef.Name
			mdicOverlayGroups(iOverlayIndex) = New TPlanGraph.TplnOverlayGroups


			If DMAcadExt.AcadTransaction.LinkExists(oLotAdditionalTopoDef.LinkLayers) Then
				sLotTopoName = oLotAdditionalTopoDef.Name
			Else
				sLotTopoName = oLotTopoDef.Name
			End If
			'		Dim oLotTopology As TopologyModel = DMAcadExt.AcadMapApp.GetTopology(sLotTopoName)

			If DMAcadExt.AcadTransaction.LinkExists(oParcelAdditionalTopoDef.LinkLayers) Then
				sParcelTopoName = oParcelAdditionalTopoDef.Name
			Else
				sParcelTopoName = oParcelTopoDef.Name
			End If
			''''Isprav
			'	Dim oParcelTopology As TopologyModel = DMAcadExt.AcadMapApp.GetTopology(sParcelTopoName)





			Dim oOverlayPgon As TplnOverlayPgon
			Dim oLotPgon As Polygon = Nothing
			Dim oParcelPgon As Polygon = Nothing
			Dim lstPolygons As IDictionary(Of Integer, DMAcadExt.MPolygonOverlay)


			If iTopoPurpose = enTopoPurpose.Proposed Then
				mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropFDO_Overlay) = New TPlanGraph.TplnOverlayPgons
				dicOverlayPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
				mdicUnionGroupsProp = New TplnUnionGroups()
			ElseIf iTopoPurpose = enTopoPurpose.Approved Then
				mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprFDO_Overlay) = New TPlanGraph.TplnOverlayPgons
				dicOverlayPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
				mdicUnionGroupsAppr = New TplnUnionGroups()
			Else
				System.Windows.Forms.MessageBox.Show("Err 11511", "Tpr - LoadFDO_Overlay_55", MessageBoxButtons.OK, MessageBoxIcon.Stop)
				Return False
			End If
			''''''''''''''''''''''''''''''''''''''''''''''''''
			''''''''''''''''''''''''''''''''''''
			Dim oResBuffer As ResultBuffer = Nothing
			'	Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
			'	Dim oLinkTable As DMAcadExt.ODTable

			'	Dim sHandle As String
			Dim iFeatureID As Integer
			'		System.Windows.Forms.MessageBox.Show(sPolylineLayer & vbCrLf & sTableName, "04_100")



			lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines2MPolygons(sPolylineLayer, XDataAppName, mbAddOverlayExists)
			'System.Windows.Forms.MessageBox.Show(CStr(lstPolygons.Count), "04_150b")
			Dim sMsg As String = ""
			For Each oPolygon As DMAcadExt.MPolygonOverlay In lstPolygons.Values
				oParcel = Nothing
				oLot = Nothing
				iFeatureID = oPolygon.FeatureID
				iParcelTopoID = oPolygon.SourceID
				iLotTopoID = oPolygon.OverlayID

				iTestCounter += 1
				'	sHandle = oPolygon.Handle.ToString()

				If True Then


					'		System.Windows.Forms.MessageBox.Show(CStr(dValue) & vbCrLf & oODRec.Item(1).Type.ToString(), "04_300")
					'	iLotTopoID()
					'iParcelTopoID
					If iTestCounter < 0 Then
						DMAcadExt.AcadDocument.WriteMessage("45** " & CStr(iFeatureID) & ":" & CStr(iParcelTopoID) & ":" & CStr(iLotTopoID) & "*" & sMsg)
					End If
					'	System.Windows.Forms.MessageBox.Show(CStr(iLotTopoID) & vbCrLf & CStr(iParcelTopoID), "04_256")
					oOverlayPgon = New TplnOverlayPgon(oPolygon, oPolygon.Entities, iFeatureID, DMAcadExt.enOverlayMethod.FDO_Overlay, iTopoPurpose)
					' 
					If True Then
						If iParcelTopoID = 0 Then
							Dim sTestA As String = "!Parcel = 0 => " & CStr(iLotTopoID)
							sTestA &= ":" & CStr(tUnionCentroid.X) & "," & tUnionCentroid.Y ''& vbCrLf
							DMAcadExt.AcadDocument.WriteMessage(sTestA)
						ElseIf mdicParcels.ContainsKey(iParcelTopoID) Then
							oParcel = mdicParcels.Item(iParcelTopoID)
							'If iParcelTopoID = 15152 Then
							'	DMAcadExt.AcadDocument.WriteMessage("!51_13: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex))
							'End If
							If iLotTopoID = 0 Then
								oParcel.OutPlan(iOverlayIndex) = True
							ElseIf dicLots.ContainsKey(iLotTopoID) Then
								oLot = dicLots.Item(iLotTopoID)
								If oLot.InPlan Then
									oParcel.InPlan(iOverlayIndex) = True
									oOverlayPgon.LotOut = False
								Else
									oParcel.OutPlan(iOverlayIndex) = True
									oOverlayPgon.LotOut = True
								End If
								'DMAcadExt.AcadDocument.WriteMessage("!51_34: i=" & CStr(iTestCounter) & "# " & oOverlayPgon.LotTopoID & "||" & CStr(oOverlayPgon.ParcelTopoID))	'oParcel.TestOverlayList(iOverlayIndex) & ":" & 
								'	System.Windows.Forms.MessageBox.Show(CStr(oOverlayPgon.LotTopoID) & vbCrLf & CStr(oOverlayPgon.ParcelTopoID) & vbCrLf & CStr(oLot.TopoID), "04_257")
								oLot.AddOverlayPgon(DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)
							Else
								oParcel.OutPlan = True
								DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus) & vbCrLf & " - was not found", "LoadFDO_Overlay" & iTopoPurpose.ToString())
							End If
							'	DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay! " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus))
							oParcel.AddOverlayPgon(iTopoPurpose, DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)
						Else
							DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay " & "Parcel:" & CStr(iParcelTopoID) & "- was not found", "LoadFDO_Overlay")
						End If
						If iOverlayIndex = DMAcadExt.enOverlayIndex.PropFDO_Overlay Then
							''''''''''''	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & ":" & CStr(iLotTopoID), "05_704")
						End If
						Try
							If dicOverlayPgons.ContainsKey(oOverlayPgon.TopoID) Then

							End If

							'	dicOverlayPgons.Add(oOverlayPgon.TopoID, oOverlayPgon)  04/02/13
							dicOverlayPgons.Add(oOverlayPgon)
						Catch oEx As Exception
							MessageBox.Show(oEx.Message & vbCrLf & CStr(iTestCounter) & vbCrLf & CStr(oOverlayPgon.ParcelTopoID) & ":" & CStr(oOverlayPgon.TopoID) & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing), "02_114")
						End Try

						'		MessageBox.Show(CStr(oUnionPgon.ParcelTopoID) & ":" & CStr(oUnionPgon.LotTopoID & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing) & vbCrLf & CStr(oLot Is Nothing)), "02_113")
						Try
							'DMAcadExt.AcadDocument.WriteMessageLog("011 " & CStr(iParcelTopoID) & ":" & CStr(iLotTopoID))
							If mdicOverlayGroups(iOverlayIndex).AddOverlayPgon(oOverlayPgon, "LoadFDO_OverlayCP") Then
								If oParcel IsNot Nothing Then

									'	DMAcadExt.AcadDocument.WriteMessage("51_66: i=" & CStr(iTestCounter) & "# " & CStr(oOverlayPgon.LotTopoID) & "!:!" & oOverlayPgon.ParcelTopoID & "!:!" & oOverlayPgon.TopoID)
									'	DMAcadExt.AcadDocument.WriteMessage("!51_72: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex) & "!::!" & oUnionPgon.TopoID)
									''''''''''''	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & ":" & CStr(iLotTopoID), "05_703 ")
									oParcel.AddOverlay(iOverlayIndex, iLotTopoID)
									If iLotTopoID <> 0 Then
										''''''''''''''	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & ":" & CStr(iLotTopoID), "05_703a")
										oParcel.AddLot(iTopoPurpose, DMAcadExt.enOverlayMethod.FDO_Overlay, iLotTopoID, oLot.InPlan, oLot.RegionNo, oLot.LanduseID)
									End If
									'If iParcelTopoID = 15152 Then
									'	DMAcadExt.AcadDocument.WriteMessage("!51_80: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex))
									'End If
								End If
								If (iLotTopoID <> 0) AndAlso (oLot IsNot Nothing) Then
									oLot.AddOverlay(iOverlayIndex, iParcelTopoID)
								End If
							End If
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "Tpr - LoadFDO_Overlay_31")
						End Try
						'	MessageBox.Show(mdicOverlayGroups(iOverlayIndex).GetTest6(), "02_115")
						Try
							If iTopoPurpose = enTopoPurpose.Approved Then
								mdicUnionGroupsAppr.AddUnionPgon(enOverlayMethod.Merge, oOverlayPgon)
							ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
								mdicUnionGroupsProp.AddUnionPgon(enOverlayMethod.Merge, oOverlayPgon)
							End If
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message, "Tpr - LoadMerge_3")
						End Try
						oPolygon.Dispose()
						oPolygon = Nothing
					End If
				Else
					System.Windows.Forms.MessageBox.Show("ODRec is Nothing" & vbCrLf & CStr(iTestCounter), "04_300")
					Dim tPoint As DMAcadExt.TPlnPoint = DMAcadExt.TPlnPoint.GetEntity2dCenter(oPolygon)
					DMAcadExt.AcadDocument.WriteMessage("E23: " & CStr(tPoint.X) & "," & CStr(tPoint.Y))
					Exit For
				End If
				If iTestCounter = 380 Then
					''''''''''Exit For
				End If
			Next
			'	System.Windows.Forms.MessageBox.Show("!!", "04_160")
			'''''''''''''''''''''''''''''''
			Return True
		End Function

		Public Shared Function LoadFDO_Overlay(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Boolean


			Select Case iTopoPurpose
				Case DMAcadExt.enTopoPurpose.Approved, DMAcadExt.enTopoPurpose.Proposed
					Return zzLoadFDO_Overlay(iTopoPurpose)
				Case DMAcadExt.enTopoPurpose.OwnershipNote, DMAcadExt.enTopoPurpose.Expro
					'DMCommon.Debug.MsgBox("3105d", iTopoPurpose)
					Return zzLoadFDO_AddTheme(iTopoPurpose)
				Case Else
					Return False
			End Select
		End Function
		Private Shared Function zzLoadFDO_Expro(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Boolean
			Dim dicOverlayPgons As TPlanGraph.TplnOverlayPgons = New TplnOverlayPgons()
			Dim tUnionCentroid As Autodesk.AutoCAD.Geometry.Point3d
			Dim iParcelTopoID As Integer
			Dim oParcel As TplnParcel

			Dim iOwnershipNoteTopoID As Integer
			Dim oOwnershipNote As TplnOwnershipNote
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex
			Dim sPolylineLayer As String
			Dim iTestCounter As Integer = 0

			Dim sTableName As String


			sPolylineLayer = "OwnNotesParcel"
			sTableName = "OwnNotesParcel"


			'''''''''''''''''''''''????????????   Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.CreateLayer(tLayerDef, True)
			'  DMAcadExt.AcadDocument.WriteDebugMessage("!!04_63:" & iOverlayIndex.ToString())
			mdicOverlayGroups(iOverlayIndex) = New TPlanGraph.TplnOverlayGroups
			''''''''''''''''''''''''''''''''''''''''''''' Merhav Only !!!    sPolylineLayer = "MerhLotKParcel"
			''''Isprav


			Dim oOverlayPgon As TplnOverlayPgon
			Dim oLotPgon As Polygon = Nothing
			Dim oParcelPgon As Polygon = Nothing
			Dim lstPolygons As IDictionary(Of Integer, DMAcadExt.MPolygonOverlay)




			mdicAddOverlayPgons = New TPlanGraph.TplnOverlayPgons

			''''''''''''''''''''''''''''''''''''''''''''''''''
			''''''''''''''''''''''''''''''''''''
			Dim oResBuffer As ResultBuffer = Nothing
			'	Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
			'	Dim oLinkTable As DMAcadExt.ODTable

			'	Dim sHandle As String
			Dim iFeatureID As Integer

			lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines2MPolygons(sPolylineLayer, XDataAppName, mbAddOverlayExists)

			If lstPolygons.Count = 0 Then
				System.Windows.Forms.MessageBox.Show(" -!-!- '" & sPolylineLayer & "'" & vbCrLf & CStr(lstPolygons.Count) & " --- ", "Pgon Count 04_151s")
			End If
			TplnProject.WriteMessageBox(CStr(lstPolygons.Count), "מספר פוליגונים בחיתוך:!!!!!!!!!")
			'	Dim sMsg As String = ""
			''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			'  DMAcadExt.AcadDocument.MsgBox("05_700", lstPolygons.Count)
			' DMCommon.Debug.MsgBox("05_700", sPolylineLayer, lstPolygons.Count)
			For Each oPolygon As DMAcadExt.MPolygonOverlay In lstPolygons.Values
				oParcel = Nothing
				oOwnershipNote = Nothing
				iFeatureID = oPolygon.FeatureID
				iParcelTopoID = oPolygon.SourceID
				iOwnershipNoteTopoID = oPolygon.OverlayID


				'	mdicOwnershipNotes

				'   DMAcadExt.AcadDocument.WriteDebugMessage("!!04_69 " & iFeatureID.ToString() & " Prc= " & iParcelTopoID.ToString() & " Lot= " & iLotTopoID.ToString())
				oOverlayPgon = New TplnOverlayPgon(oPolygon, oPolygon.Entities, iFeatureID, DMAcadExt.enOverlayMethod.FDO_Overlay, iTopoPurpose)
				If iParcelTopoID = 0 Then
					Dim sMsgText As String
					Dim oBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox(oPolygon.GeometricExtents)
					Dim sSysID As String = "PNE," & CStr(iOwnershipNoteTopoID)
					Dim oCenter As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(tUnionCentroid)
					'		DMAcadExt.AcadDocument.WriteMessage(sTestA)
					If mdicOwnershipNotes.TryGetValue(iOwnershipNoteTopoID, oOwnershipNote) Then

						oCenter = oOwnershipNote.GetCentroid()
						DMAcadExt.AcadDocument.WriteMessage("!39_08: " & CStr(oCenter.X) & ", " & CStr(oCenter.Y) & " ID= " & iOwnershipNoteTopoID.ToString())
					End If
					sMsgText = "Object " & " #" & CStr(iOwnershipNoteTopoID) & ":  " & "חלקה לא קיימת"
					DMAcadExt.AppMessages.AddMessage(True, oCenter, oBoundingBox, sSysID, sMsgText, False)
				ElseIf mdicParcels.ContainsKey(iParcelTopoID) Then
					oParcel = mdicParcels.Item(iParcelTopoID)


					If iOwnershipNoteTopoID = 0 Then

					ElseIf mdicOwnershipNotes.ContainsKey(iOwnershipNoteTopoID) Then
						oOwnershipNote = mdicOwnershipNotes.Item(iOwnershipNoteTopoID)

						' DMAcadExt.AcadDocument.WriteMessage("!51_39: iD=" & iLotTopoID.ToString() & " InPlan=" & oLot.Region.ToString() & " InPlan=" & oLot.InPlan.ToString() & " Cnt=" & dicLots.Count.ToString())
						'  DMAcadExt.AcadDocument.WriteMessage(oLot.Name.ToString())





						'DMAcadExt.AcadDocument.WriteMessage("!51_34: i=" & CStr(iTestCounter) & "# " & oOverlayPgon.LotTopoID & "||" & CStr(oOverlayPgon.ParcelTopoID))	'oParcel.TestOverlayList(iOverlayIndex) & ":" & 
						'	System.Windows.Forms.MessageBox.Show(CStr(oOverlayPgon.LotTopoID) & vbCrLf & CStr(oOverlayPgon.ParcelTopoID) & vbCrLf & CStr(oLot.TopoID), "04_257")

					End If
					'   DMAcadExt.AcadDocument.WriteDebugMessage("LoadFDO_Overlay! " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus))
					oParcel.AddOverlayPgon(oOverlayPgon)

				Else
					DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay " & "Parcel:" & CStr(iParcelTopoID) & "- was not found", "LoadFDO_Overlay")
				End If
				'If iParcelTopoID = 15152 Then
				'	DMAcadExt.AcadDocument.WriteMessage("51_12: " & CStr(oUnionPgon.LotTopoID) & "!::!" & oUnionPgon.TopoID)
				'End If
				If iOverlayIndex = DMAcadExt.enOverlayIndex.PropFDO_Overlay Then
					''''''''''''	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & ":" & CStr(iLotTopoID), "05_704")
				End If
				Try
					If dicOverlayPgons.ContainsKey(oOverlayPgon.TopoID) Then

					End If

					'	dicOverlayPgons.Add(oOverlayPgon.TopoID, oOverlayPgon) 04/02/13
					''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''MessageBox.Show(CStr(oOverlayPgon.Lines.Count), "05_100")
					dicOverlayPgons.Add(oOverlayPgon)
				Catch oEx As Exception
					MessageBox.Show(oEx.Message & vbCrLf & CStr(iTestCounter) & vbCrLf & CStr(oOverlayPgon.ParcelTopoID) & ":" & CStr(oOverlayPgon.TopoID) & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing), "02_114")
				End Try

				'		MessageBox.Show(CStr(oUnionPgon.ParcelTopoID) & ":" & CStr(oUnionPgon.LotTopoID & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing) & vbCrLf & CStr(oLot Is Nothing)), "02_113")
				Try
					'DMAcadExt.AcadDocument.WriteMessageLog("011 " & CStr(iParcelTopoID) & ":" & CStr(iLotTopoID))

					If oParcel IsNot Nothing Then

						'	DMAcadExt.AcadDocument.WriteMessage("51_66: i=" & CStr(iTestCounter) & "# " & CStr(oOverlayPgon.LotTopoID) & "!:!" & oOverlayPgon.ParcelTopoID & "!:!" & oOverlayPgon.TopoID)
						'	DMAcadExt.AcadDocument.WriteMessage("!51_72: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex) & "!::!" & oUnionPgon.TopoID)
						''''''''''''	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & ":" & CStr(iLotTopoID), "05_703 ")
						oParcel.AddOverlay(iOverlayIndex, iOwnershipNoteTopoID)

						'020816    DMAcadExt.AcadDocument.WriteDebugMessage("!!Parcel: " & oParcel.BlockFull & "," & oParcel.Name & " -  Ov:" & oParcel.OverlayCount(iOverlayIndex).ToString())
						If iOwnershipNoteTopoID <> 0 Then
							'	DMCommon.Debug.MsgBox("09_549c", sPolylineLayer, iOwnershipNoteTopoID, mbAddOverlayExists, lstPolygons.Count, oOwnershipNote)
						End If

						If oOwnershipNote IsNot Nothing AndAlso iOwnershipNoteTopoID <> 0 Then
							''''''''''''''	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & ":" & CStr(iLotTopoID), "05_703a")
							'oParcel.AddOwnershipNote(iOwnershipNoteTopoID, oOwnershipNote)
						End If
						'If iParcelTopoID = 15152 Then
						'	DMAcadExt.AcadDocument.WriteMessage("!51_80: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex))
						'End If
					End If


				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "Tpr - LoadMerge_31fdo")
				End Try
				'	MessageBox.Show(mdicOverlayGroups(iOverlayIndex).GetTest6(), "02_115")

				oPolygon.Dispose()
				oPolygon = Nothing


			Next




			Return True


		End Function

		Private Shared Function zzLoadFDO_AddTheme(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Boolean


			'DMCommon.Debug.MsgBox("04_370s", "zzLoadFDO_AddTheme", iTopoPurpose.ToString())

			Dim dicOverlayPgons As TPlanGraph.TplnOverlayPgons = New TplnOverlayPgons()
			Dim tUnionCentroid As Autodesk.AutoCAD.Geometry.Point3d
			Dim iParcelTopoID As Integer
			Dim oParcel As TplnParcel

			Dim iAddThemeTopoID As Integer
			Dim oOwnershipNote As TplnOwnershipNote
			Dim oExpro As TplnExpro = Nothing
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex
			Dim sPolylineLayer As String
			Dim iTestCounter As Integer = 0

			Dim sTableName As String
			'  mbMerhavExists()
			'  tMerhavMapThemeData()

			'	mdicOwnershipNotes = New TplnOwnershipNotes()
			'DMCommon.Debug.MsgBox("04_381c", iTopoPurpose.ToString())
			Select Case iTopoPurpose
				Case DMAcadExt.enTopoPurpose.Expro
					sPolylineLayer = "ExproParcel"
					sTableName = "ExproParcel"

				Case DMAcadExt.enTopoPurpose.OwnershipNote
					sPolylineLayer = "OwnNotesParcel"
					sTableName = "OwnNotesParcel"
				Case Else
					sPolylineLayer = "aaa"
					sTableName = "aaa"
			End Select



			'''''''''''''''''''''''????????????   Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.CreateLayer(tLayerDef, True)
			'  DMAcadExt.AcadDocument.WriteDebugMessage("!!04_63:" & iOverlayIndex.ToString())
			mdicOverlayGroups(iOverlayIndex) = New TPlanGraph.TplnOverlayGroups
			''''''''''''''''''''''''''''''''''''''''''''' Merhav Only !!!    sPolylineLayer = "MerhLotKParcel"
			''''Isprav


			Dim oOverlayPgon As TplnOverlayPgon
			Dim oLotPgon As Polygon = Nothing
			Dim oParcelPgon As Polygon = Nothing
			Dim lstPolygons As IDictionary(Of Integer, DMAcadExt.MPolygonOverlay)




			mdicAddOverlayPgons = New TPlanGraph.TplnOverlayPgons

			''''''''''''''''''''''''''''''''''''''''''''''''''
			''''''''''''''''''''''''''''''''''''
			Dim oResBuffer As ResultBuffer = Nothing
			'	Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
			'	Dim oLinkTable As DMAcadExt.ODTable

			'	Dim sHandle As String
			Dim iFeatureID As Integer

			lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines2MPolygons(sPolylineLayer, XDataAppName, mbAddOverlayExists)
			'	DMCommon.Debug.MsgBox("04_370p", iTopoPurpose.ToString(), lstPolygons.Count)
			If lstPolygons.Count = 0 Then
				'	System.Windows.Forms.MessageBox.Show(" -!-!- '" & sPolylineLayer & "'" & vbCrLf & CStr(lstPolygons.Count) & " --- ", "Pgon Count 04_151s")
			End If
			TplnProject.WriteMessageBox(iTopoPurpose.ToString(), " Theme ")

			TplnProject.WriteMessageBox(CStr(lstPolygons.Count), "מספר פוליגונים בחיתוך:!!!!!!!!!")
			'	Dim sMsg As String = ""
			''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			'  DMAcadExt.AcadDocument.MsgBox("05_700", lstPolygons.Count)
			'	DMCommon.Debug.MsgBox("05_700", "!!!!!!!!!!!!!!!!!", iTopoPurpose, sPolylineLayer, lstPolygons.Count)
			'	DMCommon.Debug.MsgBox("04_370q", iTopoPurpose.ToString(), lstPolygons.Count)
			For Each oPolygon As DMAcadExt.MPolygonOverlay In lstPolygons.Values
				oParcel = Nothing
				oOwnershipNote = Nothing
				iFeatureID = oPolygon.FeatureID
				iParcelTopoID = oPolygon.SourceID
				iAddThemeTopoID = oPolygon.OverlayID

				'	mdicOwnershipNotes

				'   DMAcadExt.AcadDocument.WriteDebugMessage("!!04_69 " & iFeatureID.ToString() & " Prc= " & iParcelTopoID.ToString() & " Lot= " & iLotTopoID.ToString())
				oOverlayPgon = New TplnOverlayPgon(oPolygon, oPolygon.Entities, iFeatureID, DMAcadExt.enOverlayMethod.FDO_Overlay, iTopoPurpose)
				If iParcelTopoID = 0 Then
					Dim sMsgText As String
					Dim oBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox(oPolygon.GeometricExtents)
					Dim sSysID As String = "PNE," & CStr(iAddThemeTopoID)
					'DMCommon.Debug.MsgBox("04_380c", sSysID, iParcelTopoID.ToString(), tUnionCentroid.ToString(), mdicOwnershipNotes Is Nothing, lstPolygons.Count)
					Dim oCenter As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(tUnionCentroid)
					'DMCommon.Debug.MsgBox("04_380d", oCenter.ToString(), oBoundingBox.Left, oBoundingBox.Bottom)

					If iTopoPurpose = DMAcadExt.enTopoPurpose.OwnershipNote AndAlso mdicOwnershipNotes.TryGetValue(iAddThemeTopoID, oOwnershipNote) Then

						oCenter = oOwnershipNote.GetCentroid()
						DMAcadExt.AcadDocument.WriteMessage("!39_08: " & CStr(oCenter.X) & ", " & CStr(oCenter.Y) & " ID= " & iAddThemeTopoID.ToString())
					End If
					sMsgText = "Object " & " #" & CStr(iAddThemeTopoID) & ":  " & "חלקה לא קיימת"
					DMAcadExt.AppMessages.AddMessage(True, oCenter, oBoundingBox, sSysID, sMsgText, False)
				ElseIf mdicParcels.ContainsKey(iParcelTopoID) Then
					oParcel = mdicParcels.Item(iParcelTopoID)



					Select Case iTopoPurpose
						Case DMAcadExt.enTopoPurpose.Expro
							Dim iExproType As Integer
							Dim sExproTypeName As String
							If iAddThemeTopoID = 0 Then
								iExproType = 0
								sExproTypeName = Nothing
							Else
								If mdicExpros.TryGetValue(iAddThemeTopoID, oExpro) Then
									iExproType = oExpro.ExproTypeID
									sExproTypeName = oExpro.ExproTypeName

								Else
									iExproType = -1
									sExproTypeName = Nothing
								End If
							End If
							If iExproType <> -1 Then
								oParcel.AddExproPgon(iExproType, sExproTypeName, oPolygon.Area)
							End If





						Case DMAcadExt.enTopoPurpose.OwnershipNote
							If mdicOwnershipNotes.ContainsKey(iAddThemeTopoID) Then
								oOwnershipNote = mdicOwnershipNotes.Item(iAddThemeTopoID)
								oParcel.AddOwnershipPgon(oOwnershipNote, oPolygon.Area)

							End If
					End Select

					'   DMAcadExt.AcadDocument.WriteDebugMessage("LoadFDO_Overlay! " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus))
					oParcel.AddOverlayPgon(oOverlayPgon)

				Else
					DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay " & "Parcel:" & CStr(iParcelTopoID) & "- was not found", "LoadFDO_Overlay")
				End If
				'If iParcelTopoID = 15152 Then
				'	DMAcadExt.AcadDocument.WriteMessage("51_12: " & CStr(oUnionPgon.LotTopoID) & "!::!" & oUnionPgon.TopoID)
				'End If

				Try
					If dicOverlayPgons.ContainsKey(oOverlayPgon.TopoID) Then

					End If

					'	dicOverlayPgons.Add(oOverlayPgon.TopoID, oOverlayPgon) 04/02/13
					''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''MessageBox.Show(CStr(oOverlayPgon.Lines.Count), "05_100")
					dicOverlayPgons.Add(oOverlayPgon)
				Catch oEx As Exception
					MessageBox.Show(oEx.Message & vbCrLf & CStr(iTestCounter) & vbCrLf & CStr(oOverlayPgon.ParcelTopoID) & ":" & CStr(oOverlayPgon.TopoID) & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing), "02_114")
				End Try

				'		MessageBox.Show(CStr(oUnionPgon.ParcelTopoID) & ":" & CStr(oUnionPgon.LotTopoID & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing) & vbCrLf & CStr(oLot Is Nothing)), "02_113")
				Try
					'DMAcadExt.AcadDocument.WriteMessageLog("011 " & CStr(iParcelTopoID) & ":" & CStr(iLotTopoID))

					If oParcel IsNot Nothing Then

						'	DMAcadExt.AcadDocument.WriteMessage("51_66: i=" & CStr(iTestCounter) & "# " & CStr(oOverlayPgon.LotTopoID) & "!:!" & oOverlayPgon.ParcelTopoID & "!:!" & oOverlayPgon.TopoID)
						'	DMAcadExt.AcadDocument.WriteMessage("!51_72: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex) & "!::!" & oUnionPgon.TopoID)
						''''''''''''	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & ":" & CStr(iLotTopoID), "05_703 ")
						oParcel.AddOverlay(iOverlayIndex, iAddThemeTopoID)

						'020816    DMAcadExt.AcadDocument.WriteDebugMessage("!!Parcel: " & oParcel.BlockFull & "," & oParcel.Name & " -  Ov:" & oParcel.OverlayCount(iOverlayIndex).ToString())
						If iAddThemeTopoID <> 0 Then
							'	DMCommon.Debug.MsgBox("09_549c", sPolylineLayer, iOwnershipNoteTopoID, mbAddOverlayExists, lstPolygons.Count, oOwnershipNote)
						End If

						If oOwnershipNote IsNot Nothing AndAlso iAddThemeTopoID <> 0 Then
							''''''''''''''	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & ":" & CStr(iLotTopoID), "05_703a")
							'	oParcel.AddOwnershipNote(iAddThemeTopoID, oOwnershipNote)
						End If
						'If iParcelTopoID = 15152 Then
						'	DMAcadExt.AcadDocument.WriteMessage("!51_80: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex))
						'End If
					End If


				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "Tpr - LoadMerge_31fdo")
				End Try
				'	MessageBox.Show(mdicOverlayGroups(iOverlayIndex).GetTest6(), "02_115")

				oPolygon.Dispose()
				oPolygon = Nothing

			Next
			'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!lstPolygons", lstPolygons.Count)
			Return True


		End Function
		Public Shared Function zzLoadFDO_Overlay(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Boolean
			'Gush=29918
			'Parcelname="17"
			'ParcelTopoID=40727
			'Area= 3993
			''''''''''''''''''''''''
			'Gush=29918
			'Parcelname="35"
			'ParcelTopoID=40721
			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!zzLoadFDO_Overlay", iTopoPurpose)
			'	DMCommon.Debug.MsgBox("04_370c", "LoadFDO_Overlay", iTopoPurpose, dicLots.Count, mbMerhavExists)
			Dim dicOverlayPgons As TPlanGraph.TplnOverlayPgons
			Dim tUnionCentroid As Autodesk.AutoCAD.Geometry.Point3d
			Dim iParcelTopoID As Integer
			Dim oParcel As TplnParcel
			Dim oMerhav As TplnMerhav
			Dim iLotTopoID As Integer
			Dim oLot As TplnLot
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex
			Dim sPolylineLayer As String
			Dim iTestCounter As Integer = 0
			Dim tLayerDef As DMAcadExt.AcadLayerDef
			Dim sTableName As String

			If iTopoPurpose = enTopoPurpose.Approved Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.ApprFDO_Overlay
				If mbMerhavExists Then
					tLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.OverlayFDO_MerhLotKParcel)
				Else
					tLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.OverlayFDO_LotKParcel)
				End If

				sPolylineLayer = tLayerDef.Name
				sTableName = "LotKParcel"
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.PropFDO_Overlay
				tLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.OverlayFDO_LotMParcel)
				sPolylineLayer = tLayerDef.Name

				sTableName = "LotMParcel"
			Else
				System.Windows.Forms.MessageBox.Show("", "LoadFDO_1317")
				Return False
			End If


			'''''''''''''''''''''''????????????   Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.CreateLayer(tLayerDef, True)

			mdicOverlayGroups(iOverlayIndex) = New TPlanGraph.TplnOverlayGroups
			''''''''''''''''''''''''''''''''''''''''''''' Merhav Only !!!    sPolylineLayer = "MerhLotKParcel"
			''''Isprav
			If mbMerhavExists Then
				mbAddOverlayExists = True
			End If

			Dim oOverlayPgon As TplnOverlayPgon
			Dim oLotPgon As Polygon = Nothing
			Dim oParcelPgon As Polygon = Nothing
			Dim dicPolygons As IDictionary(Of Integer, DMAcadExt.MPolygonOverlay)
			Dim iMerhavTopoID As Integer
			Dim sLotName As String
			Dim iRegionNo As Integer
			Dim oRegion As TplnRegion = Nothing
			Dim sTest As String
			If iTopoPurpose = enTopoPurpose.Approved Then
				mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprFDO_Overlay) = New TPlanGraph.TplnOverlayPgons
				dicOverlayPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
				mdicUnionGroupsAppr = New TplnUnionGroups()
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropFDO_Overlay) = New TPlanGraph.TplnOverlayPgons
				dicOverlayPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
				mdicUnionGroupsProp = New TplnUnionGroups()
			Else
				System.Windows.Forms.MessageBox.Show("Err 11511", "Tpr - LoadFDO_Overlay_55", MessageBoxButtons.OK, MessageBoxIcon.Stop)
				Return False
			End If


			Dim oResBuffer As ResultBuffer = Nothing
			Dim iFeatureID As Integer

			Dim dP_Area As Double
			Dim iGush As Integer

			Dim iParcel As Integer

			dicPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines2MPolygons(sPolylineLayer, XDataAppName, mbAddOverlayExists)
			'130126	DMCommon.Debug.ExcelLog.SetNextValue(0, "!lstPolygons", "zzLoadFDO_Overlay", dicPolygons.Count, sPolylineLayer, mbAddOverlayExists)
			If dicPolygons.Count = 0 Then
				System.Windows.Forms.MessageBox.Show("Layer '" & sPolylineLayer & "'" & vbCrLf & CStr(dicPolygons.Count) & " --- ", "Pgon Count 04_150")
			End If
			TplnProject.WriteMessageBox(CStr(dicPolygons.Count), "מספר פוליגונים בחיתוך:!!!!!!!!!")

			'	Dim dicTestEntities As ObjectIdCollection = New ObjectIdCollection()
			Dim sMsgText As String

			Dim oBoundingBox As DMAcadExt.TPlnBoundingBox ' = New DMAcadExt.TPlnBoundingBox(oPolygon.GeometricExtents)
			Dim sSysID As String = "PNE," & CStr(iLotTopoID)
			Dim oCenter As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(tUnionCentroid)

			For Each oPolygon As DMAcadExt.MPolygonOverlay In dicPolygons.Values
				oParcel = Nothing
				oLot = Nothing
				iFeatureID = oPolygon.FeatureID
				iParcelTopoID = oPolygon.SourceID
				iLotTopoID = oPolygon.OverlayID

				sLotName = String.Empty
				iRegionNo = -1
				If mbMerhavExists Then
					iMerhavTopoID = oPolygon.OverlayID_Add
				End If


				oOverlayPgon = New TplnOverlayPgon(oPolygon, oPolygon.Entities, iFeatureID, DMAcadExt.enOverlayMethod.FDO_Overlay, iTopoPurpose)
				If iParcelTopoID = 0 Then


					If dicLots.TryGetValue(iLotTopoID, oLot) Then
						sLotName = oLot.PolygonFullName
						iRegionNo = oLot.RegionNo
						oCenter = oLot.GetCentroid()
					Else
						oLot = Nothing
					End If
					sMsgText = sLotName & " #" & CStr(iLotTopoID) & ": " & "חלקה לא קיימת"
					oBoundingBox = New DMAcadExt.TPlnBoundingBox(oPolygon.GeometricExtents)

					DMAcadExt.AppMessages.AddMessage(True, oCenter, oBoundingBox, sSysID, sMsgText, False)
				ElseIf mdicParcels.ContainsKey(iParcelTopoID) Then
					oParcel = mdicParcels.Item(iParcelTopoID)
					oOverlayPgon.BlockNo = oParcel.BlockNo
					oOverlayPgon.BlockAddNo = oParcel.BlockAdd

					If iLotTopoID = 0 Then
						oParcel.OutPlan(iOverlayIndex) = True
					ElseIf dicLots.ContainsKey(iLotTopoID) Then
						oLot = dicLots.Item(iLotTopoID)

						If oLot.InPlan Then
							oParcel.InPlan(iOverlayIndex) = True
							oOverlayPgon.LotOut = False
							oOverlayPgon.LotGroupID = oLot.RegionNo
							oOverlayPgon.LanduseID = oLot.LanduseID
							iRegionNo = oLot.RegionNo
							If mdicRegions.Count > 1 AndAlso mdicRegions.TryGetValue(iRegionNo, oRegion) Then
								'	oRegion.AddOverlayPgon(DMAcadExt.enOverlayIndex.ApprFDO_Overlay, oOverlayPgon)

							End If

						Else
							oParcel.OutPlan(iOverlayIndex) = True
							oOverlayPgon.LotOut = True
						End If

						If mbMerhavExists AndAlso iMerhavTopoID <> 0 Then
							oParcel.SetMerhav(iOverlayIndex, iMerhavTopoID)
						End If

						oLot.AddOverlayPgon(DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)

					Else
						oParcel.OutPlan(iOverlayIndex) = True

					End If


					If oParcel IsNot Nothing Then
						dP_Area = oParcel.AcadArea(False)
						iGush = oParcel.BlockNo
						iParcel = oParcel.ParcelNo
					Else
						iGush = 0
						iParcel = 0
					End If
					Dim dL_Area As Double

					If oLot IsNot Nothing Then
						dL_Area = oLot.AcadArea(False)
						sLotName = oLot.Name
					Else
						sLotName = String.Empty
					End If
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!Lot.AddOverlayPgon", oParcel.BlockFull, oParcel.Name, oParcel.LegalArea(False), oParcel.TopoID, iLotTopoID, sLotName, oOverlayPgon.LotTopoID, oOverlayPgon.AcadArea(False))
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!LoadFDO", iFeatureID, iParcelTopoID, iLotTopoID, oPolygon.Area, dP_Area, dL_Area, iGush, iParcel, sLotName)
					oParcel.AddOverlayPgon(iTopoPurpose, DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)

					If iLotTopoID <> 0 AndAlso dicLots.ContainsKey(iLotTopoID) Then

						If mbMerhavExists AndAlso iMerhavTopoID <> 0 Then
							If mdicMerhav IsNot Nothing Then
								oMerhav = mdicMerhav.Item(iMerhavTopoID)

								'  DMCommon.ExcelLogY.SetNextValue(0, iFeatureID, iParcelTopoID, iLotTopoID, iMerhavTopoID, oPolygon.Area, oParcel.BlockNo, oParcel.BlockAdd, oParcel.BlockFull, oParcel.Name, oParcel.LegalArea(False), oParcel.TopoID, oLot.Name, oLot.TopoID, oMerhav.Code, oMerhav.Name, oMerhav.TopoID)
								If oMerhav IsNot Nothing Then
									oMerhav.AddParcel(oParcel)
									'DMCommon.Debug.ExcelLog.SetNextValue(0, "Merh", iMerhavTopoID, oMerhav.Code, oMerhav.InputName, oParcel.BlockFull, oParcel.ParcelNo)
									'	DMAcadExt.AcadDocument.WriteMessage("@@@4 " & iMerhavTopoID.ToString() & ":" & CStr(oParcel.BlockKey) & "," & CStr(oMerhav.BlockCount), "04_372z")
								End If
							Else
								'MessageBox.Show("mdicMerhav Is Nothing", "09_125")
							End If
						End If
					End If
				Else
					DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay " & "Parcel:" & CStr(iParcelTopoID) & "- was not found", "LoadFDO_Overlay")
					oParcel = Nothing
				End If
				'	DMCommon.Debug.ExcelLog.SetNextValue(0, "!LoadFDO", iFeatureID, iParcelTopoID, iLotTopoID, oPolygon.Area, sLotName, iGush, iParcel, oPolygon.FirstHandle)
				Try
					dicOverlayPgons.Add(oOverlayPgon)
				Catch oEx As Exception
					MessageBox.Show(oEx.Message & vbCrLf & CStr(iTestCounter) & vbCrLf & CStr(oOverlayPgon.ParcelTopoID) & ":" & CStr(oOverlayPgon.TopoID) & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing), "02_114")
				End Try
				sTest = "Noth0"
				Try
					If mdicOverlayGroups(iOverlayIndex).AddOverlayPgon(oOverlayPgon, "LoadFDO_Overlay") Then
						If oParcel IsNot Nothing Then
							oParcel.AddOverlay(iOverlayIndex, iLotTopoID)
							sTest = "P=" & oParcel.BlockNo & "," & oParcel.ParcelNo
							If oLot IsNot Nothing AndAlso iLotTopoID <> 0 Then
								oParcel.AddLot(iTopoPurpose, DMAcadExt.enOverlayMethod.FDO_Overlay, iLotTopoID, oLot.InPlan, oLot.RegionNo, oLot.LanduseID)
							End If
						Else
							sTest = "P=0"
						End If
						If (iLotTopoID <> 0) AndAlso (oLot IsNot Nothing) Then
							oLot.AddOverlay(iOverlayIndex, iParcelTopoID)
							sTest &= "; " & "L=" & oLot.Name
						Else
							sTest &= "; " & "L=0"
						End If
					Else
						sTest = oOverlayPgon.ParcelTopoID & "," & oOverlayPgon.LotTopoID & "C=" & oOverlayPgon.CenterPosition.Coordinates2d
					End If
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!LoadOver", iParcelTopoID, iLotTopoID, oOverlayPgon.AcadArea(False), sTest, mdicOverlayGroups(iOverlayIndex).Count)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "Tpr - LoadMerge_31fdo")
				End Try

				Try
					If iTopoPurpose = enTopoPurpose.Approved Then
						mdicUnionGroupsAppr.AddUnionPgon(enOverlayMethod.Merge, oOverlayPgon)
					ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
						mdicUnionGroupsProp.AddUnionPgon(enOverlayMethod.Merge, oOverlayPgon)
					End If
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "Tpr - LoadMerge_3")
				End Try
				oPolygon.Dispose()
				oPolygon = Nothing

			Next oPolygon  'MPolygonOverlay

			Return True
		End Function
		Private Shared Sub zzOverlayToExcel(iOverlayIndex As DMAcadExt.enOverlayIndex)
			Dim oLot As TplnLot = Nothing
			Dim sLotName As String
			Dim oParcel As TplnParcel = Nothing
			Dim iParcelNo As Integer
			Dim dParcelLegalArea As Double
			Dim dParcelAcadArea As Double

			'Dim iDataOption As TPlanGraph.enDataOptions
			For Each oOverlayGroup As TplnOverlayGroup In mdicOverlayGroups(iOverlayIndex).Values
				If mdicParcels.TryGetValue(oOverlayGroup.ParcelID, oParcel) Then
					iParcelNo = oParcel.ParcelNo
					'Dim iDataOption As TPlanGraph.enDataOptions
					'Dim iDataOption As TPlanGraph.enDataOptions
					dParcelLegalArea = oParcel.LegalArea(False)
					dParcelAcadArea = oParcel.AcadArea(False)
				Else
					iParcelNo = 0
				End If
				If mdicLotsAppr.TryGetValue(oOverlayGroup.LotID, oLot) Then
					sLotName = oLot.Name
				Else
					sLotName = ""
				End If
				'130126 	DMCommon.Debug.ExcelLog.SetNextValue(0, "!!Overlay", oOverlayGroup.BlockNo, iParcelNo, dParcelLegalArea, dParcelAcadArea, sLotName, oOverlayGroup.GroupID, oOverlayGroup.GetOptionArea(TPlanGraph.enDataOptions.AcadArea)  , oOverlayGroup.GetOptionArea(TPlanGraph.enDataOptions.CalcMergeArea), oOverlayGroup.GetOptionArea(TPlanGraph.enDataOptions.CalcMergeArea2))
			Next
		End Sub
		Public Shared Function LoadFDO_OverlayOldVer(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Boolean
			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			Dim dicOverlayPgons As TPlanGraph.TplnOverlayPgons
			Dim tUnionCentroid As Autodesk.AutoCAD.Geometry.Point3d
			Dim iParcelTopoID As Integer
			Dim oParcel As TplnParcel
			Dim iLotTopoID As Integer
			Dim oLot As TplnLot
			Dim sLotTopoName As String
			Dim tLotTopoDefID As DMAcadExt.TopoDefID = New DMAcadExt.TopoDefID(iTopoPurpose)
			Dim oLotTopoDef As DMAcadExt.TopoDef = TopoManager.TopoDefs.Item(tLotTopoDefID)
			Dim oLotAdditionalTopoDef As DMAcadExt.TopoDef = TopoManager.TopoDefs.Item(tLotTopoDefID.AdditionalID)
			Dim sParcelTopoName As String
			Dim tParcelTopoDefID As DMAcadExt.TopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Parcel)
			Dim oParcelTopoDef As DMAcadExt.TopoDef = TopoManager.TopoDefs.Item(tParcelTopoDefID)
			Dim oParcelAdditionalTopoDef As DMAcadExt.TopoDef = TopoManager.TopoDefs.Item(tParcelTopoDefID.AdditionalID)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex
			Dim sPolylineLayer As String
			Dim iTestCounter As Integer = 0
			Dim tLayerDef As DMAcadExt.AcadLayerDef
			Dim sTableName As String
			If iTopoPurpose = enTopoPurpose.Approved Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.ApprFDO_Overlay
				tLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.OverlayFDO_LotKParcel)
				sTableName = "LotKParcel"
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.PropFDO_Overlay
				tLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.OverlayFDO_LotMParcel)
				sTableName = "LotMParcel"
			Else
				System.Windows.Forms.MessageBox.Show("", "LoadFDO_1317")
				Return False
			End If
			Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.CreateLayer(tLayerDef, True)
			sPolylineLayer = tLayerDef.Name
			mdicOverlayGroups(iOverlayIndex) = New TPlanGraph.TplnOverlayGroups


			If DMAcadExt.AcadTransaction.LinkExists(oLotAdditionalTopoDef.LinkLayers) Then
				sLotTopoName = oLotAdditionalTopoDef.Name
			Else
				sLotTopoName = oLotTopoDef.Name
			End If


			If DMAcadExt.AcadTransaction.LinkExists(oParcelAdditionalTopoDef.LinkLayers) Then
				sParcelTopoName = oParcelAdditionalTopoDef.Name
			Else
				sParcelTopoName = oParcelTopoDef.Name
			End If
			''''Isprav



			Dim oOverlayPgon As TplnOverlayPgon
			Dim oLotPgon As Polygon = Nothing
			Dim oParcelPgon As Polygon = Nothing
			Dim lstPolygons As IDictionary(Of Integer, DMAcadExt.MPolygonOverlay)


			If iTopoPurpose = enTopoPurpose.Proposed Then
				mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropFDO_Overlay) = New TPlanGraph.TplnOverlayPgons
				dicOverlayPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
				mdicUnionGroupsProp = New TplnUnionGroups()
			ElseIf iTopoPurpose = enTopoPurpose.Approved Then
				mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprFDO_Overlay) = New TPlanGraph.TplnOverlayPgons
				dicOverlayPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
				mdicUnionGroupsAppr = New TplnUnionGroups()
			Else
				System.Windows.Forms.MessageBox.Show("Err 11511", "Tpr - LoadFDO_Overlay_55", MessageBoxButtons.OK, MessageBoxIcon.Stop)
				Return False
			End If
			''''''''''''''''''''''''''''''''''''''''''''''''''
			''''''''''''''''''''''''''''''''''''
			Dim oResBuffer As ResultBuffer = Nothing
			'	Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
			'	Dim oLinkTable As DMAcadExt.ODTable

			'	Dim sHandle As String
			Dim iFeatureID As Integer

			lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines2MPolygons(sPolylineLayer, XDataAppName, mbAddOverlayExists)
			If lstPolygons.Count = 0 Then
				System.Windows.Forms.MessageBox.Show(CStr(lstPolygons.Count), "04_150a")
			End If
			TplnProject.WriteMessageBox(CStr(lstPolygons.Count), "מספר פוליגונים בחיתוך:")
			'	Dim sMsg As String = ""
			For Each oPolygon As DMAcadExt.MPolygonOverlay In lstPolygons.Values
				oParcel = Nothing
				oLot = Nothing
				iFeatureID = oPolygon.FeatureID
				iParcelTopoID = oPolygon.SourceID
				iLotTopoID = oPolygon.OverlayID

				iTestCounter += 1
				'	sHandle = oPolygon.Handle.ToString()

				If True Then
					'		System.Windows.Forms.MessageBox.Show(CStr(dValue) & vbCrLf & oODRec.Item(1).Type.ToString(), "04_300")
					'	iLotTopoID()
					'iParcelTopoID
					'	System.Windows.Forms.MessageBox.Show(CStr(iLotTopoID) & vbCrLf & CStr(iParcelTopoID), "04_256")
					oOverlayPgon = New TplnOverlayPgon(oPolygon, oPolygon.Entities, iFeatureID, DMAcadExt.enOverlayMethod.FDO_Overlay, iTopoPurpose)
					' 
					If True Then
						If iParcelTopoID = 0 Then
							Dim sMsgText As String
							Dim sLotName As String = String.Empty
							Dim oBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox(oPolygon.GeometricExtents)
							Dim sSysID As String = "PNE," & CStr(iLotTopoID)
							Dim oCenter As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(tUnionCentroid)
							'		DMAcadExt.AcadDocument.WriteMessage(sTestA)
							If dicLots.TryGetValue(iLotTopoID, oLot) Then
								sLotName = oLot.PolygonFullName
								oCenter = oLot.GetCentroid()
								'	DMAcadExt.AcadDocument.WriteMessage("!39_08: " & CStr(oCenter.X) & ", " & CStr(oCenter.Y))
							End If
							sMsgText = sLotName & " #" & CStr(iLotTopoID) & ": " & "חלקה לא קיימת"
							DMAcadExt.AppMessages.AddMessage(True, oCenter, oBoundingBox, sSysID, sMsgText, False)

						ElseIf mdicParcels.ContainsKey(iParcelTopoID) Then
							oParcel = mdicParcels.Item(iParcelTopoID)
							'If iParcelTopoID = 15152 Then
							'	DMAcadExt.AcadDocument.WriteMessage("!51_13: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex))
							'End If
							If iLotTopoID = 0 Then
								oParcel.OutPlan(iOverlayIndex) = True
							ElseIf dicLots.ContainsKey(iLotTopoID) Then
								oLot = dicLots.Item(iLotTopoID)
								If oLot.InPlan Then
									oParcel.InPlan(iOverlayIndex) = True
									oOverlayPgon.LotOut = False
								Else
									oParcel.OutPlan(iOverlayIndex) = True
									oOverlayPgon.LotOut = True
								End If
								'DMAcadExt.AcadDocument.WriteMessage("!51_34: i=" & CStr(iTestCounter) & "# " & oOverlayPgon.LotTopoID & "||" & CStr(oOverlayPgon.ParcelTopoID))	'oParcel.TestOverlayList(iOverlayIndex) & ":" & 
								'	System.Windows.Forms.MessageBox.Show(CStr(oOverlayPgon.LotTopoID) & vbCrLf & CStr(oOverlayPgon.ParcelTopoID) & vbCrLf & CStr(oLot.TopoID), "04_257")
								oLot.AddOverlayPgon(DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)
							Else
								oParcel.OutPlan = True
								DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus) & vbCrLf & " - was not found", "LoadFDO_Overlay" & iTopoPurpose.ToString())
							End If
							'	DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay! " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus))
							oParcel.AddOverlayPgon(iTopoPurpose, DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)
						Else
							DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay " & "Parcel:" & CStr(iParcelTopoID) & "- was not found", "LoadFDO_Overlay")
						End If
						'If iParcelTopoID = 15152 Then
						'	DMAcadExt.AcadDocument.WriteMessage("51_12: " & CStr(oUnionPgon.LotTopoID) & "!::!" & oUnionPgon.TopoID)
						'End If
						If iOverlayIndex = DMAcadExt.enOverlayIndex.PropFDO_Overlay Then
							''''''''''''	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & ":" & CStr(iLotTopoID), "05_704")
						End If
						Try
							If dicOverlayPgons.ContainsKey(oOverlayPgon.TopoID) Then

							End If

							'	dicOverlayPgons.Add(oOverlayPgon.TopoID, oOverlayPgon) 04/02/13
							dicOverlayPgons.Add(oOverlayPgon)
						Catch oEx As Exception
							MessageBox.Show(oEx.Message & vbCrLf & CStr(iTestCounter) & vbCrLf & CStr(oOverlayPgon.ParcelTopoID) & ":" & CStr(oOverlayPgon.TopoID) & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing), "02_114")
						End Try

						'		MessageBox.Show(CStr(oUnionPgon.ParcelTopoID) & ":" & CStr(oUnionPgon.LotTopoID & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing) & vbCrLf & CStr(oLot Is Nothing)), "02_113")
						Try
							'DMAcadExt.AcadDocument.WriteMessageLog("011 " & CStr(iParcelTopoID) & ":" & CStr(iLotTopoID))
							If mdicOverlayGroups(iOverlayIndex).AddOverlayPgon(oOverlayPgon, "LoadFDO_OverlayOldVer") Then
								If oParcel IsNot Nothing Then

									'	DMAcadExt.AcadDocument.WriteMessage("51_66: i=" & CStr(iTestCounter) & "# " & CStr(oOverlayPgon.LotTopoID) & "!:!" & oOverlayPgon.ParcelTopoID & "!:!" & oOverlayPgon.TopoID)
									'	DMAcadExt.AcadDocument.WriteMessage("!51_72: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex) & "!::!" & oUnionPgon.TopoID)
									''''''''''''	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & ":" & CStr(iLotTopoID), "05_703 ")
									oParcel.AddOverlay(iOverlayIndex, iLotTopoID)
									If iLotTopoID <> 0 Then
										''''''''''''''	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & ":" & CStr(iLotTopoID), "05_703a")
										oParcel.AddLot(iTopoPurpose, DMAcadExt.enOverlayMethod.FDO_Overlay, iLotTopoID, oLot.InPlan, oLot.RegionNo, oLot.LanduseID)
									End If
									'If iParcelTopoID = 15152 Then
									'	DMAcadExt.AcadDocument.WriteMessage("!51_80: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex))
									'End If
								End If
								If (iLotTopoID <> 0) AndAlso (oLot IsNot Nothing) Then
									oLot.AddOverlay(iOverlayIndex, iParcelTopoID)
								End If
							End If
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "Tpr - LoadMerge_31")
						End Try
						'	MessageBox.Show(mdicOverlayGroups(iOverlayIndex).GetTest6(), "02_115")
						Try
							If iTopoPurpose = enTopoPurpose.Approved Then
								mdicUnionGroupsAppr.AddUnionPgon(enOverlayMethod.Merge, oOverlayPgon)
							ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
								mdicUnionGroupsProp.AddUnionPgon(enOverlayMethod.Merge, oOverlayPgon)
							End If
						Catch oEx As Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message, "Tpr - LoadMerge_3")
						End Try
						oPolygon.Dispose()
						oPolygon = Nothing
					End If
				Else
					System.Windows.Forms.MessageBox.Show("ODRec is Nothing" & vbCrLf & CStr(iTestCounter), "04_300")
					Dim tPoint As DMAcadExt.TPlnPoint = DMAcadExt.TPlnPoint.GetEntity2dCenter(oPolygon)
					DMAcadExt.AcadDocument.WriteMessage("E23: " & CStr(tPoint.X) & "," & CStr(tPoint.Y))
					Exit For
				End If
				If iTestCounter = 380 Then
					''''''''''Exit For
				End If
			Next
			'	System.Windows.Forms.MessageBox.Show("!!", "04_160")
			'''''''''''''''''''''''''''''''


			Return True


		End Function

		Public Shared Function LoadFDO_Overlay_ByOD(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Boolean
			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			Dim dicOverlayPgons As TPlanGraph.TplnOverlayPgons
			Dim tUnionCentroid As Autodesk.AutoCAD.Geometry.Point3d
			Dim iParcelTopoID As Integer
			Dim oParcel As TplnParcel
			Dim iLotTopoID As Integer
			Dim oLot As TplnLot
			Dim sLotTopoName As String
			Dim tLotTopoDefID As DMAcadExt.TopoDefID = New DMAcadExt.TopoDefID(iTopoPurpose)
			Dim oLotTopoDef As DMAcadExt.TopoDef = TopoManager.TopoDefs.Item(tLotTopoDefID)
			Dim oLotAdditionalTopoDef As DMAcadExt.TopoDef = TopoManager.TopoDefs.Item(tLotTopoDefID.AdditionalID)
			Dim sParcelTopoName As String
			Dim tParcelTopoDefID As DMAcadExt.TopoDefID = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Parcel)
			Dim oParcelTopoDef As DMAcadExt.TopoDef = TopoManager.TopoDefs.Item(tParcelTopoDefID)
			Dim oParcelAdditionalTopoDef As DMAcadExt.TopoDef = TopoManager.TopoDefs.Item(tParcelTopoDefID.AdditionalID)
			Dim iOverlayIndex As DMAcadExt.enOverlayIndex
			Dim sPolylineLayer As String
			Dim iTestCounter As Integer = 0
			Dim tLayerDef As DMAcadExt.AcadLayerDef
			Dim sTableName As String
			If iTopoPurpose = enTopoPurpose.Approved Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.ApprFDO_Overlay
				tLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.OverlayFDO_LotKParcel)
				sTableName = "LotKParcel"
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.PropFDO_Overlay
				tLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.OverlayFDO_LotMParcel)
				sTableName = "LotMParcel"
			Else
				System.Windows.Forms.MessageBox.Show("", "LoadFDO_1317")
				Return False
			End If
			Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.CreateLayer(tLayerDef, True)
			sPolylineLayer = tLayerDef.Name
			mdicOverlayGroups(iOverlayIndex) = New TPlanGraph.TplnOverlayGroups


			If DMAcadExt.AcadTransaction.LinkExists(oLotAdditionalTopoDef.LinkLayers) Then
				sLotTopoName = oLotAdditionalTopoDef.Name
			Else
				sLotTopoName = oLotTopoDef.Name
			End If
			Dim oLotTopology As TopologyModel = DMAcadExt.AcadMapApp.GetTopology(sLotTopoName)

			If DMAcadExt.AcadTransaction.LinkExists(oParcelAdditionalTopoDef.LinkLayers) Then
				sParcelTopoName = oParcelAdditionalTopoDef.Name
			Else
				sParcelTopoName = oParcelTopoDef.Name
			End If
			''''Isprav
			Dim oParcelTopology As TopologyModel = DMAcadExt.AcadMapApp.GetTopology(sParcelTopoName)

			If oLotTopology IsNot Nothing AndAlso oParcelTopology IsNot Nothing Then
				DMAcadExt.AcadDocument.WriteDebugMessage("!!!" & sParcelTopoName & "," & oParcelTopology.Status.ToString() & "," & oLotTopology.Status.ToString())
				If oLotTopology.Status = Status.Closed Then
					Try
						oLotTopology.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "Tpr - LoadFDO_Overlay_1", oLotTopology.Name)
						System.Windows.Forms.MessageBox.Show("", "LoadFDO_1318")
						Return False
					End Try
				End If

				If oParcelTopology.Status = Status.Closed Then
					Try
						oParcelTopology.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
					Catch oMapEx As Autodesk.Gis.Map.MapException
						DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, True, "Tpr - LoadFDO_Overlay_2")
						oLotTopology.Close()
						System.Windows.Forms.MessageBox.Show("", "LoadFDO_1319")
						Return False
					End Try
				End If

				Dim oOverlayPgon As TplnOverlayPgon
				Dim oLotPgon As Polygon = Nothing
				Dim oParcelPgon As Polygon = Nothing
				Dim lstPolylines As System.Collections.Generic.IList(Of Polyline)


				If iTopoPurpose = enTopoPurpose.Proposed Then
					mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropFDO_Overlay) = New TPlanGraph.TplnOverlayPgons
					dicOverlayPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
					mdicUnionGroupsProp = New TplnUnionGroups()
				ElseIf iTopoPurpose = enTopoPurpose.Approved Then
					mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprFDO_Overlay) = New TPlanGraph.TplnOverlayPgons
					dicOverlayPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
					mdicUnionGroupsAppr = New TplnUnionGroups()
				Else
					System.Windows.Forms.MessageBox.Show("Err 11511", "Tpr - LoadFDO_Overlay_55", MessageBoxButtons.OK, MessageBoxIcon.Stop)
					Return False
				End If
				''''''''''''''''''''''''''''''''''''''''''''''''''
				''''''''''''''''''''''''''''''''''''
				Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
				Dim oLinkTable As DMAcadExt.ODTable
				Dim dValue As Double
				Dim sHandle As String
				oLinkTable = New DMAcadExt.ODTable(sTableName)
				If oLinkTable.Exists Then


					lstPolylines = DMAcadExt.AcadTransaction.GetAcadPolylines(sPolylineLayer, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)

					System.Windows.Forms.MessageBox.Show(CStr(lstPolylines.Count), "04_150f")
					''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
					For Each oPolygon As Polyline In lstPolylines

						oParcel = Nothing
						oLot = Nothing
						iTestCounter += 1
						sHandle = oPolygon.Handle.ToString()
						Try
							oODRec = oLinkTable.GetODRecord(oPolygon.ObjectId)

						Catch oMapEx As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "Tpr - LoadFDO_Overlay_9")
							System.Windows.Forms.MessageBox.Show(oMapEx.Message & vbCrLf & sTableName, "04_207s")
						End Try
						If oODRec IsNot Nothing Then
							'	oODRec.Init()
							Dim iFeatureID As Integer
							Try
								dValue = oODRec.Item(0).DoubleValue
								iFeatureID = Convert.ToInt32(dValue)
							Catch oEx As Exception

								System.Windows.Forms.MessageBox.Show(oEx.Message, "04_208")
							End Try
							Try
								dValue = oODRec.Item(1).DoubleValue
								iParcelTopoID = Convert.ToInt32(dValue)
							Catch oEx As Exception
								iParcelTopoID = 0
								System.Windows.Forms.MessageBox.Show(oEx.Message, "04_210")
							End Try

							Try
								dValue = oODRec.Item(3).DoubleValue
								iLotTopoID = Convert.ToInt32(dValue)
							Catch oEx As Exception
								iLotTopoID = 0
								System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oODRec.Item(3).ToString(), "04_220")
							End Try
							'		System.Windows.Forms.MessageBox.Show(CStr(dValue) & vbCrLf & oODRec.Item(1).Type.ToString(), "04_300")
							'	iLotTopoID()
							'iParcelTopoID
							If iTestCounter > 1200 Then
								DMAcadExt.AcadDocument.WriteMessage("46** " & CStr(iFeatureID) & ":" & CStr(iLotTopoID) & ":" & CStr(iParcelTopoID))
							End If
							'	System.Windows.Forms.MessageBox.Show(CStr(iLotTopoID) & vbCrLf & CStr(iParcelTopoID), "04_256")
							oOverlayPgon = New TplnOverlayPgon(oPolygon, iFeatureID, DMAcadExt.enOverlayMethod.FDO_Overlay, iTopoPurpose, iLotTopoID, iParcelTopoID)

							' 
							If True Then
								If iParcelTopoID = 0 Then
									Dim sTestA As String = "Parcel = 0 => " & CStr(iLotTopoID)
									sTestA &= ":" & CStr(tUnionCentroid.X) & "," & tUnionCentroid.Y ''& vbCrLf
									DMAcadExt.AcadDocument.WriteMessage(sTestA)
								ElseIf mdicParcels.ContainsKey(iParcelTopoID) Then
									oParcel = mdicParcels.Item(iParcelTopoID)
									'If iParcelTopoID = 15152 Then
									'	DMAcadExt.AcadDocument.WriteMessage("!51_13: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex))
									'End If
									If iLotTopoID = 0 Then
										oParcel.OutPlan(iOverlayIndex) = True
									ElseIf dicLots.ContainsKey(iLotTopoID) Then
										oLot = dicLots.Item(iLotTopoID)
										If oLot.InPlan Then
											oParcel.InPlan(iOverlayIndex) = True
											oOverlayPgon.LotOut = False
										Else
											oParcel.OutPlan(iOverlayIndex) = True
											oOverlayPgon.LotOut = True
										End If
										'DMAcadExt.AcadDocument.WriteMessage("!51_34: i=" & CStr(iTestCounter) & "# " & oOverlayPgon.LotTopoID & "||" & CStr(oOverlayPgon.ParcelTopoID))	'oParcel.TestOverlayList(iOverlayIndex) & ":" & 
										'	System.Windows.Forms.MessageBox.Show(CStr(oOverlayPgon.LotTopoID) & vbCrLf & CStr(oOverlayPgon.ParcelTopoID) & vbCrLf & CStr(oLot.TopoID), "04_257")
										oLot.AddOverlayPgon(DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)
									Else
										oParcel.OutPlan = True
										DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus) & vbCrLf & " - was not found", "LoadFDO_Overlay" & iTopoPurpose.ToString())
									End If
									'	DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay! " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus))
									oParcel.AddOverlayPgon(iTopoPurpose, DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)
								Else
									DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay " & "Parcel:" & CStr(iParcelTopoID) & "- was not found", "LoadFDO_Overlay")

								End If
								'If iParcelTopoID = 15152 Then
								'	DMAcadExt.AcadDocument.WriteMessage("51_12: " & CStr(oUnionPgon.LotTopoID) & "!::!" & oUnionPgon.TopoID)
								'End If
								If iOverlayIndex = DMAcadExt.enOverlayIndex.PropFDO_Overlay Then
									''''''''''''	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & ":" & CStr(iLotTopoID), "05_704")
								End If


								'		dicOverlayPgons.Add(oOverlayPgon.TopoID, oOverlayPgon) 04/02/13
								dicOverlayPgons.Add(oOverlayPgon)
								'		MessageBox.Show(CStr(oUnionPgon.ParcelTopoID) & ":" & CStr(oUnionPgon.LotTopoID & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing) & vbCrLf & CStr(oLot Is Nothing)), "02_117")
								Try
									'DMAcadExt.AcadDocument.WriteMessageLog("011 " & CStr(iParcelTopoID) & ":" & CStr(iLotTopoID))
									If mdicOverlayGroups(iOverlayIndex).AddOverlayPgon(oOverlayPgon, "LoadFDO_Overlay_ByOD") Then
										If oParcel IsNot Nothing Then

											'	DMAcadExt.AcadDocument.WriteMessage("51_66: i=" & CStr(iTestCounter) & "# " & CStr(oOverlayPgon.LotTopoID) & "!:!" & oOverlayPgon.ParcelTopoID & "!:!" & oOverlayPgon.TopoID)
											'	DMAcadExt.AcadDocument.WriteMessage("!51_72: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex) & "!::!" & oUnionPgon.TopoID)
											''''''''''''	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & ":" & CStr(iLotTopoID), "05_703 ")
											oParcel.AddOverlay(iOverlayIndex, iLotTopoID)
											If iLotTopoID <> 0 Then
												''''''''''''''	System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString() & ":" & CStr(iLotTopoID), "05_703a")
												oParcel.AddLot(iTopoPurpose, DMAcadExt.enOverlayMethod.FDO_Overlay, iLotTopoID, oLot.InPlan, oLot.RegionNo, oLot.LanduseID)
											End If
											'If iParcelTopoID = 15152 Then
											'	DMAcadExt.AcadDocument.WriteMessage("!51_80: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex))
											'End If
										End If
										If (iLotTopoID <> 0) AndAlso (oLot IsNot Nothing) Then
											oLot.AddOverlay(iOverlayIndex, iParcelTopoID)
										End If
									End If
								Catch oEx As Exception
									System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "Tpr - LoadMerge_31")
								End Try

								'	MessageBox.Show(mdicOverlayGroups(iOverlayIndex).GetTest6(), "02_115")
								Try
									If iTopoPurpose = enTopoPurpose.Approved Then
										mdicUnionGroupsAppr.AddUnionPgon(enOverlayMethod.Merge, oOverlayPgon)
									ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
										mdicUnionGroupsProp.AddUnionPgon(enOverlayMethod.Merge, oOverlayPgon)
									End If

								Catch oEx As Exception
									System.Windows.Forms.MessageBox.Show(oEx.Message, "Tpr - LoadMerge_3")
								End Try
								oPolygon.Dispose()
								oPolygon = Nothing
							End If
						Else
							System.Windows.Forms.MessageBox.Show("ODRec is Nothing" & vbCrLf & CStr(iTestCounter) & vbCrLf & sHandle, "04_300")
							Dim tPoint As Autodesk.AutoCAD.Geometry.Point2d = oPolygon.GetPoint2dAt(0)
							DMAcadExt.AcadDocument.WriteMessage("E23: " & CStr(tPoint.X) & "," & CStr(tPoint.Y))
							Exit For
						End If
						If iTestCounter = 380 Then
							''''''''''Exit For
						End If

					Next
				End If
				'''''''''''''''''''''''''''''''
				Try
					oLotTopology.Close()
					'	oLotTopology.Dispose()
					oLotTopology = Nothing
					oParcelTopology.Close()
					'	oParcelTopology.Dispose()
					oParcelTopology = Nothing


				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "Tpr - LoadMerge_4")
				End Try
			End If
			Return True


		End Function

		Public Shared Function LoadFDO_ParcelExproZone() As Boolean
			Dim iTopoPurpose As DMAcadExt.enTopoPurpose
			DMCommon.Debug.MsgBox("LoadFDO_ParcelExproZone", "04_992")

			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			Dim dicOverlayPgons As TPlanGraph.TplnOverlayPgons
			Dim tUnionCentroid As Autodesk.AutoCAD.Geometry.Point3d
			Dim iParcelTopoID As Integer
			Dim oParcel As TplnParcel
			Dim iLotTopoID As Integer
			Dim iExproTopoID As Integer

			Dim oLot As TplnLot
			Dim oExpro As TplnExpro

			Dim iOverlayIndex As DMAcadExt.enOverlayIndex
			Dim sPolylineLayer As String
			Dim iTestCounter As Integer = 0
			Dim tLayerDef As DMAcadExt.AcadLayerDef
			Dim sTableName As String
			If iTopoPurpose = enTopoPurpose.Approved Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.ApprFDO_Overlay
				tLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.OverlayFDO_LotKParcel)
				sTableName = "LotKParcel"
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.PropFDO_Overlay
				tLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.OverlayFDO_LotMParcel)
				sTableName = "LotMParcel"
			Else
				System.Windows.Forms.MessageBox.Show("", "LoadFDO_1317")
				Return False
			End If
			Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.CreateLayer(tLayerDef, True)
			sPolylineLayer = tLayerDef.Name
			mdicOverlayGroups(iOverlayIndex) = New TPlanGraph.TplnOverlayGroups
			mdicParcelLotExproPgons = New TplnOverlayPgons()
			''''Isprav

			Dim oOverlayPgon As TplnOverlayPgon
			Dim oLotPgon As Polygon = Nothing
			Dim oParcelPgon As Polygon = Nothing
			Dim lstPolygons As IDictionary(Of Integer, DMAcadExt.MPolygonOverlay)

			If iTopoPurpose = enTopoPurpose.Approved Then
				mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprFDO_Overlay) = New TPlanGraph.TplnOverlayPgons
				dicOverlayPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
				mdicUnionGroupsAppr = New TplnUnionGroups()
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropFDO_Overlay) = New TPlanGraph.TplnOverlayPgons
				dicOverlayPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
				mdicUnionGroupsProp = New TplnUnionGroups()
			Else
				System.Windows.Forms.MessageBox.Show("Err 11511", "Tpr - LoadFDO_Overlay_55", MessageBoxButtons.OK, MessageBoxIcon.Stop)
				Return False
			End If
			''''''''''''''''''''''''''''''''''''''''''''''''''
			''''''''''''''''''''''''''''''''''''
			Dim oResBuffer As ResultBuffer = Nothing
			'	Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
			'	Dim oLinkTable As DMAcadExt.ODTable

			'	Dim sHandle As String
			Dim iFeatureID As Integer
			mbAddOverlayExists = True
			sPolylineLayer = "ExpLotKParcel"
			sPolylineLayer = "ZoneExpParcel"

			lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines2MPolygons(sPolylineLayer, XDataAppName, mbAddOverlayExists)

			TplnProject.WriteMessageBox(CStr(lstPolygons.Count), "מספר פוליגונים בחיתוך:")
			'	Dim sMsg As String = ""
			' System.Windows.Forms.MessageBox.Show("lstPolygons.Count" & vbCrLf & CStr(lstPolygons.Count), "04_214")

			DMCommon.Debug.MsgBox("08_229", DMCommon.Debug.ColCount(mdicZones), DMCommon.Debug.ColCount(dicLots), DMCommon.Debug.ColCount(mdicExpros), DMCommon.Debug.ColCount(mdicParcels))
			For Each oPolygon As DMAcadExt.MPolygonOverlay In lstPolygons.Values
				oParcel = Nothing
				oLot = Nothing
				iFeatureID = oPolygon.FeatureID
				iParcelTopoID = oPolygon.SourceID
				iLotTopoID = oPolygon.OverlayID
				iExproTopoID = oPolygon.OverlayID_Add
				iTestCounter += 1
				'	sHandle = oPolygon.Handle.ToString()

				If True Then
					'		System.Windows.Forms.MessageBox.Show(CStr(dValue) & vbCrLf & oODRec.Item(1).Type.ToString(), "04_300")
					'	iLotTopoID()
					'iParcelTopoID
					'      System.Windows.Forms.MessageBox.Show(CStr(iLotTopoID) & vbCrLf & CStr(iParcelTopoID), "04_256")
					oOverlayPgon = New TplnOverlayPgon(oPolygon, oPolygon.Entities, iFeatureID, DMAcadExt.enOverlayMethod.FDO_Overlay, iTopoPurpose)
					' 
					mdicParcelLotExproPgons.Add(oOverlayPgon)

					If True Then
						oExpro = Nothing
						If iParcelTopoID = 0 Then
							Dim sMsgText As String
							Dim sLotName As String = String.Empty
							Dim oBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox(oPolygon.GeometricExtents)
							Dim sSysID As String = "PNE," & CStr(iLotTopoID)
							Dim oCenter As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(tUnionCentroid)
							'		DMAcadExt.AcadDocument.WriteMessage(sTestA)
							If dicLots.TryGetValue(iLotTopoID, oLot) Then
								sLotName = oLot.PolygonFullName
								oCenter = oLot.GetCentroid()
								'	DMAcadExt.AcadDocument.WriteMessage("!39_08: " & CStr(oCenter.X) & ", " & CStr(oCenter.Y))
							End If
							sMsgText = sLotName & " #" & CStr(iLotTopoID) & ": " & "חלקה לא קיימת"
							DMAcadExt.AppMessages.AddMessage(True, oCenter, oBoundingBox, sSysID, sMsgText, False)

						ElseIf mdicParcels.ContainsKey(iParcelTopoID) Then
							oParcel = mdicParcels.Item(iParcelTopoID)
							'If iParcelTopoID = 15152 Then
							'	DMAcadExt.AcadDocument.WriteMessage("!51_13: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex))
							'End If
							If mdicExpros.ContainsKey(iExproTopoID) Then
								oExpro = mdicExpros.Item(iExproTopoID)
								oParcel.AddExproPgon(oExpro, oPolygon.Area)
							End If
							oParcel.AddOverlayPgon(iTopoPurpose, DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)
							'  TplnProject.vb:line 4467
							If iLotTopoID = 0 Then
								oParcel.OutPlan(iOverlayIndex) = True
							ElseIf dicLots.ContainsKey(iLotTopoID) Then
								oLot = dicLots.Item(iLotTopoID)
								If oLot.InPlan Then
									oParcel.InPlan(iOverlayIndex) = True
									oOverlayPgon.LotOut = False
								Else
									oParcel.OutPlan(iOverlayIndex) = True
									oOverlayPgon.LotOut = True
								End If
								'DMAcadExt.AcadDocument.WriteMessage("!51_34: i=" & CStr(iTestCounter) & "# " & oOverlayPgon.LotTopoID & "||" & CStr(oOverlayPgon.ParcelTopoID))	'oParcel.TestOverlayList(iOverlayIndex) & ":" & 
								'	System.Windows.Forms.MessageBox.Show(CStr(oOverlayPgon.LotTopoID) & vbCrLf & CStr(oOverlayPgon.ParcelTopoID) & vbCrLf & CStr(oLot.TopoID), "04_257")
								oLot.AddOverlayPgon(DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)
							Else
								oParcel.OutPlan = True
								DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus) & vbCrLf & " - was not found", "LoadFDO_Overlay" & iTopoPurpose.ToString())
							End If
							'	DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay! " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus))
							oParcel.AddOverlayPgon(iTopoPurpose, DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)
						Else
							DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay " & "Parcel:" & CStr(iParcelTopoID) & "- was not found", "LoadFDO_Overlay")
						End If

						'If iParcelTopoID = 15152 Then
						'	DMAcadExt.AcadDocument.WriteMessage("51_12: " & CStr(oUnionPgon.LotTopoID) & "!::!" & oUnionPgon.TopoID)
						'End If

						Try


							'	dicOverlayPgons.Add(oOverlayPgon.TopoID, oOverlayPgon) 04/02/13
							''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''MessageBox.Show(CStr(oOverlayPgon.Lines.Count), "05_100")
							dicOverlayPgons.Add(oOverlayPgon)
						Catch oEx As Exception
							MessageBox.Show(oEx.Message & vbCrLf & CStr(iTestCounter) & vbCrLf & CStr(oOverlayPgon.ParcelTopoID) & ":" & CStr(oOverlayPgon.TopoID) & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing), "02_114")
						End Try

						'		MessageBox.Show(CStr(oUnionPgon.ParcelTopoID) & ":" & CStr(oUnionPgon.LotTopoID & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing) & vbCrLf & CStr(oLot Is Nothing)), "02_113")

						'	MessageBox.Show(mdicOverlayGroups(iOverlayIndex).GetTest6(), "02_115")

						oPolygon.Dispose()
						oPolygon = Nothing
					End If
				Else
					System.Windows.Forms.MessageBox.Show("ODRec is Nothing" & vbCrLf & CStr(iTestCounter), "04_300")
					Dim tPoint As DMAcadExt.TPlnPoint = DMAcadExt.TPlnPoint.GetEntity2dCenter(oPolygon)
					DMAcadExt.AcadDocument.WriteMessage("E23: " & CStr(tPoint.X) & "," & CStr(tPoint.Y))
					Exit For
				End If
				If iTestCounter = 380 Then
					''''''''''Exit For
				End If
			Next
			'	System.Windows.Forms.MessageBox.Show("!!", "04_160")
			'''''''''''''''''''''''''''''''
			TplnOverlayPgon.CreateMainDataTable()
			'System.Windows.Forms.MessageBox.Show("mdicParcelLotExproPgons.Count" & vbCrLf & CStr(mdicParcelLotExproPgons.Count), "04_217")
			For Each oOverlayPgon In mdicParcelLotExproPgons.Values
				oOverlayPgon.AddDataToMainTable()
			Next
			'	System.Windows.Forms.MessageBox.Show("mdicParcelLotExproPgons.Count" & vbCrLf & CStr(TplnOverlayPgon.MainDataTable.Rows.Count) & vbCrLf & CStr(TplnOverlayPgon.MainView.Count), "04_218")

			Return True


		End Function
		Public Shared Function LoadFDO_ExproOverlay_New(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Boolean
			DMCommon.Debug.MsgBox("04_998c", "LoadFDO_ExproOverlay_New", iTopoPurpose)
			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			Dim dicOverlayPgons As TPlanGraph.TplnOverlayPgons
			Dim tUnionCentroid As Autodesk.AutoCAD.Geometry.Point3d
			Dim iParcelTopoID As Integer
			Dim oParcel As TplnParcel
			Dim iLotTopoID As Integer
			Dim iExproTopoID As Integer

			Dim oLot As TplnLot
			Dim oExpro As TplnExpro

			Dim iOverlayIndex As DMAcadExt.enOverlayIndex
			Dim sPolylineLayer As String
			Dim iTestCounter As Integer = 0
			Dim tLayerDef As DMAcadExt.AcadLayerDef
			Dim sTableName As String
			If dicLots Is Nothing Then
				Return True
			End If
			If iTopoPurpose = enTopoPurpose.Approved Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.ApprFDO_Overlay
				tLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.OverlayFDO_LotKParcel)
				sTableName = "LotKParcel"
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.PropFDO_Overlay
				tLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.OverlayFDO_LotMParcel)
				sTableName = "LotMParcel"
			Else
				System.Windows.Forms.MessageBox.Show("", "LoadFDO_1317")
				Return False
			End If
			Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.CreateLayer(tLayerDef, True)
			sPolylineLayer = tLayerDef.Name
			mdicOverlayGroups(iOverlayIndex) = New TPlanGraph.TplnOverlayGroups
			mdicParcelLotExproPgons = New TplnOverlayPgons()
			''''Isprav

			Dim oOverlayPgon As TplnOverlayPgon
			Dim oLotPgon As Polygon = Nothing
			Dim oParcelPgon As Polygon = Nothing
			Dim lstPolygons As IDictionary(Of Integer, DMAcadExt.MPolygonOverlay)

			If iTopoPurpose = enTopoPurpose.Approved Then
				mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprFDO_Overlay) = New TPlanGraph.TplnOverlayPgons
				dicOverlayPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
				mdicUnionGroupsAppr = New TplnUnionGroups()
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropFDO_Overlay) = New TPlanGraph.TplnOverlayPgons
				dicOverlayPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
				mdicUnionGroupsProp = New TplnUnionGroups()
			Else
				System.Windows.Forms.MessageBox.Show("Err 11511", "Tpr - LoadFDO_Overlay_55", MessageBoxButtons.OK, MessageBoxIcon.Stop)
				Return False
			End If
			''''''''''''''''''''''''''''''''''''''''''''''''''
			''''''''''''''''''''''''''''''''''''
			Dim oResBuffer As ResultBuffer = Nothing
			'	Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
			'	Dim oLinkTable As DMAcadExt.ODTable

			'	Dim sHandle As String
			Dim iFeatureID As Integer
			mbAddOverlayExists = True
			sPolylineLayer = "ExpLotKParcel"
			sPolylineLayer = "ZoneExpParcel"

			lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines2MPolygons(sPolylineLayer, XDataAppName, mbAddOverlayExists)

			TplnProject.WriteMessageBox(CStr(lstPolygons.Count), "מספר פוליגונים בחיתוך:")
			'	Dim sMsg As String = ""
			' System.Windows.Forms.MessageBox.Show("lstPolygons.Count" & vbCrLf & CStr(lstPolygons.Count), "04_214")

			'   DMCommon.Debug.MsgBox("08_229", DMCommon.Debug.ColCount(mdicZones), DMCommon.Debug.ColCount(dicLots), DMCommon.Debug.ColCount(mdicExpros), DMCommon.Debug.ColCount(mdicParcels))
			For Each oPolygon As DMAcadExt.MPolygonOverlay In lstPolygons.Values
				oParcel = Nothing
				oLot = Nothing
				iFeatureID = oPolygon.FeatureID
				iParcelTopoID = oPolygon.SourceID
				iLotTopoID = oPolygon.OverlayID
				iExproTopoID = oPolygon.OverlayID_Add
				iTestCounter += 1
				'	sHandle = oPolygon.Handle.ToString()

				If True Then
					'		System.Windows.Forms.MessageBox.Show(CStr(dValue) & vbCrLf & oODRec.Item(1).Type.ToString(), "04_300")
					'	iLotTopoID()
					'iParcelTopoID
					'      System.Windows.Forms.MessageBox.Show(CStr(iLotTopoID) & vbCrLf & CStr(iParcelTopoID), "04_256")
					oOverlayPgon = New TplnOverlayPgon(oPolygon, oPolygon.Entities, iFeatureID, DMAcadExt.enOverlayMethod.FDO_Overlay, iTopoPurpose)
					' 
					mdicParcelLotExproPgons.Add(oOverlayPgon)

					If True Then
						oExpro = Nothing
						If iParcelTopoID = 0 Then
							Dim sMsgText As String
							Dim sLotName As String = String.Empty
							Dim oBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox(oPolygon.GeometricExtents)
							Dim sSysID As String = "PNE," & CStr(iLotTopoID)
							Dim oCenter As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(tUnionCentroid)
							'		DMAcadExt.AcadDocument.WriteMessage(sTestA)
							If dicLots.TryGetValue(iLotTopoID, oLot) Then
								sLotName = oLot.PolygonFullName
								oCenter = oLot.GetCentroid()
								'	DMAcadExt.AcadDocument.WriteMessage("!39_08: " & CStr(oCenter.X) & ", " & CStr(oCenter.Y))
							End If
							sMsgText = sLotName & " #" & CStr(iLotTopoID) & ": " & "חלקה לא קיימת"
							DMAcadExt.AppMessages.AddMessage(True, oCenter, oBoundingBox, sSysID, sMsgText, False)

						ElseIf mdicParcels.ContainsKey(iParcelTopoID) Then
							oParcel = mdicParcels.Item(iParcelTopoID)
							'If iParcelTopoID = 15152 Then
							'	DMAcadExt.AcadDocument.WriteMessage("!51_13: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex))
							'End If
							If mdicExpros.ContainsKey(iExproTopoID) Then
								oExpro = mdicExpros.Item(iExproTopoID)
								oParcel.AddExproPgon(oExpro, oPolygon.Area)
							End If
							oParcel.AddOverlayPgon(iTopoPurpose, DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)
							'  TplnProject.vb:line 4467
							If iLotTopoID = 0 Then
								oParcel.OutPlan(iOverlayIndex) = True
							ElseIf dicLots.ContainsKey(iLotTopoID) Then
								oLot = dicLots.Item(iLotTopoID)
								If oLot.InPlan Then
									oParcel.InPlan(iOverlayIndex) = True
									oOverlayPgon.LotOut = False
								Else
									oParcel.OutPlan(iOverlayIndex) = True
									oOverlayPgon.LotOut = True
								End If
								'DMAcadExt.AcadDocument.WriteMessage("!51_34: i=" & CStr(iTestCounter) & "# " & oOverlayPgon.LotTopoID & "||" & CStr(oOverlayPgon.ParcelTopoID))	'oParcel.TestOverlayList(iOverlayIndex) & ":" & 
								'	System.Windows.Forms.MessageBox.Show(CStr(oOverlayPgon.LotTopoID) & vbCrLf & CStr(oOverlayPgon.ParcelTopoID) & vbCrLf & CStr(oLot.TopoID), "04_257")
								oLot.AddOverlayPgon(DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)
							Else
								oParcel.OutPlan = True
								DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus) & vbCrLf & " - was not found", "LoadFDO_Overlay" & iTopoPurpose.ToString())
							End If
							'	DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay! " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus))
							oParcel.AddOverlayPgon(iTopoPurpose, DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)
						Else
							DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay " & "Parcel:" & CStr(iParcelTopoID) & "- was not found", "LoadFDO_Overlay")
						End If

						'If iParcelTopoID = 15152 Then
						'	DMAcadExt.AcadDocument.WriteMessage("51_12: " & CStr(oUnionPgon.LotTopoID) & "!::!" & oUnionPgon.TopoID)
						'End If

						Try


							'	dicOverlayPgons.Add(oOverlayPgon.TopoID, oOverlayPgon) 04/02/13
							''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''MessageBox.Show(CStr(oOverlayPgon.Lines.Count), "05_100")
							dicOverlayPgons.Add(oOverlayPgon)
						Catch oEx As Exception
							MessageBox.Show(oEx.Message & vbCrLf & CStr(iTestCounter) & vbCrLf & CStr(oOverlayPgon.ParcelTopoID) & ":" & CStr(oOverlayPgon.TopoID) & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing), "02_114")
						End Try

						'		MessageBox.Show(CStr(oUnionPgon.ParcelTopoID) & ":" & CStr(oUnionPgon.LotTopoID & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing) & vbCrLf & CStr(oLot Is Nothing)), "02_113")

						'	MessageBox.Show(mdicOverlayGroups(iOverlayIndex).GetTest6(), "02_115")

						oPolygon.Dispose()
						oPolygon = Nothing
					End If
				Else
					System.Windows.Forms.MessageBox.Show("ODRec is Nothing" & vbCrLf & CStr(iTestCounter), "04_300")
					Dim tPoint As DMAcadExt.TPlnPoint = DMAcadExt.TPlnPoint.GetEntity2dCenter(oPolygon)
					DMAcadExt.AcadDocument.WriteMessage("E23: " & CStr(tPoint.X) & "," & CStr(tPoint.Y))
					Exit For
				End If
				If iTestCounter = 380 Then
					''''''''''Exit For
				End If
			Next
			'	System.Windows.Forms.MessageBox.Show("!!", "04_160")
			'''''''''''''''''''''''''''''''
			TplnOverlayPgon.CreateMainDataTable()
			'System.Windows.Forms.MessageBox.Show("mdicParcelLotExproPgons.Count" & vbCrLf & CStr(mdicParcelLotExproPgons.Count), "04_217")
			For Each oOverlayPgon In mdicParcelLotExproPgons.Values
				oOverlayPgon.AddDataToMainTable()
			Next
			'	System.Windows.Forms.MessageBox.Show("mdicParcelLotExproPgons.Count" & vbCrLf & CStr(TplnOverlayPgon.MainDataTable.Rows.Count) & vbCrLf & CStr(TplnOverlayPgon.MainView.Count), "04_218")

			Return True


		End Function
		Public Shared Function LoadFDO_ExproOverlay(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Boolean

			DMCommon.Debug.MsgBox("04_998d", "LoadFDO_ExproOverlay", iTopoPurpose)
			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			Dim dicOverlayPgons As TPlanGraph.TplnOverlayPgons
			Dim tUnionCentroid As Autodesk.AutoCAD.Geometry.Point3d
			Dim iParcelTopoID As Integer
			Dim oParcel As TplnParcel
			Dim iLotTopoID As Integer
			Dim iExproTopoID As Integer

			Dim oLot As TplnLot
			Dim oExpro As TplnExpro

			Dim iOverlayIndex As DMAcadExt.enOverlayIndex
			Dim sPolylineLayer As String
			Dim iTestCounter As Integer = 0
			Dim tLayerDef As DMAcadExt.AcadLayerDef
			Dim sTableName As String
			If iTopoPurpose = enTopoPurpose.Approved Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.ApprFDO_Overlay
				tLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.OverlayFDO_LotKParcel)
				sTableName = "LotKParcel"
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.PropFDO_Overlay
				tLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.OverlayFDO_LotMParcel)
				sTableName = "LotMParcel"
			Else
				System.Windows.Forms.MessageBox.Show("", "LoadFDO_1317")
				Return False
			End If
			Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.CreateLayer(tLayerDef, True)
			sPolylineLayer = tLayerDef.Name
			mdicOverlayGroups(iOverlayIndex) = New TPlanGraph.TplnOverlayGroups
			mdicParcelLotExproPgons = New TplnOverlayPgons()
			''''Isprav

			Dim oOverlayPgon As TplnOverlayPgon
			Dim oLotPgon As Polygon = Nothing
			Dim oParcelPgon As Polygon = Nothing
			Dim lstPolygons As IDictionary(Of Integer, DMAcadExt.MPolygonOverlay)

			If iTopoPurpose = enTopoPurpose.Approved Then
				mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprFDO_Overlay) = New TPlanGraph.TplnOverlayPgons
				dicOverlayPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
				mdicUnionGroupsAppr = New TplnUnionGroups()
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropFDO_Overlay) = New TPlanGraph.TplnOverlayPgons
				dicOverlayPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
				mdicUnionGroupsProp = New TplnUnionGroups()
			Else
				System.Windows.Forms.MessageBox.Show("Err 11511", "Tpr - LoadFDO_Overlay_55", MessageBoxButtons.OK, MessageBoxIcon.Stop)
				Return False
			End If
			''''''''''''''''''''''''''''''''''''''''''''''''''
			''''''''''''''''''''''''''''''''''''
			Dim oResBuffer As ResultBuffer = Nothing
			'	Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
			'	Dim oLinkTable As DMAcadExt.ODTable

			'	Dim sHandle As String
			Dim iFeatureID As Integer
			mbAddOverlayExists = True
			sPolylineLayer = "ExpLotKParcel"
			sPolylineLayer = "ZoneExpParcel"

			lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines2MPolygons(sPolylineLayer, XDataAppName, mbAddOverlayExists)

			TplnProject.WriteMessageBox(CStr(lstPolygons.Count), "מספר פוליגונים בחיתוך:")
			'	Dim sMsg As String = ""
			System.Windows.Forms.MessageBox.Show("lstPolygons.Count" & vbCrLf & CStr(lstPolygons.Count), "04_214")
			For Each oPolygon As DMAcadExt.MPolygonOverlay In lstPolygons.Values
				oParcel = Nothing
				oLot = Nothing
				iFeatureID = oPolygon.FeatureID
				iParcelTopoID = oPolygon.SourceID
				iLotTopoID = oPolygon.OverlayID
				iExproTopoID = oPolygon.OverlayID_Add
				iTestCounter += 1
				'	sHandle = oPolygon.Handle.ToString()

				If True Then
					'		System.Windows.Forms.MessageBox.Show(CStr(dValue) & vbCrLf & oODRec.Item(1).Type.ToString(), "04_300")
					'	iLotTopoID()
					'iParcelTopoID
					'      System.Windows.Forms.MessageBox.Show(CStr(iLotTopoID) & vbCrLf & CStr(iParcelTopoID), "04_256")
					oOverlayPgon = New TplnOverlayPgon(oPolygon, oPolygon.Entities, iFeatureID, DMAcadExt.enOverlayMethod.FDO_Overlay, iTopoPurpose)
					' 
					mdicParcelLotExproPgons.Add(oOverlayPgon)
					If True Then
						oExpro = Nothing
						If iParcelTopoID = 0 Then
							Dim sMsgText As String
							Dim sLotName As String = String.Empty
							Dim oBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox(oPolygon.GeometricExtents)
							Dim sSysID As String = "PNE," & CStr(iLotTopoID)
							Dim oCenter As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(tUnionCentroid)
							'		DMAcadExt.AcadDocument.WriteMessage(sTestA)
							If dicLots.TryGetValue(iLotTopoID, oLot) Then
								sLotName = oLot.PolygonFullName
								oCenter = oLot.GetCentroid()
								'	DMAcadExt.AcadDocument.WriteMessage("!39_08: " & CStr(oCenter.X) & ", " & CStr(oCenter.Y))
							End If
							sMsgText = sLotName & " #" & CStr(iLotTopoID) & ": " & "חלקה לא קיימת"
							DMAcadExt.AppMessages.AddMessage(True, oCenter, oBoundingBox, sSysID, sMsgText, False)

						ElseIf mdicParcels.ContainsKey(iParcelTopoID) Then
							oParcel = mdicParcels.Item(iParcelTopoID)
							'If iParcelTopoID = 15152 Then
							'	DMAcadExt.AcadDocument.WriteMessage("!51_13: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex))
							'End If
							If mdicExpros.ContainsKey(iExproTopoID) Then
								oExpro = mdicExpros.Item(iExproTopoID)
								oParcel.AddExproPgon(oExpro, oPolygon.Area)
							End If
							oParcel.AddOverlayPgon(iTopoPurpose, DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)
							'  TplnProject.vb:line 4467
							If iLotTopoID = 0 Then
								oParcel.OutPlan(iOverlayIndex) = True
							ElseIf dicLots.ContainsKey(iLotTopoID) Then
								oLot = dicLots.Item(iLotTopoID)
								If oLot.InPlan Then
									oParcel.InPlan(iOverlayIndex) = True
									oOverlayPgon.LotOut = False
								Else
									oParcel.OutPlan(iOverlayIndex) = True
									oOverlayPgon.LotOut = True
								End If
								'DMAcadExt.AcadDocument.WriteMessage("!51_34: i=" & CStr(iTestCounter) & "# " & oOverlayPgon.LotTopoID & "||" & CStr(oOverlayPgon.ParcelTopoID))	'oParcel.TestOverlayList(iOverlayIndex) & ":" & 
								'	System.Windows.Forms.MessageBox.Show(CStr(oOverlayPgon.LotTopoID) & vbCrLf & CStr(oOverlayPgon.ParcelTopoID) & vbCrLf & CStr(oLot.TopoID), "04_257")
								oLot.AddOverlayPgon(DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)
							Else
								oParcel.OutPlan = True
								DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus) & vbCrLf & " - was not found", "LoadFDO_Overlay" & iTopoPurpose.ToString())
							End If
							'	DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay! " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus))
							oParcel.AddOverlayPgon(iTopoPurpose, DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)
						Else
							DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay " & "Parcel:" & CStr(iParcelTopoID) & "- was not found", "LoadFDO_Overlay")
						End If

						'If iParcelTopoID = 15152 Then
						'	DMAcadExt.AcadDocument.WriteMessage("51_12: " & CStr(oUnionPgon.LotTopoID) & "!::!" & oUnionPgon.TopoID)
						'End If

						Try


							'	dicOverlayPgons.Add(oOverlayPgon.TopoID, oOverlayPgon) 04/02/13
							''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''MessageBox.Show(CStr(oOverlayPgon.Lines.Count), "05_100")
							dicOverlayPgons.Add(oOverlayPgon)
						Catch oEx As Exception
							MessageBox.Show(oEx.Message & vbCrLf & CStr(iTestCounter) & vbCrLf & CStr(oOverlayPgon.ParcelTopoID) & ":" & CStr(oOverlayPgon.TopoID) & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing), "02_114")
						End Try

						'		MessageBox.Show(CStr(oUnionPgon.ParcelTopoID) & ":" & CStr(oUnionPgon.LotTopoID & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing) & vbCrLf & CStr(oLot Is Nothing)), "02_113")

						'	MessageBox.Show(mdicOverlayGroups(iOverlayIndex).GetTest6(), "02_115")

						oPolygon.Dispose()
						oPolygon = Nothing
					End If
				Else
					System.Windows.Forms.MessageBox.Show("ODRec is Nothing" & vbCrLf & CStr(iTestCounter), "04_300")
					Dim tPoint As DMAcadExt.TPlnPoint = DMAcadExt.TPlnPoint.GetEntity2dCenter(oPolygon)
					DMAcadExt.AcadDocument.WriteMessage("E23: " & CStr(tPoint.X) & "," & CStr(tPoint.Y))
					Exit For
				End If
				If iTestCounter = 380 Then
					''''''''''Exit For
				End If
			Next
			'	System.Windows.Forms.MessageBox.Show("!!", "04_160")
			'''''''''''''''''''''''''''''''
			TplnOverlayPgon.CreateMainDataTable()
			'System.Windows.Forms.MessageBox.Show("mdicParcelLotExproPgons.Count" & vbCrLf & CStr(mdicParcelLotExproPgons.Count), "04_217")
			For Each oOverlayPgon In mdicParcelLotExproPgons.Values
				oOverlayPgon.AddDataToMainTable()
			Next
			'	System.Windows.Forms.MessageBox.Show("mdicParcelLotExproPgons.Count" & vbCrLf & CStr(TplnOverlayPgon.MainDataTable.Rows.Count) & vbCrLf & CStr(TplnOverlayPgon.MainView.Count), "04_218")

			Return True


		End Function

		Public Shared Function LoadFDO_MerhavOverlay(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As Boolean
			System.Windows.Forms.MessageBox.Show("LoadFDO_MerhavOverlay" & vbCrLf & iTopoPurpose.ToString(), "04_999")

			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			Dim dicOverlayPgons As TPlanGraph.TplnOverlayPgons
			Dim tUnionCentroid As Autodesk.AutoCAD.Geometry.Point3d
			Dim iParcelTopoID As Integer
			Dim oParcel As TplnParcel
			Dim iLotTopoID As Integer
			Dim iMerhavTopoID As Integer

			Dim oLot As TplnLot
			Dim oMerhav As TplnMerhav


			Dim iOverlayIndex As DMAcadExt.enOverlayIndex
			Dim sPolylineLayer As String
			Dim iTestCounter As Integer = 0
			Dim tLayerDef As DMAcadExt.AcadLayerDef
			Dim sTableName As String
			If iTopoPurpose = enTopoPurpose.Approved Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.ApprFDO_Overlay
				tLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.OverlayFDO_LotKParcel)
				sTableName = "LotKParcel"
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				iOverlayIndex = DMAcadExt.enOverlayIndex.PropFDO_Overlay
				tLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.OverlayFDO_LotMParcel)
				sTableName = "LotMParcel"
			Else
				System.Windows.Forms.MessageBox.Show("", "LoadFDO_1319")
				Return False
			End If
			Dim bCurrentLayerOK As Boolean = DMAcadExt.AcadTransaction.CreateLayer(tLayerDef, True)
			sPolylineLayer = tLayerDef.Name
			mdicOverlayGroups(iOverlayIndex) = New TPlanGraph.TplnOverlayGroups
			mdicParcelLotExproPgons = New TplnOverlayPgons()
			''''Isprav

			Dim oOverlayPgon As TplnOverlayPgon
			Dim oLotPgon As Polygon = Nothing
			Dim oParcelPgon As Polygon = Nothing
			Dim lstPolygons As IDictionary(Of Integer, DMAcadExt.MPolygonOverlay)

			If iTopoPurpose = enTopoPurpose.Approved Then
				mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprFDO_Overlay) = New TPlanGraph.TplnOverlayPgons
				dicOverlayPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.ApprFDO_Overlay)
				mdicUnionGroupsAppr = New TplnUnionGroups()
			ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
				mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropFDO_Overlay) = New TPlanGraph.TplnOverlayPgons
				dicOverlayPgons = mdicOverlayPgons(DMAcadExt.enOverlayIndex.PropFDO_Overlay)
				mdicUnionGroupsProp = New TplnUnionGroups()
			Else
				System.Windows.Forms.MessageBox.Show("Err 11511", "Tpr - LoadFDO_Overlay_55", MessageBoxButtons.OK, MessageBoxIcon.Stop)
				Return False
			End If
			''''''''''''''''''''''''''''''''''''''''''''''''''
			''''''''''''''''''''''''''''''''''''
			Dim oResBuffer As ResultBuffer = Nothing
			'	Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
			'	Dim oLinkTable As DMAcadExt.ODTable

			'	Dim sHandle As String
			Dim iFeatureID As Integer
			mbAddOverlayExists = True
			sPolylineLayer = "MerhLotKParcel"
			lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines2MPolygons(sPolylineLayer, XDataAppName, mbAddOverlayExists)

			TplnProject.WriteMessageBox(CStr(lstPolygons.Count), "מספר פוליגונים בחיתוך:")
			'	Dim sMsg As String = ""
			System.Windows.Forms.MessageBox.Show("lstPolygons.Count" & vbCrLf & CStr(lstPolygons.Count), "04_214")
			For Each oPolygon As DMAcadExt.MPolygonOverlay In lstPolygons.Values
				oParcel = Nothing
				oLot = Nothing
				iFeatureID = oPolygon.FeatureID
				iParcelTopoID = oPolygon.SourceID
				iLotTopoID = oPolygon.OverlayID
				iMerhavTopoID = oPolygon.OverlayID_Add
				iTestCounter += 1
				'	sHandle = oPolygon.Handle.ToString()

				If True Then
					'		System.Windows.Forms.MessageBox.Show(CStr(dValue) & vbCrLf & oODRec.Item(1).Type.ToString(), "04_300")
					'	iLotTopoID()
					'iParcelTopoID
					'	System.Windows.Forms.MessageBox.Show(CStr(iLotTopoID) & vbCrLf & CStr(iParcelTopoID), "04_256")
					oOverlayPgon = New TplnMerhavOverlayPgon(oPolygon, oPolygon.Entities, iFeatureID, DMAcadExt.enOverlayMethod.FDO_Overlay, iTopoPurpose)
					' 
					mdicParcelLotExproPgons.Add(oOverlayPgon)
					If True Then
						oMerhav = Nothing
						If iParcelTopoID = 0 Then
							Dim sMsgText As String
							Dim sLotName As String = String.Empty
							Dim oBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox(oPolygon.GeometricExtents)
							Dim sSysID As String = "PNE," & CStr(iLotTopoID)
							Dim oCenter As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(tUnionCentroid)
							'		DMAcadExt.AcadDocument.WriteMessage(sTestA)
							If dicLots.TryGetValue(iLotTopoID, oLot) Then
								sLotName = oLot.PolygonFullName
								oCenter = oLot.GetCentroid()
								'	DMAcadExt.AcadDocument.WriteMessage("!39_08: " & CStr(oCenter.X) & ", " & CStr(oCenter.Y))
							End If
							sMsgText = sLotName & " #" & CStr(iLotTopoID) & ": " & "חלקה לא קיימת"
							DMAcadExt.AppMessages.AddMessage(True, oCenter, oBoundingBox, sSysID, sMsgText, False)

						ElseIf mdicParcels.ContainsKey(iParcelTopoID) Then
							oParcel = mdicParcels.Item(iParcelTopoID)
							'If iParcelTopoID = 15152 Then
							'	DMAcadExt.AcadDocument.WriteMessage("!51_13: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex))
							'End If
							If mdicMerhav.ContainsKey(iMerhavTopoID) Then
								oMerhav = mdicMerhav.Item(iMerhavTopoID)
								''''''''''''''''    oParcel.AddExproPgon(oExpro, oPolygon.Area)
							End If
							'  TplnProject.vb:line 4467
							If iLotTopoID = 0 Then
								oParcel.OutPlan(iOverlayIndex) = True
							ElseIf dicLots.ContainsKey(iLotTopoID) Then
								oLot = dicLots.Item(iLotTopoID)
								If oLot.InPlan Then
									oParcel.InPlan(iOverlayIndex) = True
									oOverlayPgon.LotOut = False
								Else
									oParcel.OutPlan(iOverlayIndex) = True
									oOverlayPgon.LotOut = True
								End If
								'DMAcadExt.AcadDocument.WriteMessage("!51_34: i=" & CStr(iTestCounter) & "# " & oOverlayPgon.LotTopoID & "||" & CStr(oOverlayPgon.ParcelTopoID))	'oParcel.TestOverlayList(iOverlayIndex) & ":" & 
								'	System.Windows.Forms.MessageBox.Show(CStr(oOverlayPgon.LotTopoID) & vbCrLf & CStr(oOverlayPgon.ParcelTopoID) & vbCrLf & CStr(oLot.TopoID), "04_257")
								oLot.AddOverlayPgon(DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)
							Else
								oParcel.OutPlan = True
								DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus) & vbCrLf & " - was not found", "LoadFDO_Overlay" & iTopoPurpose.ToString())
							End If
							'	DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay! " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus))
							oParcel.AddOverlayPgon(iTopoPurpose, DMAcadExt.enOverlayMethod.FDO_Overlay, oOverlayPgon)
						Else
							DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay " & "Parcel:" & CStr(iParcelTopoID) & "- was not found", "LoadFDO_Overlay")
						End If

						'If iParcelTopoID = 15152 Then
						'	DMAcadExt.AcadDocument.WriteMessage("51_12: " & CStr(oUnionPgon.LotTopoID) & "!::!" & oUnionPgon.TopoID)
						'End If

						Try


							'	dicOverlayPgons.Add(oOverlayPgon.TopoID, oOverlayPgon) 04/02/13
							''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''MessageBox.Show(CStr(oOverlayPgon.Lines.Count), "05_100")
							dicOverlayPgons.Add(oOverlayPgon)
						Catch oEx As Exception
							MessageBox.Show(oEx.Message & vbCrLf & CStr(iTestCounter) & vbCrLf & CStr(oOverlayPgon.ParcelTopoID) & ":" & CStr(oOverlayPgon.TopoID) & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing), "02_114")
						End Try

						'		MessageBox.Show(CStr(oUnionPgon.ParcelTopoID) & ":" & CStr(oUnionPgon.LotTopoID & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing) & vbCrLf & CStr(oLot Is Nothing)), "02_113")

						'	MessageBox.Show(mdicOverlayGroups(iOverlayIndex).GetTest6(), "02_115")

						oPolygon.Dispose()
						oPolygon = Nothing
					End If
				Else
					System.Windows.Forms.MessageBox.Show("ODRec is Nothing" & vbCrLf & CStr(iTestCounter), "04_300")
					Dim tPoint As DMAcadExt.TPlnPoint = DMAcadExt.TPlnPoint.GetEntity2dCenter(oPolygon)
					DMAcadExt.AcadDocument.WriteMessage("E23: " & CStr(tPoint.X) & "," & CStr(tPoint.Y))
					Exit For
				End If
				If iTestCounter = 380 Then
					''''''''''Exit For
				End If
			Next
			'	System.Windows.Forms.MessageBox.Show("!!", "04_160")
			'''''''''''''''''''''''''''''''
			TplnMerhavOverlayPgon.CreateMainDataTable()
			'System.Windows.Forms.MessageBox.Show("mdicParcelLotExproPgons.Count" & vbCrLf & CStr(mdicParcelLotExproPgons.Count), "04_217")
			Dim oMerhavOverlayPgon As TplnMerhavOverlayPgon
			For Each oOverlayPgon In mdicParcelLotExproPgons.Values
				oMerhavOverlayPgon = DirectCast(oOverlayPgon, TplnMerhavOverlayPgon)
				oMerhavOverlayPgon.AddDataToMainTable()
			Next
			'	System.Windows.Forms.MessageBox.Show("mdicParcelLotExproPgons.Count" & vbCrLf & CStr(TplnOverlayPgon.MainDataTable.Rows.Count) & vbCrLf & CStr(TplnOverlayPgon.MainView.Count), "04_218")

			Return True


		End Function

		Private Shared Function zzLoadFDO_ExproZoneOverlay() As Boolean
			'   System.Windows.Forms.MessageBox.Show("zzLoadFDO_ExproZoneOverlay", "04_991a")
			'  Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)a

			'     DMCommon.ExcelLogG.Open()
			Dim tUnionCentroid As Autodesk.AutoCAD.Geometry.Point3d
			Dim iParcelTopoID As Integer
			Dim oParcel As TplnParcel
			Dim iZoneTopoID As Integer
			Dim iExproTopoID As Integer

			Dim oZone As TplnZone
			Dim oExpro As TplnExpro

			Dim iOverlayIndex As DMAcadExt.enOverlayIndex
			Dim sPolylineLayer As String
			Dim iTestCounter As Integer = 0
			'	Dim tLayerDef As DMAcadExt.AcadLayerDef

			Dim iZoneID As Integer
			Dim sZoneName As String


			'	sPolylineLayer = tLayerDef.Name
			sPolylineLayer = "ZoneExpParcel"
			mdicOverlayGroups(iOverlayIndex) = New TPlanGraph.TplnOverlayGroups
			mdicParcelLotExproPgons = New TplnOverlayPgons()
			''''Isprav

			Dim oOverlayPgon As TplnOverlayPgon
			Dim oLotPgon As Polygon = Nothing
			Dim oParcelPgon As Polygon = Nothing
			Dim lstPolygons As IDictionary(Of Integer, DMAcadExt.MPolygonOverlay)


			mdicOverlayPgons(0) = New TPlanGraph.TplnOverlayPgons
			''''''''''''''''''''''''''''''''''''''''''''''''''
			''''''''''''''''''''''''''''''''''''
			Dim oResBuffer As ResultBuffer = Nothing
			'	Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
			'	Dim oLinkTable As DMAcadExt.ODTable

			'	Dim sHandle As String
			Dim iFeatureID As Integer
			mbAddOverlayExists = True

			lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines2MPolygons(sPolylineLayer, XDataAppName, mbAddOverlayExists)

			TplnProject.WriteMessageBox(CStr(lstPolygons.Count), "מספר פוליגונים בחיתוך:" & "++++" & mbAddOverlayExists.ToString())
			TplnProject.WriteMessageBox(CStr(mdicParcels.Count) & ":" & CStr(mdicExpros.Count) & ":" & CStr(mdicZones.Count), "")


			'	Dim sMsg As String = ""
			'     System.Windows.Forms.MessageBox.Show("lstPolygons.Count" & vbCrLf & CStr(lstPolygons.Count), "04_214")


			For Each oPolygon As DMAcadExt.MPolygonOverlay In lstPolygons.Values
				oParcel = Nothing
				oExpro = Nothing
				oZone = Nothing
				iZoneID = 0
				sZoneName = ""

				iFeatureID = oPolygon.FeatureID
				iParcelTopoID = oPolygon.SourceID
				iExproTopoID = oPolygon.OverlayID
				iZoneTopoID = oPolygon.OverlayID_Add
				'   DMCommon.ExcelLogG.SetNextValue(iRow, 0, iFeatureID, "Parcel=", iParcelTopoID, "Expro=", iExproTopoID, "Zone=", iZoneTopoID)
				If iExproTopoID <> 0 Then
					' TplnProject.WriteMessageBox(CStr(iParcelTopoID) & ":" & CStr(iExproTopoID) & ":" & CStr(iZoneTopoID), "")
				End If
				iTestCounter += 1
				'	sHandle = oPolygon.Handle.ToString()



				'  oOverlayPgon = New TplnOverlayPgon(oPolygon, oPolygon.Entities, iFeatureID, DMAcadExt.enOverlayMethod.FDO_Overlay, 0)

				'   mdicParcelLotExproPgons.Add(oOverlayPgon)

				oExpro = Nothing
				If iParcelTopoID = 0 Then
					' Dim sMsgText As String
					Dim sLotName As String = String.Empty
					Dim oBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox(oPolygon.GeometricExtents)
					Dim sSysID As String = "PNE," & CStr(135)
					Dim oCenter As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(tUnionCentroid)
					'		DMAcadExt.AcadDocument.WriteMessage(sTestA)
					' DMCommon.ExcelLogG.SetValue(iRow, 12, iParcelTopoID, "XXXXX")
				ElseIf mdicParcels.ContainsKey(iParcelTopoID) Then
					oParcel = mdicParcels.Item(iParcelTopoID)
					'  DMCommon.ExcelLogG.SetValue(iRow, 12, iParcelTopoID, "!" & oParcel.Name)
					'If iParcelTopoID = 15152 Then
					'	DMAcadExt.AcadDocument.WriteMessage("!51_13: i=" & CStr(iTestCounter) & "# " & oParcel.TestOverlayList(iOverlayIndex))
					'End If
					If iExproTopoID <> 0 AndAlso mdicExpros.ContainsKey(iExproTopoID) Then
						oExpro = mdicExpros.Item(iExproTopoID)
						'  DMCommon.ExcelLogG.SetValue(iRow, 14, iExproTopoID, oExpro.ExproType, oExpro.ExproTypeName)
					End If
					If iZoneTopoID <> 0 AndAlso mdicZones.ContainsKey(iZoneTopoID) Then
						oZone = mdicZones.Item(iZoneTopoID)
						iZoneID = oZone.ZoneID
						sZoneName = DMCommon.Hebrew.Invert(oZone.ZoneName)
						' DMCommon.ExcelLogG.SetValue(iRow, 18, iZoneTopoID, iZoneID, sZoneName)
					End If



					'  TplnProject.vb:line 4467

					'	DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay! " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus))

				Else
					DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay " & "Parcel:" & CStr(iParcelTopoID) & "- was not found", "LoadFDO_Overlay")
				End If

				'If iParcelTopoID = 15152 Then
				'	DMAcadExt.AcadDocument.WriteMessage("51_12: " & CStr(oUnionPgon.LotTopoID) & "!::!" & oUnionPgon.TopoID)
				'End If


				'		MessageBox.Show(CStr(oUnionPgon.ParcelTopoID) & ":" & CStr(oUnionPgon.LotTopoID & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing) & vbCrLf & CStr(oLot Is Nothing)), "02_113")

				'	MessageBox.Show(mdicOverlayGroups(iOverlayIndex).GetTest6(), "02_115")

				oPolygon.Dispose()
				oPolygon = Nothing


				If iTestCounter = 380 Then
					''''''''''Exit For
				End If
			Next
			Return False
			'	System.Windows.Forms.MessageBox.Show("!!", "04_160")
			'''''''''''''''''''''''''''''''
			TplnOverlayPgon.CreateMainDataTable()
			'System.Windows.Forms.MessageBox.Show("mdicParcelLotExproPgons.Count" & vbCrLf & CStr(mdicParcelLotExproPgons.Count), "04_217")
			For Each oOverlayPgon In mdicParcelLotExproPgons.Values
				oOverlayPgon.AddDataToMainTable()
			Next
			'	System.Windows.Forms.MessageBox.Show("mdicParcelLotExproPgons.Count" & vbCrLf & CStr(TplnOverlayPgon.MainDataTable.Rows.Count) & vbCrLf & CStr(TplnOverlayPgon.MainView.Count), "04_218")

			Return True


		End Function
		Private Shared Function zzLoadFDO_ExproLotOverlay() As Boolean
			Dim dic As IDictionary(Of Integer, DMAcadExt.ColorScheme) = TplnLanduse.GetColorSchemeDic(DMAcadExt.enTopoPurpose.Approved)
			DMCommon.Debug.MsgBox("zzLoadFDO_ExproLotOverlay", "04_991a", dic.Count)
			'  Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)a

			'     DMCommon.ExcelLogG.Open()
			Dim tUnionCentroid As Autodesk.AutoCAD.Geometry.Point3d
			Dim iParcelTopoID As Integer
			Dim oParcel As TplnParcel
			Dim iLotTopoID As Integer
			Dim iExproTopoID As Integer
			Dim sExproTypeName As String = Nothing
			Dim oZone As TplnZone
			Dim oExpro As TplnExpro

			Dim iOverlayIndex As DMAcadExt.enOverlayIndex
			Dim sPolylineLayer As String
			Dim iTestCounter As Integer = 0
			'	Dim tLayerDef As DMAcadExt.AcadLayerDef

			Dim iZoneID As Integer
			Dim sZoneName As String


			'	sPolylineLayer = tLayerDef.Name
			sPolylineLayer = "ExpLotKParcel"
			mdicOverlayGroups(iOverlayIndex) = New TPlanGraph.TplnOverlayGroups
			mdicParcelLotExproPgons = New TplnOverlayPgons()
			''''Isprav

			Dim oOverlayPgon As TplnOverlayPgon
			Dim oLotPgon As Polygon = Nothing
			Dim oParcelPgon As Polygon = Nothing
			Dim lstPolygons As IDictionary(Of Integer, DMAcadExt.MPolygonOverlay)


			mdicOverlayPgons(0) = New TPlanGraph.TplnOverlayPgons
			''''''''''''''''''''''''''''''''''''''''''''''''''
			''''''''''''''''''''''''''''''''''''
			Dim oResBuffer As ResultBuffer = Nothing
			'	Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
			'	Dim oLinkTable As DMAcadExt.ODTable

			'	Dim sHandle As String
			Dim iFeatureID As Integer
			Dim iExproType As Integer
			Dim oLot As TplnLot = Nothing
			mbAddOverlayExists = True

			lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines2MPolygons(sPolylineLayer, XDataAppName, mbAddOverlayExists)

			TplnProject.WriteMessageBox(CStr(lstPolygons.Count), "מספר פוליגונים בחיתוך:" & "++++" & mbAddOverlayExists.ToString())
			'	TplnProject.WriteMessageBox(CStr(mdicParcels.Count) & ":" & CStr(mdicExpros.Count), "")


			'	Dim sMsg As String = ""
			'     System.Windows.Forms.MessageBox.Show("lstPolygons.Count" & vbCrLf & CStr(lstPolygons.Count), "04_214")

			'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!?!BeforeLoad", DMCommon.Debug.ColCount(lstPolygons.Keys), DMCommon.Debug.ColCount(mdicParcels), DMCommon.Debug.ColCount(mdicExpros), DMCommon.Debug.ColCount(mdicLotsAppr))
			For Each oPolygon As DMAcadExt.MPolygonOverlay In lstPolygons.Values
				oParcel = Nothing
				oExpro = Nothing
				oZone = Nothing
				iZoneID = 0
				sZoneName = ""

				iFeatureID = oPolygon.FeatureID
				iParcelTopoID = oPolygon.SourceID
				iExproTopoID = oPolygon.OverlayID
				iLotTopoID = oPolygon.OverlayID_Add


				If iExproTopoID <> 0 Then
					' TplnProject.WriteMessageBox(CStr(iParcelTopoID) & ":" & CStr(iExproTopoID) & ":" & CStr(iZoneTopoID), "")
				End If
				iTestCounter += 1
				'	sHandle = oPolygon.Handle.ToString()
				oOverlayPgon = New TplnOverlayPgon(oPolygon, New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection(), iFeatureID, DMAcadExt.enOverlayMethod.FDO_Overlay, DMAcadExt.enTopoPurpose.Approved)


				'  oOverlayPgon = New TplnOverlayPgon(oPolygon, oPolygon.Entities, iFeatureID, DMAcadExt.enOverlayMethod.FDO_Overlay, 0)

				'   mdicParcelLotExproPgons.Add(oOverlayPgon)


				If iParcelTopoID = 0 Then
					' Dim sMsgText As String
					Dim sLotName As String = String.Empty
					Dim oBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox(oPolygon.GeometricExtents)
					Dim sSysID As String = "PNE," & CStr(135)
					Dim oCenter As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(tUnionCentroid)
					'		DMAcadExt.AcadDocument.WriteMessage(sTestA)
					' DMCommon.ExcelLogG.SetValue(iRow, 12, iParcelTopoID, "XXXXX")
				ElseIf mdicParcels.ContainsKey(iParcelTopoID) Then
					oParcel = mdicParcels.Item(iParcelTopoID)

					If iExproTopoID <> 0 AndAlso mdicExpros.ContainsKey(iExproTopoID) Then
						oExpro = mdicExpros.Item(iExproTopoID)
						iExproType = oExpro.ExproTypeID
						sExproTypeName = oExpro.Name
					Else
						iExproType = 0

					End If

					If iLotTopoID <> 0 AndAlso mdicLotsAppr.ContainsKey(iLotTopoID) Then

						oLot = mdicLotsAppr.Item(iLotTopoID)
						If oParcel IsNot Nothing Then
							If iExproTopoID <> 0 Then


								oParcel.AddExproLusePolygon(iExproType, oLot.LanduseID, oPolygon.Area)


								If iExproType <> -1 Then
									''''''''''''''''''''''''''''''''''''''''''''oParcel.AddExproPgon(iExproType, sExproTypeName, oPolygon.Area)
								End If
							Else


							End If
						End If
					ElseIf iLotTopoID <> 0 Then
						'130126	DMCommon.Debug.ExcelLog.SetNextValue(0, "!Else_A", oParcel.TopoID, oParcel.BlockNo, oParcel.ParcelNo, iExproTopoID, iExproType, iLotTopoID, oLot.LanduseID)
					End If


					'  TplnProject.vb:line 4467

					'	DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay! " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus))

				Else
					DMAcadExt.AcadDocument.WriteMessage("!LoadFDO_Overlay - ParcelID:" & CStr(iParcelTopoID) & "- was not found", "LoadFDO_Overlay")
				End If

				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!BaseTbl", iFeatureID, iParcelTopoID, iExproTopoID, iLotTopoID)
				'If iParcelTopoID = 15152 Then
				'	DMAcadExt.AcadDocument.WriteMessage("51_12: " & CStr(oUnionPgon.LotTopoID) & "!::!" & oUnionPgon.TopoID)
				'End If


				'		MessageBox.Show(CStr(oUnionPgon.ParcelTopoID) & ":" & CStr(oUnionPgon.LotTopoID & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing) & vbCrLf & CStr(oLot Is Nothing)), "02_113")

				'	MessageBox.Show(mdicOverlayGroups(iOverlayIndex).GetTest6(), "02_115")

				oPolygon.Dispose()
				oPolygon = Nothing


				If iTestCounter = 380 Then
					''''''''''Exit For
				End If
			Next


			Return False
			'	System.Windows.Forms.MessageBox.Show("!!", "04_160")
			'''''''''''''''''''''''''''''''
			TplnOverlayPgon.CreateMainDataTable()
			'System.Windows.Forms.MessageBox.Show("mdicParcelLotExproPgons.Count" & vbCrLf & CStr(mdicParcelLotExproPgons.Count), "04_217")
			For Each oOverlayPgon In mdicParcelLotExproPgons.Values
				oOverlayPgon.AddDataToMainTable()
			Next
			'	System.Windows.Forms.MessageBox.Show("mdicParcelLotExproPgons.Count" & vbCrLf & CStr(TplnOverlayPgon.MainDataTable.Rows.Count) & vbCrLf & CStr(TplnOverlayPgon.MainView.Count), "04_218")

			Return True


		End Function

		Private Shared Function zzLoadFDO_ExproLotOverlayBasicPlan() As Boolean
			'DMCommon.Debug.MsgBox("zzLoadFDO_ExproLotOverlay", "04_993")
			'  Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)a

			'     DMCommon.ExcelLogG.Open()
			Dim tUnionCentroid As Autodesk.AutoCAD.Geometry.Point3d
			Dim iParcelTopoID As Integer
			Dim oParcel As TplnParcel
			Dim iLotTopoID As Integer
			Dim iExproTopoID As Integer
			Dim sExproTypeName As String = Nothing
			Dim oZone As TplnZone
			Dim oExpro As TplnExpro

			Dim iOverlayIndex As DMAcadExt.enOverlayIndex
			Dim sPolylineLayer As String
			Dim iTestCounter As Integer = 0
			'	Dim tLayerDef As DMAcadExt.AcadLayerDef

			Dim iZoneID As Integer
			Dim sZoneName As String


			'	sPolylineLayer = tLayerDef.Name
			sPolylineLayer = "ExpLotKParcel"
			mdicOverlayGroups(iOverlayIndex) = New TPlanGraph.TplnOverlayGroups
			mdicParcelLotExproPgons = New TplnOverlayPgons()
			''''Isprav

			Dim oOverlayPgon As TplnOverlayPgon
			Dim oLotPgon As Polygon = Nothing
			Dim oParcelPgon As Polygon = Nothing
			Dim lstPolygons As IDictionary(Of Integer, DMAcadExt.MPolygonOverlay)


			mdicOverlayPgons(0) = New TPlanGraph.TplnOverlayPgons
			''''''''''''''''''''''''''''''''''''''''''''''''''
			''''''''''''''''''''''''''''''''''''
			Dim oResBuffer As ResultBuffer = Nothing
			'	Dim oODRec As Autodesk.Gis.Map.ObjectData.Record = Nothing
			'	Dim oLinkTable As DMAcadExt.ODTable

			'	Dim sHandle As String
			Dim iFeatureID As Integer
			Dim iExproType As Integer
			Dim oLot As TplnLot = Nothing
			mbAddOverlayExists = True

			lstPolygons = DMAcadExt.AcadTransaction.GetAcadPolylines2MPolygons(sPolylineLayer, XDataAppName, mbAddOverlayExists)

			TplnProject.WriteMessageBox(CStr(lstPolygons.Count), "מספר פוליגונים בחיתוך:" & "++++" & mbAddOverlayExists.ToString())
			'	TplnProject.WriteMessageBox(CStr(mdicParcels.Count) & ":" & CStr(mdicExpros.Count), "")


			'	Dim sMsg As String = ""
			'     System.Windows.Forms.MessageBox.Show("lstPolygons.Count" & vbCrLf & CStr(lstPolygons.Count), "04_214")

			'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!?!BeforeLoad", DMCommon.Debug.ColCount(lstPolygons.Keys), DMCommon.Debug.ColCount(mdicParcels), DMCommon.Debug.ColCount(mdicExpros), DMCommon.Debug.ColCount(mdicLotsAppr))
			For Each oPolygon As DMAcadExt.MPolygonOverlay In lstPolygons.Values
				oParcel = Nothing
				oExpro = Nothing
				oZone = Nothing
				iZoneID = 0
				sZoneName = ""

				iFeatureID = oPolygon.FeatureID
				iParcelTopoID = oPolygon.SourceID
				iExproTopoID = oPolygon.OverlayID
				iLotTopoID = oPolygon.OverlayID_Add


				If iExproTopoID <> 0 Then
					' TplnProject.WriteMessageBox(CStr(iParcelTopoID) & ":" & CStr(iExproTopoID) & ":" & CStr(iZoneTopoID), "")
				End If
				iTestCounter += 1
				'	sHandle = oPolygon.Handle.ToString()
				oOverlayPgon = New TplnOverlayPgon(oPolygon, New Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection(), iFeatureID, DMAcadExt.enOverlayMethod.FDO_Overlay, DMAcadExt.enTopoPurpose.Approved)


				'  oOverlayPgon = New TplnOverlayPgon(oPolygon, oPolygon.Entities, iFeatureID, DMAcadExt.enOverlayMethod.FDO_Overlay, 0)

				'   mdicParcelLotExproPgons.Add(oOverlayPgon)


				If iParcelTopoID = 0 Then
					' Dim sMsgText As String
					Dim sLotName As String = String.Empty
					Dim oBoundingBox As DMAcadExt.TPlnBoundingBox = New DMAcadExt.TPlnBoundingBox(oPolygon.GeometricExtents)
					Dim sSysID As String = "PNE," & CStr(135)
					Dim oCenter As DMAcadExt.TPlnPoint = New DMAcadExt.TPlnPoint(tUnionCentroid)
					'		DMAcadExt.AcadDocument.WriteMessage(sTestA)
					' DMCommon.ExcelLogG.SetValue(iRow, 12, iParcelTopoID, "XXXXX")
				ElseIf mdicParcels.ContainsKey(iParcelTopoID) Then
					oParcel = mdicParcels.Item(iParcelTopoID)

					If iExproTopoID <> 0 AndAlso mdicExpros.ContainsKey(iExproTopoID) Then
						oExpro = mdicExpros.Item(iExproTopoID)
						iExproType = oExpro.ExproTypeID
						sExproTypeName = oExpro.Name
					Else
						iExproType = 0

					End If

					If iLotTopoID <> 0 AndAlso mdicLotsAppr.ContainsKey(iLotTopoID) Then

						oLot = mdicLotsAppr.Item(iLotTopoID)
						If oLot IsNot Nothing Then
							If iExproTopoID <> 0 Then
								' DMCommon.Debug.MsgBoxLoop("B01_00", 1, oLot.TopoID, oLot.BasicPlanID, oLot.LanduseID)
								oParcel.AddExproLusePolygon(iExproType, oLot.LanduseID, oPolygon.Area)
								oParcel.AddExproLusePolygon(iExproType, oLot.LanduseID, oLot.BasicPlanID, oPolygon.Area)




								If iExproType <> -1 Then
									''''''''''''''''''''''''''''''''''''''''''''oParcel.AddExproPgon(iExproType, sExproTypeName, oPolygon.Area)
								End If
							Else


							End If
						End If
					ElseIf iLotTopoID <> 0 Then
						'130126	DMCommon.Debug.ExcelLog.SetNextValue(0, "!Else_A", oParcel.TopoID, oParcel.BlockNo, oParcel.ParcelNo, iExproTopoID, iExproType, iLotTopoID, oLot.LanduseID)
					End If


					'  TplnProject.vb:line 4467

					'	DMAcadExt.AcadDocument.WriteMessage("LoadFDO_Overlay! " & iTopoPurpose.ToString() & "Lot:" & CStr(iLotTopoID) & vbCrLf & CStr(dicLots.PlanStatus))

				Else
					DMAcadExt.AcadDocument.WriteMessage("!LoadFDO_Overlay - ParcelID:" & CStr(iParcelTopoID) & "- was not found", "LoadFDO_Overlay")
				End If

				'DMCommon.Debug.ExcelLog.SetNextValue(0, "!BaseTbl", iFeatureID, iParcelTopoID, iExproTopoID, iLotTopoID)
				'If iParcelTopoID = 15152 Then
				'	DMAcadExt.AcadDocument.WriteMessage("51_12: " & CStr(oUnionPgon.LotTopoID) & "!::!" & oUnionPgon.TopoID)
				'End If


				'		MessageBox.Show(CStr(oUnionPgon.ParcelTopoID) & ":" & CStr(oUnionPgon.LotTopoID & vbCrLf & CStr(mdicOverlayGroups(iOverlayIndex) Is Nothing) & vbCrLf & CStr(oLot Is Nothing)), "02_113")

				'	MessageBox.Show(mdicOverlayGroups(iOverlayIndex).GetTest6(), "02_115")

				oPolygon.Dispose()
				oPolygon = Nothing


				If iTestCounter = 380 Then
					''''''''''Exit For
				End If
			Next


			Return False
			'	System.Windows.Forms.MessageBox.Show("!!", "04_160")
			'''''''''''''''''''''''''''''''
			TplnOverlayPgon.CreateMainDataTable()
			'System.Windows.Forms.MessageBox.Show("mdicParcelLotExproPgons.Count" & vbCrLf & CStr(mdicParcelLotExproPgons.Count), "04_217")
			For Each oOverlayPgon In mdicParcelLotExproPgons.Values
				oOverlayPgon.AddDataToMainTable()
			Next
			'	System.Windows.Forms.MessageBox.Show("mdicParcelLotExproPgons.Count" & vbCrLf & CStr(TplnOverlayPgon.MainDataTable.Rows.Count) & vbCrLf & CStr(TplnOverlayPgon.MainView.Count), "04_218")

			Return True


		End Function


		Public Shared Function GetLanduseIDByLotID(ByVal iStatus As DMAcadExt.enTopoPurpose, ByVal iLotTopoID As Integer) As Integer
			Dim dicLots As TPlanGraph.TplnLots = Lots(iStatus)
			If dicLots.ContainsKey(iLotTopoID) Then
				Dim oLot As TplnLot = dicLots.Item(iLotTopoID)
				If oLot.InPlan Then
					Return oLot.LanduseID
				Else
					Return -1
				End If
			Else
				DMAcadExt.AcadDocument.WriteMessage("GetLanduseIDByLotID: Lot " & CStr(iLotTopoID) & " was not found")
				Return -1
			End If
		End Function
		Public Shared Function GetLot(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iLotTopoID As Integer) As TplnLot
			Dim dicLots As TPlanGraph.TplnLots = Lots(iTopoPurpose)
			Dim oLot As TplnLot = Nothing
			If dicLots IsNot Nothing AndAlso dicLots.TryGetValue(iLotTopoID, oLot) Then
				Return oLot
			Else
				Return Nothing
			End If
		End Function
		Public Shared Function GetLusePgon(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal iLusePgonTopoID As Integer) As TplnLusePgon
			Dim dicLusePgons As IDictionary(Of Integer, TplnLusePgon) = zzGetLusePgons(iTopoPurpose, False)
			If dicLusePgons IsNot Nothing Then
				'  MessageBox.Show(dicLusePgons.Count.ToString(), "12_100")

			End If

			If dicLusePgons IsNot Nothing AndAlso dicLusePgons.ContainsKey(iLusePgonTopoID) Then
				Dim oLusePgon As TplnLusePgon = dicLusePgons.Item(iLusePgonTopoID)
				Return oLusePgon
			Else
				Return Nothing
			End If
		End Function
		Public Shared Function GetOverlayPgon(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex, ByVal iUnionTopoID As Integer) As TplnOverlayPgon
			Dim dicUnionPgons As TPlanGraph.TplnOverlayPgons = zzOverlayPgons(iOverlayIndex)
			'	MessageBox.Show(iOverlayIndex.ToString() & ":" & CStr(iUnionTopoID), "04_870")
			If dicUnionPgons IsNot Nothing AndAlso dicUnionPgons.ContainsKey(iUnionTopoID) Then
				Dim oUnionPgon As TplnOverlayPgon = dicUnionPgons.Item(iUnionTopoID)
				Return oUnionPgon
			Else
				Return Nothing
			End If
		End Function
		Public Shared Function GetStatus(ByVal iTopologyID As Integer) As enTopoPurpose
			If iTopologyID < miTopoRange Then
				Return CType(iTopologyID, enTopoPurpose)
			ElseIf iTopologyID < miTopoRange * miTopoRange Then
				Return CType(iTopologyID Mod miTopoRange, enTopoPurpose)
			Else
				Return CType((iTopologyID - miTopoRange * miTopoRange) Mod miTopoRange, enTopoPurpose)
			End If
		End Function

		Public Shared Function GetTopoPolygon(ByVal iTopoID As Integer, ByVal iTopoDefID As DMAcadExt.TopoDefID) As TplnTopoPgon

			If iTopoDefID.TopoIsBase Then
				Dim iBaseID As DMAcadExt.enTopoPurpose = CType(iTopoDefID.ID, DMAcadExt.enTopoPurpose)
				'	System.Windows.Forms.MessageBox.Show(iBaseID.ToString() & vbCrLf & CStr(iTopoID) & vbCrLf & CStr(iTopoDefID.ID) & vbCrLf & CStr(iTopoDefID.TopoIsBase) & vbCrLf & CStr(iTopoDefID.TopoIsDissolve), "27_452")
				Select Case iBaseID
					Case DMAcadExt.enTopoPurpose.Parcel
						Return GetParcel(iTopoID)
					Case DMAcadExt.enTopoPurpose.Approved, DMAcadExt.enTopoPurpose.Proposed
						Return GetLot(DMAcadExt.enTopoPurpose.Approved, iTopoID)
					Case DMAcadExt.enTopoPurpose.Proposed
						Return GetLot(DMAcadExt.enTopoPurpose.Proposed, iTopoID)
					Case DMAcadExt.enTopoPurpose.Mitham
						'System.Windows.Forms.MessageBox.Show(iTopoDefID.ToString() & vbCrLf & CStr(iTopoID))
						Return GetRegion(iTopoID)
					Case Else
						Return Nothing
				End Select
			ElseIf iTopoDefID.TopoIsDissolve Then
				Dim tTopoDefID As DMAcadExt.TopoDefID = iTopoDefID.SourceID
				Dim iSourceID As DMAcadExt.enTopoPurpose = CType(tTopoDefID.ID, DMAcadExt.enTopoPurpose)
				'	MessageBox.Show(CStr(iTopoDefID.ID) & ":" & CStr(iTopoDefID.SourceID.ID) & ":" & iSourceID.ToString(), "01_688")
				Select Case iSourceID
					Case DMAcadExt.enTopoPurpose.Parcel
						Return mdicBlocks.TopoItem(iTopoID)
					Case DMAcadExt.enTopoPurpose.Approved, DMAcadExt.enTopoPurpose.Proposed
						Return GetLusePgon(iSourceID, iTopoID)
					Case Else
						Return Nothing
				End Select
			Else
				Dim iOverlayIndex As DMAcadExt.enOverlayIndex = iTopoDefID.OverlayIndex
				Return GetOverlayPgon(iOverlayIndex, iTopoID)
			End If
		End Function
		Public Shared Function GetTopoPolygon(ByVal iTopoID As Integer, iMapTheme As DMAcadExt.enMapTheme) As TplnTopoPgon
			'	System.Windows.Forms.MessageBox.Show(iTopoDefID.ToString() & vbCrLf & CStr(iTopoID) & vbCrLf & CStr(iTopoDefID.ID) & vbCrLf & CStr(iTopoDefID.TopoIsBase) & vbCrLf & CStr(iTopoDefID.TopoIsDissolve), "27_452")

			Select Case iMapTheme
				Case DMAcadExt.enMapTheme.LotApproved
					Return GetLot(DMAcadExt.enTopoPurpose.Approved, iTopoID)
				Case DMAcadExt.enMapTheme.LotProposed
					Return GetLot(DMAcadExt.enTopoPurpose.Proposed, iTopoID)
				Case DMAcadExt.enMapTheme.LanduseApproved
					Return GetLusePgon(DMAcadExt.enTopoPurpose.Approved, iTopoID)
				Case DMAcadExt.enMapTheme.Parcels
					Return GetParcel(iTopoID)
				Case DMAcadExt.enMapTheme.ParcelsXApproved
					Return GetOverlayPgon(DMAcadExt.enOverlayIndex.ApprFDO_Overlay, iTopoID)
				Case DMAcadExt.enMapTheme.ParcelsXExpro
					Return GetOverlayPgon(DMAcadExt.enOverlayIndex.ApprFDO_Overlay, iTopoID)
				Case DMAcadExt.enMapTheme.Expropriation
					Return GetExpro(iTopoID)
				Case DMAcadExt.enMapTheme.ParcelsXExproXLots
					Return GetExproOverlay(iTopoID)
				Case DMAcadExt.enMapTheme.BN
					Return TplnBNProject.GetBNPgon(iTopoID)
				Case Else
					Return Nothing
			End Select



		End Function
		Public Shared Function GetTopologyName(iMapTheme As DMAcadExt.enMapTheme, bLine As Boolean) As String
			Dim tMapThemeData As DMAcadExt.MapThemeData = New DMAcadExt.MapThemeData()
			If mdicMapThemes.TryGetValue(iMapTheme, tMapThemeData) Then
				If bLine Then
					Return tMapThemeData.LineTopoName
				Else
					Return tMapThemeData.TopoName
				End If
			Else
				Return Nothing
			End If
		End Function
		Public Shared Function GetPlan(ByVal iPlanTopoID As Integer, Optional sTag As String = "") As TplnPlan
			Dim oPlan As TplnPlan = Nothing
			If mdicPlans Is Nothing Then
				System.Windows.Forms.MessageBox.Show(sTag & vbCrLf & "!!!Design Error: mdicPlans Is Nothing", "01_827c")
				Return Nothing
			Else
				If mdicPlans.TryGetValue(iPlanTopoID, oPlan) Then
					'DMCommon.Debug.MsgBox("!GetPlan", iPlanTopoID, oPlan.Name)
					DMCommon.Debug.ExcelLog.SetNextValue(0, "!+GetPlan", iPlanTopoID, oPlan.TopoID, oPlan.Name)

					Return oPlan
				Else
					Return Nothing
				End If
			End If

		End Function
		Public Shared Function GetRegion(ByVal iRegionTopoID As Integer, Optional sTag As String = "") As TplnRegion
			Dim oRegion As TplnRegion = Nothing
			If mdicRegions Is Nothing Then
				System.Windows.Forms.MessageBox.Show(sTag & vbCrLf & "!!!Design Error: mdicPlans Is Nothing", "01_828d")
				Return Nothing
			Else
				If mdicRegions.TryGetValue(iRegionTopoID, oRegion) Then
					Return oRegion
				Else
					Return Nothing
				End If
			End If

		End Function
		Public Shared Function GetParcel(ByVal iParcelTopoID As Integer, Optional sTag As String = "") As TplnParcel
			Dim oParcel As TplnParcel = Nothing
			If mdicParcels Is Nothing Then
				System.Windows.Forms.MessageBox.Show(sTag & vbCrLf & "!!!Design Error: mdicParcels Is Nothing", "01_822b")
				Return Nothing
			Else
				If mdicParcels.TryGetValue(iParcelTopoID, oParcel) Then
					Return oParcel
				Else
					Return Nothing
				End If
			End If

		End Function
		Public Shared Function GetRegion(ByVal iTopoID As Integer) As TplnRegion
			Dim oRegion As TplnRegion = Nothing
			If mdicRegions Is Nothing Then
				System.Windows.Forms.MessageBox.Show("!!!Design Error: mdicParcels Is Nothing", "01_822b")
				Return Nothing
			Else
				If mdicRegions.TryGetRegion(iTopoID, oRegion) Then
					Return oRegion
				Else
					For Each oRegion1 As TplnRegion In mdicRegions.Values
						'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!+mdicRegions", oRegion1.TopoID, oRegion1.RegionNo, oRegion1.RegionName)
					Next
					For Each iKey As Integer In mdicRegions.Keys
						'130126 DMCommon.Debug.ExcelLog.SetNextValue(0, "!+mdicKey", iKey)
					Next
					Return Nothing
				End If
			End If

		End Function
		Public Shared Function GetParcelByPgon(ByVal iTopoID As Integer, ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As TplnParcel
			Dim iParcelTopoID As Integer
			Dim oUnionPgon As TplnOverlayPgon
			Dim dicUnionPgons As TPlanGraph.TplnOverlayPgons = mdicOverlayPgons(iOverlayIndex)
			If dicUnionPgons.ContainsKey(iTopoID) Then
				oUnionPgon = dicUnionPgons.Item(iTopoID)
				iParcelTopoID = oUnionPgon.ParcelTopoID
				If mdicParcels.ContainsKey(iParcelTopoID) Then
					Dim oParcel As TplnParcel = mdicParcels.Item(iParcelTopoID)
					Return oParcel
				Else
					Return Nothing
				End If
			Else
				Return Nothing
			End If

		End Function
		Public Shared Function GetExpro(ByVal iExproTopoID As Integer) As TplnExpro
			Dim oExpro As TplnExpro = Nothing
			If mdicExpros Is Nothing Then
				System.Windows.Forms.MessageBox.Show("!!!Design Error: mdicExpro Is Nothing", "01_818")
				Return Nothing
			Else
				If mdicExpros.TryGetValue(iExproTopoID, oExpro) Then
					Return oExpro
				Else
					Return Nothing
				End If
			End If

		End Function
		Public Shared Function GetMerhav(ByVal iMerhavTopoID As Integer) As TplnMerhav
			Dim oMerhav As TplnMerhav = Nothing
			If mdicMerhav Is Nothing Then
				System.Windows.Forms.MessageBox.Show("!!!Design Error: mdicMerhav Is Nothing", "01_818")
				Return Nothing
			Else
				If mdicMerhav.TryGetValue(iMerhavTopoID, oMerhav) Then
					Return oMerhav
				Else
					Return Nothing
				End If
			End If

		End Function
		Public Shared Function GetExproOverlay(ByVal iTopoID As Integer) As TplnOverlayPgon
			Dim oExproOverlay As TplnOverlayPgon = Nothing
			If mdicParcelLotExproPgons Is Nothing Then
				System.Windows.Forms.MessageBox.Show("!!!Design Error: mdicExpro Is Nothing", "01_818")
				Return Nothing
			Else
				If mdicParcelLotExproPgons.TryGetValue(iTopoID, oExproOverlay) Then
					Return oExproOverlay
				Else
					Return Nothing
				End If
			End If

		End Function
		Public Shared Function GetLanduseItem(ByVal iLanduseID As Integer) As TPlServerDB.TPlLanduseItem
			If (mdicLanduses IsNot Nothing) AndAlso mdicLanduses.ContainsKey(iLanduseID) Then
				Try
					Dim oLanduseItem As TPlServerDB.TPlLanduseItem = DirectCast(mdicLanduses.Item(iLanduseID), TPlServerDB.TPlLanduseItem)
					Return oLanduseItem
				Catch oEx As Exception
					DMAcadExt.AcadDocument.WriteMessage(oEx.Message & vbCrLf & oEx.StackTrace)
					Return Nothing
				End Try
			Else
				If iLanduseID <> 0 Then
					DMAcadExt.AcadDocument.WriteMessage("Landuse " & CStr(iLanduseID) & " was not found!" & vbCrLf)
				End If
				Return Nothing
			End If

		End Function

		Public Shared Sub WriteMessageBox(ByVal sMsg As String, ByVal sTitle As String)
			Dim sMsgTitle As String
			sMsgTitle = sTitle & " - " & sMsg
			DMAcadExt.AcadDocument.WriteMessage(sMsgTitle)
		End Sub
		Public Shared Function GetDoubleNamesCriteria(ByVal iStatus As DMAcadExt.enTopoPurpose) As String
			Select Case iStatus
				Case DMAcadExt.enTopoPurpose.Parcel
					Return mdicParcels.GetDoubleNamesCriteria
				Case DMAcadExt.enTopoPurpose.Approved
					Return mdicLotsAppr.GetDoubleNamesCriteria
				Case DMAcadExt.enTopoPurpose.Proposed
					Return mdicLotsProp.GetDoubleNamesCriteria
				Case Else
					Return String.Empty
			End Select
		End Function
		Public Shared Function FindPgonByPoint(ByVal sTopoName As String) As Integer
			Dim oTopology As TopologyModel = DMAcadExt.AcadMapApp.GetTopology(sTopoName)
			'	Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock
			If oTopology IsNot Nothing Then
				Dim tPoint As Autodesk.AutoCAD.Geometry.Point3d
				If GetPoint("Select point ... ", tPoint) Then
					Dim iTopoID As Integer
					If oTopology.Status = Status.Closed Then
						Try
							oTopology.Open(Autodesk.Gis.Map.Topology.OpenMode.ForRead)
						Catch oMapEx As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - FindPgonByPoint_01")
							Return 0
						End Try
					End If

					If oTopology.Status = Status.OpenForRead Then
						Dim oPolygon As Polygon
						Try
							oPolygon = oTopology.FindPolygon(tPoint)
							If oPolygon IsNot Nothing Then
								DMAcadExt.AcadDocument.WriteMessage("29_546: " & CStr(oPolygon.ID) & vbCrLf)
								iTopoID = oPolygon.ID
							Else
								iTopoID = 0
							End If

						Catch oMapEx As Autodesk.Gis.Map.MapException
							If oMapEx.ErrorCode = 3 Then
								'  System.Windows.Forms.MessageBox.Show("Polygon was not found" & vbCrLf & CStr(tPoint.X) & ":" & CStr(tPoint.Y), "TplnProject - FindPgonByPoint")
								DMAcadExt.AcadDocument.WriteMessage("Polygon was not found" & vbCrLf & CStr(tPoint.X) & ":" & CStr(tPoint.Y))
							Else
								DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - FindPgonByPoint_02")
							End If
							iTopoID = 0
						End Try
						Try
							oTopology.Close()
						Catch oMapEx As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "TplnProject - FindPgonByPoint_03")
						End Try

						Return iTopoID
					Else
						Return 0
					End If
				Else
					Return 0
				End If
			Else
				Return 0
			End If
		End Function


		Public Shared Function GetPoint(ByVal sPrompt As String, ByRef tPoint As Autodesk.AutoCAD.Geometry.Point3d) As Boolean
			Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = GetEditor()
			Dim oPromptPointOptions As Autodesk.AutoCAD.EditorInput.PromptPointOptions = New Autodesk.AutoCAD.EditorInput.PromptPointOptions(vbCrLf & sPrompt)
			''  ptopts.BasePoint = New Autodesk.AutoCAD.Geometry.Point3d(1, 1, 1)
			oPromptPointOptions.UseBasePoint = False
			oPromptPointOptions.UseDashedLine = True
			oPromptPointOptions.AllowArbitraryInput = False

			Dim oPromptPointResult As Autodesk.AutoCAD.EditorInput.PromptPointResult = oEditor.GetPoint(oPromptPointOptions)


			Dim oaOutput(1) As Autodesk.AutoCAD.Geometry.Point3d
			If oPromptPointResult.Status <> Autodesk.AutoCAD.EditorInput.PromptStatus.Cancel Then
				tPoint = oPromptPointResult.Value
				Return True
			Else
				Return False
			End If

		End Function
		Public Shared Function GetEditor() As Autodesk.AutoCAD.EditorInput.Editor
			Return Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
		End Function
		Public Shared ReadOnly Property MerhavExists As Boolean
			Get
				Return mbMerhavExists
			End Get
		End Property
		Public Shared Property FormatsSetting() As Integer()
			Get
				Dim sSettting As String = Microsoft.VisualBasic.GetSetting(Common.AppName, Common.SettingSectionName, msFormatsSettingKey, "1,1,1")
				Dim saSetting() As String = Split(sSettting, ",")
				Dim iFormatID As Integer
				Dim iaOut(miFormatsUB) As Integer

				For iIndex As Integer = 0 To Math.Min(miFormatsUB, saSetting.GetUpperBound(0))
					Try
						iFormatID = Math.Max(Convert.ToInt32(saSetting(iIndex)), 1)
					Catch oEx As Exception
						iFormatID = 1
					End Try
					iaOut(iIndex) = iFormatID
				Next
				Return iaOut
			End Get
			Set(ByVal iaValue As Integer())
				Dim sSetting As String = String.Empty
				Dim iValue As Integer
				For iIndex As Integer = 0 To Math.Min(miFormatsUB, iaValue.GetUpperBound(0))
					If sSetting.Length <> 0 Then sSetting &= ","
					iValue = Math.Max(iaValue(iIndex), 1)

					sSetting &= CStr(iValue)
				Next
				Microsoft.VisualBasic.SaveSetting(Common.AppName, Common.SettingSectionName, msFormatsSettingKey, sSetting)
			End Set
		End Property
		Public Shared Property CoordinateFormatSetting() As String
			Get
				Dim sSettting As String = Microsoft.VisualBasic.GetSetting(Common.AppName, Common.SettingSectionName, msCoordFormatSettingKey, "0.000")

				Return sSettting
			End Get
			Set(ByVal sValue As String)
				Microsoft.VisualBasic.SaveSetting(Common.AppName, Common.SettingSectionName, msCoordFormatSettingKey, sValue)
			End Set
		End Property
		Public Shared Property AreaFormatSetting() As String
			Get
				Dim sSettting As String = Microsoft.VisualBasic.GetSetting(Common.AppName, Common.SettingSectionName, msAreaFormatSettingKey, "0.000")

				Return sSettting
			End Get
			Set(ByVal sValue As String)
				Microsoft.VisualBasic.SaveSetting(Common.AppName, Common.SettingSectionName, msAreaFormatSettingKey, sValue)
			End Set
		End Property
		Public Shared ReadOnly Property OverlayGroups(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As TplnOverlayGroups
			Get
				Return mdicOverlayGroups(iOverlayIndex)
			End Get
		End Property
		Public Shared Property LandusesFormatID() As Integer
			Get
				Return miLandusesFormatID
			End Get
			Set(ByVal iValue As Integer)
				If miLandusesFormatID <> iValue Then
					miLandusesFormatID = iValue
					mdicLanduses = TPlServerDB.ServerDB.CurrentServerDB.GetItemDict(TPlServerDB.enListType.LanduseD, , , , miLandusesFormatID)
				End If
			End Set
		End Property
		Public Shared Property IsParcelTopology() As Boolean
			Get
				Return mbIsParcelTopology
			End Get
			Set(bValue As Boolean)
				mbIsParcelTopology = bValue
			End Set
		End Property
		Public Shared Property MapThemes() As IDictionary(Of DMAcadExt.enMapTheme, DMAcadExt.MapThemeData)
			Get
				Return mdicMapThemes
			End Get
			Set(bValue As IDictionary(Of DMAcadExt.enMapTheme, DMAcadExt.MapThemeData))
				mdicMapThemes = bValue
			End Set
		End Property
		Public Shared ReadOnly Property MerhavDic As TplnMerhavDic
			Get
				Return mdicMerhav
			End Get
		End Property

		Public Shared Sub ClearPaintNew(iMapTheme As DMAcadExt.enMapTheme) ', ByVal iTopoPurpose As DMAcadExt.enTopoPurpose
			Dim tPaintLayerDef As DMAcadExt.AcadLayerDef = PaintLayerDef(iMapTheme)
			Dim tPLineLayerDef As DMAcadExt.AcadLayerDef = DrawPlineLayerDef(iMapTheme)


			DMAcadExt.AcadTransaction.ClearLayers(tPaintLayerDef)
			DMAcadExt.AcadTransaction.ClearLayers(tPLineLayerDef)

			''''''''''''''''''''''''''''''''''''''''    ClearPaintTemp()
			'Dim oTopoNameCriteria As TopoCreator.TopoNameCriteria = New TopoCreator.TopoNameCriteria(AddressOf TplnTopoPgon.IsPaintTopoName)
			'TopoCreator.DeleteTopologies(oTopoNameCriteria)
		End Sub

		Public Shared Sub ClearPaint(iMapTheme As DMAcadExt.enMapTheme)  ', ByVal iTopoPurpose As DMAcadExt.enTopoPurpose
			Dim sPaintLayer As String = PaintLayerDef(iMapTheme).Name

			DMAcadExt.AcadTransaction.ClearLayerList(sPaintLayer)
			ClearPaintTemp()
			'Dim oTopoNameCriteria As TopoCreator.TopoNameCriteria = New TopoCreator.TopoNameCriteria(AddressOf TplnTopoPgon.IsPaintTopoName)
			'TopoCreator.DeleteTopologies(oTopoNameCriteria)
		End Sub
		Public Shared Sub ClearPaintTemp()
			Dim sPaintLayer As String = PaintTempLayerDef().Name
			Dim oTopoNameCriteria As TopoCreator.TopoNameCriteria = New TopoCreator.TopoNameCriteria(AddressOf TplnTopoPgon.IsPaintTopoName)
			Try
				TopoCreator.DeleteTopologies(oTopoNameCriteria)
			Catch oEx As Exception

			End Try
			'	DMAcadExt.AcadDocument.RegenDoc()
			DMAcadExt.AcadTransaction.ClearLayerList(sPaintLayer)
		End Sub
		Public Shared ReadOnly Property GushimVectorizedRootPath() As String
			Get
				Return msGushimVectorizedRootPath
			End Get

		End Property

		Public Shared ReadOnly Property InitializedServerDB() As Boolean
			Get
				Return mbInitializedServerDB
			End Get
		End Property
		Public Shared Property CoordinateFormat() As String
			Get
				Return msCoordinateFormat
			End Get
			Set(ByVal sValue As String)
				msCoordinateFormat = sValue
			End Set
		End Property
		Public Shared Property AreaFormat() As String
			Get
				Return msAreaFormat
			End Get
			Set(ByVal sValue As String)
				msAreaFormat = sValue
			End Set
		End Property
		Public Shared Property DefaultTopoPurpose As DMAcadExt.enTopoPurpose
			Get
				Return miDefaultTopoPurpose
			End Get
			Set(iValue As DMAcadExt.enTopoPurpose)
				miDefaultTopoPurpose = iValue
			End Set
		End Property
		Public Shared Property OverlayMethod As DMAcadExt.enOverlayMethod
			Get
				Return miOverlayMethod
			End Get
			Set(iValue As DMAcadExt.enOverlayMethod)
				miOverlayMethod = iValue
			End Set
		End Property



		Public Shared Property ParcelGeoMethod As enGeoMethod
			Get
				Return miParcelGeoMethod
			End Get
			Set(iValue As enGeoMethod)
				miParcelGeoMethod = iValue
			End Set
		End Property


		Public Shared Function PaintLayerDef(iMapTheme As DMAcadExt.enMapTheme, Optional iOption As Integer = 0) As DMAcadExt.AcadLayerDef  ', ByVal iTopoPurpose As DMAcadExt.enTopoPurpose
			'  Dim iTopoPurpose As DMAcadExt.enTopoPurpose
			If ((iMapTheme = DMAcadExt.enMapTheme.LotApproved) OrElse (iMapTheme = DMAcadExt.enMapTheme.LanduseApproved)) Then 'AndAlso (iTopoPurpose = DMAcadExt.enTopoPurpose.Approved)
				If Not mtPaintApprLayerDef.Exists Then
					mtPaintApprLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.PaintHatchAppr)
				End If
				Return mtPaintApprLayerDef
			ElseIf iMapTheme = DMAcadExt.enMapTheme.LotProposed Then  'AndAlso iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed
				If Not mtPaintPropLayerDef.Exists Then
					mtPaintPropLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.PaintHatchProp)
				End If
				Return mtPaintPropLayerDef
			ElseIf iMapTheme = DMAcadExt.enMapTheme.Parcels Then
				Return New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.PaintHatchParcel)
			ElseIf iMapTheme = DMAcadExt.enMapTheme.Expropriation Then
				Return New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.PaintHatchPgons)
			ElseIf iMapTheme = DMAcadExt.enMapTheme.Ownership Then
				Select Case iOption
					Case 0
						Return New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.PaintHatchPgons)
					Case 1
						Return New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.PaintParagraph19)
					Case 2
						Return New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.PaintLeasing)
					Case 3
						Return New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.PaintParagraph126)
					Case 4
						Return New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.PaintSharedHouse)
					Case Else
						Return New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.PaintHatchPgons)
				End Select

			Else
				Return Nothing
			End If
		End Function
		Public Shared Function DrawPlineLayerDef(iMapTheme As DMAcadExt.enMapTheme) As DMAcadExt.AcadLayerDef  ', ByVal iTopoPurpose As DMAcadExt.enTopoPurpose
			'  Dim iTopoPurpose As DMAcadExt.enTopoPurpose
			If ((iMapTheme = DMAcadExt.enMapTheme.LotApproved) OrElse (iMapTheme = DMAcadExt.enMapTheme.LanduseApproved)) Then 'AndAlso (iTopoPurpose = DMAcadExt.enTopoPurpose.Approved)
				If Not mtDrawPLineApprLayerDef.Exists Then
					mtDrawPLineApprLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.PaintPLineAppr)
				End If
				Return mtDrawPLineApprLayerDef
			ElseIf iMapTheme = DMAcadExt.enMapTheme.LotProposed Then  'AndAlso iTopoPurpose = DMAcadExt.enTopoPurpose.Proposed
				If Not mtDrawPLinePropLayerDef.Exists Then
					mtDrawPLinePropLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.PaintPLineProp)
				End If
				Return mtDrawPLinePropLayerDef
			ElseIf iMapTheme = DMAcadExt.enMapTheme.Parcels Then
				Return New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.PaintHatchParcel)
			ElseIf iMapTheme = DMAcadExt.enMapTheme.Expropriation Then
				Return New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.PaintHatchPgons)

			Else
				Return Nothing
			End If
		End Function

		Public Shared ReadOnly Property PaintTempLayerDef() As DMAcadExt.AcadLayerDef
			Get
				If Not mtPaintTempLayerDef.Exists Then
					mtPaintTempLayerDef = New DMAcadExt.AcadLayerDef(DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.PaintHatchK)
				End If
				Return mtPaintTempLayerDef
			End Get
		End Property
		Public Shared ReadOnly Property Parcels() As TopoManager.TPlanGraph.TplnParcels
			Get

				Return mdicParcels

			End Get
		End Property

		Public Shared ReadOnly Property Expros() As TopoManager.TPlanGraph.TplnExpros
			Get

				Return mdicExpros

			End Get
		End Property

		Public Shared ReadOnly Property Lots(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As TPlanGraph.TplnLots
			Get
				If iTopoPurpose = enTopoPurpose.Approved Then

					Return mdicLotsAppr
				ElseIf iTopoPurpose = enTopoPurpose.Proposed Then
					Return mdicLotsProp
				Else
					System.Windows.Forms.MessageBox.Show("Err #1129: " & iTopoPurpose.ToString(), "TplnProject - Lots")
					Return Nothing
				End If
			End Get
		End Property
		Public Shared Sub ShowOverlayPgons(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex, iParcelID As Integer, iOverlayID As Integer, ByRef mcolExternalEntities As ObjectIdCollection)
			'	DMAcadExt.AcadUtil.AddObjectIDCollection(colTopoLinks, mcolBlockedLinks)

			Dim oParcel As TplnParcel = Nothing
			Dim oLot As TplnLot = Nothing
			Dim oBasicPgon As TplnBasicPgon = Nothing
			Dim colAcObjIDs As ObjectIdCollection
			Dim iTopoPurpose As DMAcadExt.enTopoPurpose = UnionPgonArea.GetTopoPurpose(iOverlayIndex)
			Dim dicLots As TplnLots = Lots(iTopoPurpose)

			DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)
			DMAcadExt.AcadTransaction.Start()
			If iParcelID <> 0 AndAlso mdicParcels.TryGetValue(iParcelID, oParcel) Then
				oBasicPgon = oParcel
			End If


			If iOverlayID <> 0 AndAlso dicLots IsNot Nothing AndAlso dicLots.TryGetValue(iOverlayID, oLot) Then
				oBasicPgon = oLot

			End If

			If oBasicPgon IsNot Nothing Then

				'DMCommon.Debug.MsgBox("221120_4", iParcelID, oBasicPgon.Name, oBasicPgon.Lines.Count)
				colAcObjIDs = oBasicPgon.Lines
				DMAcadExt.AcadTransaction.SetColor(colAcObjIDs, Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByColor, 5S), TriState.True)
				DMAcadExt.AcadUtil.AddObjectIDCollection(mcolExternalEntities, colAcObjIDs)

				Dim oOverlayPgons As TplnOverlayPgons = oBasicPgon.GetOverlayPgons(iOverlayIndex)
				colAcObjIDs = oOverlayPgons.GetEntities()
				DMAcadExt.AcadTransaction.SetColor(colAcObjIDs, Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByColor, 1S), TriState.True)
				DMAcadExt.AcadUtil.AddObjectIDCollection(mcolExternalEntities, colAcObjIDs)
			Else
				DMCommon.Debug.MsgBox("221120_5", mdicParcels.Count, iParcelID)
			End If

			DMAcadExt.AcadTransaction.Terminate()
			DMAcadExt.AcadDocument.Unlock()
			DMAcadExt.AcadDocument.UpdateScreen()


		End Sub
		Public Shared Sub SetExproLuseReport() ' AcadReport.BaseReport
			Dim iRow As Integer = 0
			Dim tRect As Rectangle
			moExcelAppExt = New DMCommon.ExcelAppExt()
			moExcelAppExt.Open()

			DMCommon.Debug.MsgBox("190530_2", mdicParcels.Count, DMCommon.Debug.ColCount(TplnLot.Landuses), moExcelAppExt.WorksheetName)
			'moExcelAppExt.TEST()

			'	moExcelAppExt.SetNextValue(0, "", zzGetText(0, 1), zzGetText(1, 1), zzGetText(2, 1))
			'	moExcelAppExt.SetValueInHeaderRow(iRow, 0, zzGetText(0, 1), zzGetText(1, 1), zzGetText(2, 1))

			For iCol As Integer = 0 To 2
				tRect = New Rectangle(iCol, 0, 0, 1)
				moExcelAppExt.SetValueInHeaderCell(tRect, False, zzGetText(iCol, 1))
			Next
			tRect = New Rectangle(3, 0, 4, 0)
			moExcelAppExt.SetValueInHeaderCell(tRect, False, zzGetText(3, 1))
			zzSubgroupHeader(3)
			Dim iTypeIndex As Integer = 1
			DMCommon.Debug.MsgBox("190605_2", TplnExpro.TypeNames.Count, TplnParcel.ExproTypes.Count)
			For Each tTypeID_Name As KeyValuePair(Of Integer, String) In TplnExpro.TypeNames
				If TplnParcel.ExproTypes.Contains(tTypeID_Name.Key) Then
					'moExcelAppExt.SetValueInRow(10, 3 + 5 * iTypeIndex, tTypeID_Name.Value)
					tRect = New Rectangle(3 + 5 * iTypeIndex, 0, 4, 0)
					moExcelAppExt.SetValueInHeaderCell(tRect, False, tTypeID_Name.Value)
					zzSubgroupHeader(3 + 5 * iTypeIndex)
					iTypeIndex += 1
				End If

			Next


			'Next
			iRow = 2
			Dim i As Integer = 0
			For Each oParcel As TplnParcel In mdicParcels.Values
				If oParcel.HasLanduses Then
					'''''''''''''''''''''''''''''''''''''''''''''''''081219	oParcel.ExproLuseReport(moExcelAppExt, iRow, mdicExproTypes)
					i += 1
				End If


				If iRow > 30 Then
					Exit For
				End If

				If i >= 20 Then
					Exit For
				End If
			Next
		End Sub
		Private Shared Sub zzSubgroupHeader(iFirstColumn As Integer)
			Dim tRect As Rectangle
			tRect = New Rectangle(iFirstColumn, 1, iFirstColumn + 4, 0)
			For iIndex As Integer = 0 To 4
				tRect = New Rectangle(iFirstColumn + iIndex, 1, 0, 0)
				moExcelAppExt.SetValueInHeaderCell(tRect, False, zzGetText(iIndex + 5, 2))
			Next
		End Sub
		Private Shared Function zzGetText(ByVal iItemID As Integer, ByVal iSectionID As Integer) As String
			Dim iResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcRepExproLuse
			Return TPlServerDB.TextResource.GetText(iItemID, iResourceTheme, iSectionID, True)
		End Function

		Public Shared Function GetLusePgons(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As IDictionary(Of Integer, TplnLusePgon)
			Return zzGetLusePgons(iTopoPurpose, False)
		End Function

		Public Shared Function GetPrjLuseSet(iProjectCode As Integer, iDetailNo As Integer, iMapThemeID As DMAcadExt.enMapTheme) As IDictionary(Of Integer, LanduseData)
			Const sSPName As String = "GetColorSchemeSet"

			Dim oaParams(2) As System.Data.Common.DbParameter
			'	Dim oErrOut As System.Data.Common.DbException = Nothing


			oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, iProjectCode)
			oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetail", DbType.Int32, iDetailNo)
			oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prMapThemeID", DbType.Int32, iMapThemeID)
			Dim dicLanduseData As IDictionary(Of Integer, LanduseData) = New Dictionary(Of Integer, LanduseData)
			Dim tLanduseData As LanduseData
			Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sSPName, CommandType.StoredProcedure, oaParams)
			If oDataReader IsNot Nothing Then

				Dim iLanduseID, iColorSchemeID As Integer
				Dim sLanduseName As String

				Do While oDataReader.Read
					tLanduseData = New LanduseData
					iLanduseID = oDataReader.GetInt32(0)
					tLanduseData.LanduseID = iLanduseID
					If oDataReader.IsDBNull(2) Then
						iColorSchemeID = 0
					Else
						iColorSchemeID = oDataReader.GetInt32(2)
					End If
					tLanduseData.ColorSchemeID = iColorSchemeID

					If oDataReader.IsDBNull(1) Then
						sLanduseName = String.Empty
					Else
						sLanduseName = Convert.ToString(iLanduseID) & DMAcadExt.ColorScheme.NameDelim & oDataReader.GetString(1)
					End If
					tLanduseData.LanduseName = sLanduseName

					If iColorSchemeID <> 0 Then
						tLanduseData.LuseColorScheme = New DMAcadExt.ColorScheme(oDataReader, 3, 2.0)
					End If

					'''''''''''''	olviList.Status = enItemStatus.Added
					Dim sPrefix As String = String.Empty

					dicLanduseData.Add(iLanduseID, tLanduseData)


				Loop
				oDataReader.Close()
			End If
			Return dicLanduseData
		End Function
		Private Shared ReadOnly Property zzOverlayPgons(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As TPlanGraph.TplnOverlayPgons
			Get
				Try
					Return mdicOverlayPgons(iOverlayIndex)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show("Err 11244", "TplnProject - zzOverlayPgons")
					Return Nothing
				End Try

			End Get
		End Property
		Private Shared Sub zzFillOverlayGroupTable(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex)
			Const sParcelTopoID As String = "ParcelTopoID"
			'	Const sParcelAcObjID As String = "ParcelAcObjID"
			Const sLotTopoID As String = "LotTopoID"
			Const sPgonCount As String = "PgonCount"
			'		Dim tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
			If mdicOverlayGroups(iOverlayIndex) IsNot Nothing Then
				Dim iTopoPurpose As DMAcadExt.enTopoPurpose = UnionPgonArea.GetTopoPurpose(iOverlayIndex)
				If moOverlayTables(iOverlayIndex) Is Nothing Then
					moOverlayTables(iOverlayIndex) = New System.Data.DataTable("Overlay_" & iOverlayIndex.ToString())
					With moOverlayTables(iOverlayIndex).Columns
						.Add(TplnParcel.BlockFullFieldName, GetType(System.String))              '0
						.Add(TplnParcel.NameFieldName, GetType(System.String))             '1
						.Add(TplnLot.NameFieldName, GetType(System.String))                '2
						.Add(TplnParcel.LanduseIDFieldName, GetType(System.Int32))         '3
						.Add(TplnParcel.LanduseNameFieldName, GetType(System.String)) '		'4
						.Add(TplnParcel.LegalAreaFieldName, GetType(System.Double))     '5
						.Add(TopoReader.msAreaFldName, GetType(System.Double))               '6
						.Add(TopoReader.msCalcAreaFldName, GetType(System.Double))        '7
						.Add(TopoReader.msCalcArea2FldName, GetType(System.Double))       '8
						.Add(TopoReader.msRoundedAreaFldName, GetType(System.Double))     '9
						.Add(TplnLot.msCalcGroupArea2FieldName, GetType(System.Double)) '10

						.Add(sPgonCount, GetType(System.Int32))                              '11
						.Add(TplnParcel.msInPlanFieldName, System.Type.GetType("System.Boolean"))           '12
						.Add(sParcelTopoID, GetType(System.Int32))                           '13
						.Add(sLotTopoID, GetType(System.Int32))                              '14
						.Add(TplnParcel.ParcelOrderFieldName, GetType(System.Int32))       '15
						.Add(TplnParcel.msLotOrderFieldName, GetType(System.Int32))          '16
						.Add(TplnParcel.LanduseOrderFieldName, GetType(System.Int32))            '17
						.Add(TplnParcel.BlockFieldName, GetType(System.Int32))             '18
						.Add(TplnParcel.BlockAddFieldName, GetType(System.Int32))             '19
						.Add(TplnLot.msRegionFieldName, GetType(System.Int32))
					End With
				Else
					moOverlayTables(iOverlayIndex).Clear()
				End If
				Dim iParcelTopoID As Integer
				Dim iLotTopoID As Integer
				Dim oParcel As TplnParcel
				Dim oLot As TplnLot
				Dim oLanduse As TplnLanduse
				'	Dim oColorScheme As DMAcadExt.ColorScheme
				Dim sTest As String
				Dim oNewRow As System.Data.DataRow
				For Each oOverlayGroup As TplnOverlayGroup In mdicOverlayGroups(iOverlayIndex).Values
					sTest = ""
					iParcelTopoID = oOverlayGroup.ParcelID
					iLotTopoID = oOverlayGroup.LotID
					oParcel = GetParcel(iParcelTopoID)
					If iLotTopoID <> 0 Then
						oLot = GetLot(iTopoPurpose, iLotTopoID)
					Else
						oLot = Nothing
					End If

					If oParcel IsNot Nothing Then
						Try
							oNewRow = moOverlayTables(iOverlayIndex).NewRow()
							With oNewRow
								.Item(TplnParcel.BlockFullFieldName) = oParcel.BlockFull
								.Item(TplnParcel.NameFieldName) = oParcel.Name

								If oLot IsNot Nothing Then
									.Item(TplnLot.NameFieldName) = oLot.Name
									.Item(TplnParcel.LanduseIDFieldName) = oLot.LanduseID
									sTest = "A"
									oLanduse = oLot.Landuse
									sTest = "B"
									If oLanduse IsNot Nothing Then
										.Item(TplnParcel.LanduseNameFieldName) = oLanduse.GetNameNew(iTopoPurpose)
										sTest = "Ba"
										.Item(TplnParcel.LanduseOrderFieldName) = oLanduse.GetOrder(iTopoPurpose)
									End If

									sTest = "C"
									.Item(TplnParcel.msLotOrderFieldName) = oLot.Order
									.Item(TplnLot.msRegionFieldName) = oLot.RegionNo

								End If
								.Item(TplnParcel.LegalAreaFieldName) = oParcel.LegalArea(False)

								.Item(TopoReader.msAreaFldName) = oOverlayGroup.AcadArea
								.Item(TopoReader.msCalcAreaFldName) = oOverlayGroup.CalcArea
								.Item(TopoReader.msCalcArea2FldName) = oOverlayGroup.CalcArea2
								.Item(TopoReader.msRoundedAreaFldName) = oOverlayGroup.RoundedArea
								.Item(TplnLot.msCalcGroupArea2FieldName) = oOverlayGroup.CalcGroupArea2

								.Item(sPgonCount) = oOverlayGroup.PolygonCount
								.Item(TplnParcel.msInPlanFieldName) = oOverlayGroup.IsInPlan

								.Item(sParcelTopoID) = iParcelTopoID
								.Item(sLotTopoID) = iLotTopoID
								.Item(TplnParcel.BlockFieldName) = oParcel.BlockNo
								.Item(TplnParcel.BlockAddFieldName) = oParcel.BlockAdd
								.Item(TplnParcel.ParcelOrderFieldName) = oParcel.Order

							End With
							moOverlayTables(iOverlayIndex).Rows.Add(oNewRow)
						Catch oEx As System.Exception
							System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & sTest, "1:FillOverlayGroupTable-TplnProject")
						End Try
					End If
				Next
			End If
		End Sub
		Public Shared Sub FillMapLayerODTable()
			Const sODTableName As String = "LayerDef"
			'MessageBox.Show(CStr(TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode) & ":" & CStr(TPlServerDB.ServerDB.CurrentProjectDB.DetailNo), "01_988")
			Dim dicLots As TplnLots = Lots(DMAcadExt.enTopoPurpose.Approved)
			Dim oODTables As Tables = Autodesk.Gis.Map.HostMapApplicationServices.Application.ActiveProject.ODTables
			If oODTables.IsTableDefined(sODTableName) Then


				Dim oODTable As Autodesk.Gis.Map.ObjectData.Table = oODTables.Item(sODTableName)
				Dim colODRecords As Records
				Dim oEnum As IEnumerator
				Dim sComText As String = "SELECT lt_Name FROM prjUserConstData WHERE (ProjectCode = " & Convert.ToString(TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode) & ") AND (Detail = " & Convert.ToString(TPlServerDB.ServerDB.CurrentProjectDB.DetailNo) & ")"
				Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
				Dim sLT_Name As String = Nothing
				Dim sLanduseName As String = Nothing
				Try
					If oDataReader IsNot Nothing Then
						If oDataReader.Read Then
							If Not oDataReader.IsDBNull(0) Then
								sLT_Name = oDataReader.GetString(0)
							End If
						End If
						oDataReader.Close()
					End If
					For Each oLot As TplnLot In dicLots.Values
						'oLot.CalcArea

						Dim oMapValue As Autodesk.Gis.Map.Utilities.MapValue
						Dim oNewODRecord As Record
						Try
							oNewODRecord = Autodesk.Gis.Map.ObjectData.Record.Create
							oODTable.InitRecord(oNewODRecord)
							sLanduseName = oLot.LanduseName
							DMAcadExt.AcadDocument.WriteMessage("LLL# " & sLanduseName)
							If sLanduseName IsNot Nothing Then
								oMapValue = oNewODRecord.Item(3)
								oMapValue.Assign(sLanduseName)
							End If


							If sLT_Name IsNot Nothing Then
								oMapValue = oNewODRecord.Item(9)
								oMapValue.Assign(sLT_Name)
							End If

							oMapValue = oNewODRecord.Item(10)
							oMapValue.Assign("-")

							oMapValue = oNewODRecord.Item(11)
							oMapValue.Assign(Convert.ToDouble(oLot.LanduseID))
							oMapValue = oNewODRecord.Item(12)
							oMapValue.Assign(Convert.ToDouble(oLot.LanduseID))
							oODTable.AddRecord(oNewODRecord, oLot.CentroidAcObjID)
						Catch oMapEx As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "prjPrograms - zzAddObjectData")
						End Try
						colODRecords = oODTable.GetObjectTableRecords(0, oLot.CentroidAcObjID, Autodesk.Gis.Map.Constants.OpenMode.OpenForWrite, False)
						If colODRecords IsNot Nothing Then
							oEnum = colODRecords.GetEnumerator()
							oEnum.Reset()
							If oEnum.MoveNext() Then
								DMAcadExt.AcadDocument.WriteDebugMessage("ODD_" & colODRecords.CurrentObjectId.ToString() & CStr(colODRecords.Count))
							End If

							'colODRecords.UpdateRecord(oNewODRecord)
							For Each oODRecord As Autodesk.Gis.Map.ObjectData.Record In colODRecords

								If oODRecord.TableName = sODTableName Then

									'	System.Windows.Forms.MessageBox.Show("END of GetODRecord", "01_152! ")

								End If
							Next
							'''''''''''''''colODRecords.UpdateRecord(oNewODRecord)
						End If
					Next
				Catch oEx As Exception
					MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "03_127")
				End Try
			Else
				MessageBox.Show("OD Table was not found", "03_178")
			End If

		End Sub
		Public Shared Sub FillMapLanduseODTable()
			Const sODTableName As String = "LotLanduse"
			Dim iTeSt As Integer = 0
			'MessageBox.Show(CStr(TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode) & ":" & CStr(TPlServerDB.ServerDB.CurrentProjectDB.DetailNo), "01_988")
			Dim dicLots As TplnLots = Lots(DMAcadExt.enTopoPurpose.Approved)
			Dim oODTables As Tables = Autodesk.Gis.Map.HostMapApplicationServices.Application.ActiveProject.ODTables
			If oODTables.IsTableDefined(sODTableName) Then


				Dim oODTable As Autodesk.Gis.Map.ObjectData.Table = oODTables.Item(sODTableName)
				Dim colODRecords As Records
				Dim oEnum As IEnumerator
				'     Dim sComText As String = "SELECT lt_Name FROM prjUserConstData WHERE (ProjectCode = " & Convert.ToString(TPlServerDB.ServerDB.CurrentProjectDB.ProjectCode) & ") AND (Detail = " & Convert.ToString(TPlServerDB.ServerDB.CurrentProjectDB.DetailNo) & ")"
				'      Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
				'     Dim sLT_Name As String = Nothing
				Dim sLanduseName As String = Nothing
				Dim sPlanName As String = Nothing
				Dim sPlanNameHebRev As String = Nothing
				Dim oHebText As DMCommon.HebrewTrans

				Try
					'
					'  MessageBox.Show(CStr(dicLots.Count) & ":" & CStr(150), "01_985")
					For Each oLot As TplnLot In dicLots.Values
						'oLot.CalcArea

						Dim oMapValue As Autodesk.Gis.Map.Utilities.MapValue
						Dim oNewODRecord As Record
						Try
							oNewODRecord = Autodesk.Gis.Map.ObjectData.Record.Create
							oODTable.InitRecord(oNewODRecord)

							oMapValue = oNewODRecord.Item(0)
							oMapValue.Assign(oLot.LanduseID)

							sLanduseName = oLot.LanduseName
							' DMAcadExt.AcadDocument.WriteMessage("LLL# " & sLanduseName)
							If sLanduseName IsNot Nothing Then
								oMapValue = oNewODRecord.Item(1)
								oMapValue.Assign(sLanduseName)
							End If

							oMapValue = oNewODRecord.Item(2)
							oMapValue.Assign(oLot.AcadArea(True))

							sPlanName = oLot.PlanName

							If Not String.IsNullOrEmpty(sPlanName) Then
								oHebText = New DMCommon.HebrewTrans(sPlanName, False)
								oMapValue = oNewODRecord.Item(3)
								oMapValue.Assign(oHebText.GetWinDest(False))
								'   DMAcadExt.AcadDocument.WriteMessage("PPP# " & sPlanName)
							Else
								DMAcadExt.AcadDocument.WriteMessage("PPP# " & "Nothing")
							End If

							oODTable.AddRecord(oNewODRecord, oLot.CentroidAcObjID)
						Catch oMapEx As Autodesk.Gis.Map.MapException
							DMAcadExt.AcadErrCode.ShowMapError(oMapEx.ErrorCode, False, "prjPrograms - zzAddObjectData")
						End Try
						iTeSt = +1
						If iTeSt = 5 Then
							Exit For
						End If
						If False Then


							colODRecords = oODTable.GetObjectTableRecords(0, oLot.CentroidAcObjID, Autodesk.Gis.Map.Constants.OpenMode.OpenForWrite, False)
							If colODRecords IsNot Nothing Then
								oEnum = colODRecords.GetEnumerator()
								oEnum.Reset()
								If oEnum.MoveNext() Then
									DMAcadExt.AcadDocument.WriteDebugMessage("ODD_" & colODRecords.CurrentObjectId.ToString() & CStr(colODRecords.Count))
								End If

								'colODRecords.UpdateRecord(oNewODRecord)
								For Each oODRecord As Autodesk.Gis.Map.ObjectData.Record In colODRecords

									If oODRecord.TableName = sODTableName Then

										'	System.Windows.Forms.MessageBox.Show("END of GetODRecord", "01_152! ")

									End If
								Next
								'''''''''''''''colODRecords.UpdateRecord(oNewODRecord)
							End If
						End If
					Next
				Catch oEx As Exception
					MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "03_128")
				End Try
			Else
				MessageBox.Show("OD Table was not found", "03_178")
			End If

		End Sub
		Private Shared Function zzGetLusePgons(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, ByVal bNew As Boolean) As IDictionary(Of Integer, TplnLusePgon)

			If mdicLusePgonsAppr IsNot Nothing Then

				'  MessageBox.Show(mdicLusePgonsAppr.Count.ToString(), "03_147q")
			End If
			Select Case iTopoPurpose
				Case DMAcadExt.enTopoPurpose.Approved
					If bNew Then
						If mdicLusePgonsAppr Is Nothing Then
							mdicLusePgonsAppr = New Dictionary(Of Integer, TplnLusePgon)
						Else
							mdicLusePgonsAppr.Clear()
						End If
					End If
					Return mdicLusePgonsAppr
				Case DMAcadExt.enTopoPurpose.Proposed
					If bNew Then
						If mdicLusePgonsProp Is Nothing Then
							mdicLusePgonsProp = New Dictionary(Of Integer, TplnLusePgon)
						Else
							mdicLusePgonsProp.Clear()
						End If
					End If
					Return mdicLusePgonsProp
				Case Else
					Return Nothing
			End Select
		End Function

#End Region
#Region "Private members"

		Private Shared Function zzGetTopoID(ByVal oPolygon As Polygon) As Integer
			Try
				If oPolygon Is Nothing Then
					Return 0
				Else
					Return oPolygon.ID
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show("!!!STOPPPP!!!", "zzGetTopoID")
				Return 0
			End Try

		End Function

		Private Shared Sub zzCalculateOverlayGroups(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean, iRegion As Integer)
			If bMerge OrElse bUnion OrElse bFDO_Overlay Then
				Dim baOverlayArray() As Boolean = UnionPgonArea.GetOverlayArray(bMerge, bUnion, bFDO_Overlay, bApproved, bProposed)
				For iOverlayIndex As DMAcadExt.enOverlayIndex = 0 To DMAcadExt.enOverlayIndex.OverlayIndexUB
					If baOverlayArray(iOverlayIndex) Then
						If mdicOverlayGroups(iOverlayIndex) IsNot Nothing Then
							'!!!!!!!!!!!!!!!!!!!!System.Windows.Forms.MessageBox.Show("CalculateOverlayGroups" & vbCrLf & iOverlayIndex.ToString() & vbCrLf & CInt(mdicOverlayGroups(iOverlayIndex).Count), "04_365")
							mdicOverlayGroups(iOverlayIndex).Calculate2(iRegion)
						Else
							System.Windows.Forms.MessageBox.Show(iOverlayIndex.ToString(), "01_554k")
						End If

					End If
				Next





			End If

		End Sub
		Private Shared Function zzGetRegionTopology() As TopologyModel
			' Dim iRow As Integer

			Dim oRegionTopology As TopologyModel '= TopoManager.TopoCreator.GetOpenedTopology(TplnRegion.GetTopoName(), Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			Dim sRegionTopoName As String
			'  Dim oRegion As TplnRegion
			Dim oRegionPgon As Polygon = Nothing
			Dim bProx As Boolean
			sRegionTopoName = TplnRegion.GetTopoName()

			'	DMCommon.Debug.MsgBox("020121_1", sRegionTopoName, bProx)
			If String.IsNullOrEmpty(sRegionTopoName) Then
				sRegionTopoName = TplnRegion.GetProxTopoName()
				bProx = True
			End If
			'DMCommon.Debug.MsgBox("020121_2", sRegionTopoName, bProx)
			oRegionTopology = TopoManager.TopoCreator.GetOpenedTopology(sRegionTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			If (oRegionTopology Is Nothing) AndAlso (Not bProx) Then
				sRegionTopoName = TplnRegion.GetProxTopoName()
				oRegionTopology = TopoManager.TopoCreator.GetOpenedTopology(sRegionTopoName, Autodesk.Gis.Map.Topology.OpenMode.ForRead, False, False)
			End If
			'	DMCommon.Debug.MsgBox("020121_3a", sRegionTopoName, bProx, oRegionTopology Is Nothing)
			Return oRegionTopology
		End Function
		Private Shared Sub zzCalculateRegionParcel(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean, iRegion As Integer)
			If bFDO_Overlay Then
				If bApproved Then
					For Each oParcel As TplnParcel In mdicParcels.Values
						oParcel.CalculateGroup(DMAcadExt.enOverlayIndex.ApprFDO_Overlay, iRegion)
					Next
				End If
				If bProposed Then
					For Each oParcel As TplnParcel In mdicParcels.Values
						oParcel.CalculateGroup(DMAcadExt.enOverlayIndex.PropFDO_Overlay, iRegion)
					Next
				End If
			End If

		End Sub
		Private Shared Sub zzCalculateRegionLot(ByVal bMerge As Boolean, ByVal bUnion As Boolean, ByVal bFDO_Overlay As Boolean, ByVal bApproved As Boolean, ByVal bProposed As Boolean, iRegion As Integer)
			If bFDO_Overlay Then
				If bApproved Then
					TplnLot.ClearDataTable(DMAcadExt.enTopoPurpose.Approved)

					For Each oLot As TplnLot In mdicLotsAppr.Values
						oLot.CalculateGroup(DMAcadExt.enOverlayIndex.ApprFDO_Overlay, iRegion)
						oLot.AddDataToMainTable(bMerge, bUnion, bFDO_Overlay, True, False)

					Next
				End If
				If bProposed Then
					TplnLot.ClearDataTable(DMAcadExt.enTopoPurpose.Proposed)
					For Each oLot As TplnLot In mdicLotsAppr.Values
						oLot.CalculateGroup(DMAcadExt.enOverlayIndex.PropFDO_Overlay, iRegion)
						oLot.AddDataToMainTable(bMerge, bUnion, bFDO_Overlay, False, True)
					Next
				End If
			End If

		End Sub
		Private Shared Function zzGetTempLayerName(ByVal iTopoID As Integer) As String
			Dim iTopo As TPlanGraph.enTopoPurpose
			iTopo = CType(iTopoID, enTopoPurpose)
			Select Case iTopo
				Case enTopoPurpose.Parcel
					Return "Parcels"
				Case enTopoPurpose.Approved
					Return "LotsK"
				Case enTopoPurpose.Proposed
					Return "LotsM"
				Case Else
					Return String.Empty
			End Select
		End Function

#End Region

		Protected Overrides Sub Finalize()
			MyBase.Finalize()
		End Sub


		Public Sub New()

      End Sub
   End Class
End Namespace


