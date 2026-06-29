Option Explicit On
Option Strict On
Public Enum enGraphType
	Undefined
	Topology = 1
	ClosedPolygons = 2
	TopologyList = 3
	Union = 11
	TopoOverlay = 12
	MapLayerOverlay = 13
	AnalyticClip = 21
End Enum

Public Structure MapThemeData
	'Public Shared AAAA As MapThemeData
	Public Const MissingCentroidLayer As String = "TplnMissingCntr"
	Public Const WorkAreaBoundaryLayer As String = "WorkArea_Boundary"
	Private Const msDfltClosedPgonsMethod As String = "MAVAT"
	Private miMapThemeID As DMAcadExt.enMapTheme
	Private msMapThemeName As String

	Private msGraphTypeName As String
	Private msGraphTypeShortName As String

	Private msTopoName As String
	Private msLinkLayers As String
	Private msCentroidBlocks As String
	Private msCentroidLayers As String
	Private msNodeBlocks As String
	Private msNodeLayers As String

	Private msSPointsBlocks As String
	Private msSPointsLayers As String

	Private miCleanupType As Integer
	Private msClosedPgonsMethod As String
	Private msClosedPgonsLayers As String



	Private msLineTopoName As String
	Private msDissolveTopoName As String
	Private msDissolveAttribExpr As String
	Private msDissolveClosedPgonsLayers As String

	Private msLineLinkLayers As String
	Private msLineClosedPgonsLayers As String ' obsolete
	Private msMPgonLayers As String

	Private miLineCleanupType As Integer
	Private miGraphType As DMAcadExt.enGraphType
	Private miSourceMapThemeID As DMAcadExt.enMapTheme
	Private miOverlayMapThemeID As DMAcadExt.enMapTheme
	Private miOverlayMapThemeID_A As DMAcadExt.enMapTheme
	Private miTopoPurpose As DMAcadExt.enTopoPurpose
	Private miDesignElement As Integer
	Private miTopoPriority As Integer

	Private mbIsNotEmpty As Boolean



	Public Shared Function ToGraphType(iValue As Integer) As enGraphType
		If [Enum].IsDefined(GetType(enGraphType), iValue) Then
			Return CType(iValue, enGraphType)
		Else
			Return enGraphType.Undefined
		End If
	End Function
	Public Shared Function ToMapTheme(iValue As Integer) As enMapTheme
		If [Enum].IsDefined(GetType(enMapTheme), iValue) Then
			Return CType(iValue, enMapTheme)
		Else
			Return enMapTheme.Undefined
		End If
	End Function



	Public Sub New(oPrjMapThemesDataRow As System.Data.DataRow)
		miMapThemeID = MapThemeData.ToMapTheme(DirectCast(oPrjMapThemesDataRow.Item("MapThemeID"), Integer))
		msMapThemeName = DirectCast(oPrjMapThemesDataRow.Item("MapThemeName"), String)
		msTopoName = DMCommon.Functions.CStrN(oPrjMapThemesDataRow.Item("TopoName"))

		msGraphTypeName = DMCommon.Functions.CStrN(oPrjMapThemesDataRow.Item("GraphTypeName"))
		msGraphTypeShortName = DMCommon.Functions.CStrN(oPrjMapThemesDataRow.Item("GraphTypeShortName"))

		msLinkLayers = DMCommon.Functions.CStrN(oPrjMapThemesDataRow.Item("LinkLayers"))
		msCentroidBlocks = DMCommon.Functions.CStrN(oPrjMapThemesDataRow.Item("CentroidBlocks"))
		msCentroidLayers = DMCommon.Functions.CStrN(oPrjMapThemesDataRow.Item("CentroidLayers"))
		zzParseClosedPgonsDBValue(oPrjMapThemesDataRow.Item("ClosedPgonsLayers"))

		'	msClosedPgonsLayers = DMCommon.Functions.CStrN(oPrjMapThemesDataRow.Item("ClosedPgonsLayers"))
		msLineTopoName = DMCommon.Functions.CStrN(oPrjMapThemesDataRow.Item("LineTopoName"))

		msLineLinkLayers = DMCommon.Functions.CStrN(oPrjMapThemesDataRow.Item("LineLinkLayer"))

		msNodeBlocks = DMCommon.Functions.CStrN(oPrjMapThemesDataRow.Item("NodeBlocks"))
		msNodeLayers = DMCommon.Functions.CStrN(oPrjMapThemesDataRow.Item("NodeLayers"))
		'    DMCommon.Debug.MsgBox("09_439", True, msNodeBlocks, msNodeLayers)
		msMPgonLayers = DMCommon.Functions.CStrN(oPrjMapThemesDataRow.Item("MPgonLayers"))

		msDissolveTopoName = DMCommon.Functions.CStrN(oPrjMapThemesDataRow.Item("DissolveTopoName"))
		msDissolveAttribExpr = DMCommon.Functions.CStrN(oPrjMapThemesDataRow.Item("DissolveAttribExpr"))
		msDissolveClosedPgonsLayers = DMCommon.Functions.CStrN(oPrjMapThemesDataRow.Item("DissolveClosedPgonsLayers"))
		miCleanupType = DMCommon.Functions.CIntN(oPrjMapThemesDataRow.Item("CleanupType"))
		miLineCleanupType = DMCommon.Functions.CIntN(oPrjMapThemesDataRow.Item("LineCleanupType"))
		miGraphType = ToGraphType(DMCommon.Functions.CIntN(oPrjMapThemesDataRow.Item("GraphTypeID")))
		miSourceMapThemeID = MapThemeData.ToMapTheme(DMCommon.Functions.CIntN(oPrjMapThemesDataRow.Item("SourceMapThemeID")))
		miOverlayMapThemeID = MapThemeData.ToMapTheme(DMCommon.Functions.CIntN(oPrjMapThemesDataRow.Item("OverlayMapThemeID")))
		miOverlayMapThemeID_A = MapThemeData.ToMapTheme(DMCommon.Functions.CIntN(oPrjMapThemesDataRow.Item("OverlayMapThemeID_A")))

		miTopoPurpose = TplnEnum.ToTopoPurpose(DMCommon.Functions.CIntN(oPrjMapThemesDataRow.Item("TabaPurpose")))

		miDesignElement = DMCommon.Functions.CIntN(oPrjMapThemesDataRow.Item("DesignElementType"))
		miTopoPriority = DMCommon.Functions.CIntN(oPrjMapThemesDataRow.Item("TopoPriority"))

		msSPointsBlocks = DMCommon.Functions.CStrN(oPrjMapThemesDataRow.Item("SPointsBlocks"))
		msSPointsLayers = DMCommon.Functions.CStrN(oPrjMapThemesDataRow.Item("SPointsLayers"))



		mbIsNotEmpty = True
	End Sub
	Public ReadOnly Property IsNotEmpty As Boolean
		Get
			Return mbIsNotEmpty
		End Get
	End Property
	Public ReadOnly Property HasNodes As Boolean
		Get
			Return Not String.IsNullOrEmpty(msNodeLayers)
		End Get
	End Property


	Public Property MapThemeID As DMAcadExt.enMapTheme
		Get
			Return miMapThemeID
		End Get
		Set(iValue As DMAcadExt.enMapTheme)
			miMapThemeID = iValue
		End Set
	End Property

	Public Property MapThemeName As String
		Get
			Return msMapThemeName
		End Get
		Set(sValue As String)
			msMapThemeName = sValue
		End Set
	End Property

	Public Property GraphTypeName As String
		Get
			Return msGraphTypeName
		End Get
		Set(sValue As String)
			msGraphTypeName = sValue
		End Set
	End Property

	Public Property GraphTypeShortName As String
		Get
			Return msGraphTypeShortName
		End Get
		Set(sValue As String)
			msGraphTypeShortName = sValue
		End Set
	End Property
	Public Property TopoName As String

		Get
			If miGraphType = enGraphType.Topology Then
				Return msTopoName
			ElseIf miGraphType = enGraphType.ClosedPolygons Then
				Return msTopoName
			ElseIf miGraphType = enGraphType.TopologyList Then
				Return msTopoName
			ElseIf miGraphType = enGraphType.TopoOverlay Then
				Return msTopoName
			ElseIf miGraphType = enGraphType.MapLayerOverlay Then
				Return msTopoName
			Else
				DMCommon.Debug.MsgBox("GraphType #1270", miGraphType.ToString() & vbCrLf & mbIsNotEmpty.ToString() & vbCrLf & Me.MapThemeID.ToString())
				Return Nothing
			End If

		End Get
		Set(sValue As String)
			msTopoName = sValue
		End Set
	End Property
	Public ReadOnly Property ShapeConnection As String
		Get
			Const sPrefix As String = "shp_"
			Return sPrefix & msLineTopoName
		End Get

	End Property
	Public ReadOnly Property MapLayer As String
		Get
			Return msLineTopoName
		End Get

	End Property
	Public ReadOnly Property IntermediateLayer As String
		Get
			Return msTopoName
		End Get

	End Property
	Public Property LinkLayers As String
		Get
			Return msLinkLayers
		End Get
		Set(sValue As String)
			msLinkLayers = sValue
		End Set
	End Property
	Public Sub AddLinkLayers(sAddLinkLayers As String)
		If Not String.IsNullOrEmpty(sAddLinkLayers) Then
			If Not String.IsNullOrEmpty(msLinkLayers) Then
				msLinkLayers &= ","
			End If
			msLinkLayers &= sAddLinkLayers
		End If

	End Sub
	Public Sub AddAllUpLayers(ByRef hsLayers As HashSet(Of String))
		zzAddUpLayers(hsLayers, msLinkLayers)
		zzAddUpLayers(hsLayers, msLineLinkLayers)
		zzAddUpLayers(hsLayers, msLinkLayers)
		zzAddUpLayers(hsLayers, msCentroidLayers)

		zzAddUpLayers(hsLayers, msNodeLayers)

		zzAddUpLayers(hsLayers, msClosedPgonsLayers)

		zzAddUpLayers(hsLayers, msSPointsLayers)
		zzAddUpLayers(hsLayers, msDissolveClosedPgonsLayers)



	End Sub
	Private Sub zzAddUpLayers(ByRef hsLayers As HashSet(Of String), sLayers As String)
		If Not String.IsNullOrEmpty(sLayers) Then
			Dim saLayers() As String = Split(sLayers, ",")
			For iIndex As Integer = 0 To saLayers.GetUpperBound(0)
				hsLayers.Add(UCase(saLayers(iIndex)))
			Next
		End If
	End Sub

	Public ReadOnly Property LinkLayer As String
		Get
			If String.IsNullOrEmpty(msLinkLayers) Then
				Return msLinkLayers
			Else
				Dim tList As DMCommon.dmList = New DMCommon.dmList(msLinkLayers)
				Return tList.First
			End If

		End Get

	End Property
	Public ReadOnly Property ClosedPgonsMethod As String
		Get
			If String.IsNullOrEmpty(msClosedPgonsMethod) Then
				Return msDfltClosedPgonsMethod
			Else
				Return msClosedPgonsMethod
			End If

		End Get

	End Property
	Public Property ClosedPgonsLayersAAAAA As String
		Get
			Return msClosedPgonsLayers
		End Get
		Set(sValue As String)
			msClosedPgonsLayers = sValue
		End Set
	End Property
	Public ReadOnly Property ClosedPgonsLayers As String
		Get
			Return msClosedPgonsLayers
		End Get

	End Property

	Public ReadOnly Property ClosedPgonsLayer As String
		Get
			If String.IsNullOrEmpty(msClosedPgonsLayers) Then
				Return msClosedPgonsLayers
			Else
				Dim tList As DMCommon.dmList = New DMCommon.dmList(msClosedPgonsLayers)
				Return tList.First
			End If


		End Get

	End Property
	Public Property MPgonLayers As String
		Get
			Return msMPgonLayers
		End Get
		Set(sValue As String)
			msMPgonLayers = sValue
		End Set
	End Property


	Public Property CentroidBlocks As String
		Get
			Return msCentroidBlocks
		End Get
		Set(sValue As String)
			msCentroidBlocks = sValue
		End Set
	End Property
	Public ReadOnly Property CentroidBlock As String
		Get
			If String.IsNullOrEmpty(msCentroidBlocks) Then
				Return msCentroidBlocks
			Else
				Dim tList As DMCommon.dmList = New DMCommon.dmList(msCentroidBlocks)
				Return tList.First
			End If

		End Get

	End Property
	Public Property CentroidLayers As String
		Get
			Return msCentroidLayers
		End Get
		Set(sValue As String)
			msCentroidLayers = sValue
		End Set
	End Property
	Public ReadOnly Property CentroidLayer As String
		Get
			If String.IsNullOrEmpty(msCentroidLayers) Then
				Return msCentroidLayers
			Else
				Dim tList As DMCommon.dmList = New DMCommon.dmList(msCentroidLayers)
				Return tList.First
			End If

		End Get

	End Property


	Public Property NodeBlocks As String
		Get
			Return msNodeBlocks
		End Get
		Set(sValue As String)
			msNodeBlocks = sValue
		End Set
	End Property
	Public ReadOnly Property NodeBlock As String
		Get
			If String.IsNullOrEmpty(msNodeBlocks) Then
				Return msNodeBlocks
			Else
				Dim tList As DMCommon.dmList = New DMCommon.dmList(msNodeBlocks)
				Return tList.First
			End If
		End Get

	End Property
	Public Property NodeLayers As String
		Get
			Return msNodeLayers
		End Get
		Set(sValue As String)
			msNodeLayers = sValue
		End Set
	End Property
	Public ReadOnly Property NodeLayer As String
		Get
			If String.IsNullOrEmpty(msNodeLayers) Then
				Return msNodeLayers
			Else
				Dim tList As DMCommon.dmList = New DMCommon.dmList(msNodeLayers)
				Return tList.First
			End If
		End Get

	End Property

	Public Property SPointsBlocks As String
		Get
			Return msSPointsBlocks

		End Get
		Set(sValue As String)
			msSPointsBlocks = sValue
		End Set
	End Property
	Public Property SPointsLayers As String
		Get
			Return msSPointsLayers
		End Get
		Set(sValue As String)
			msSPointsLayers = sValue
		End Set
	End Property
	Public ReadOnly Property LineTopoExists As Boolean
		Get
			Return Not String.IsNullOrEmpty(msLineTopoName)
		End Get
	End Property
	Public Property LineTopoName As String
		Get
			If miGraphType = enGraphType.Topology Then
				Return msLineTopoName
			ElseIf miGraphType = enGraphType.ClosedPolygons Then
				Return msLineTopoName
			Else
				Return msLineTopoName
			End If

		End Get
		Set(sValue As String)
			msLineTopoName = sValue
		End Set
	End Property

	Public Property DissolveTopoName As String
		Get
			Return msDissolveTopoName
		End Get
		Set(sValue As String)
			msDissolveTopoName = sValue
		End Set
	End Property
	Public Property DissolveAttribExpr As String
		Get
			Return msDissolveAttribExpr
		End Get
		Set(sValue As String)
			msDissolveAttribExpr = sValue
		End Set
	End Property


	Public Property LineLinkLayers As String
		Get
			Return msLineLinkLayers
		End Get
		Set(sValue As String)
			msLineLinkLayers = sValue
		End Set
	End Property
	Public ReadOnly Property LineLinkLayer As String
		Get
			If String.IsNullOrEmpty(msLineLinkLayers) Then
				Return msLineLinkLayers
			Else
				Dim tList As DMCommon.dmList = New DMCommon.dmList(msLineLinkLayers)
				Return tList.First
			End If

		End Get

	End Property

	Public Property DissolveClosedPgonsLayers As String
		Get
			Return msDissolveClosedPgonsLayers
		End Get
		Set(sValue As String)
			msDissolveClosedPgonsLayers = sValue
		End Set
	End Property
	Public Property LineClosedPgonsLayers As String
		Get
			Return msDissolveClosedPgonsLayers
		End Get
		Set(sValue As String)
			msDissolveClosedPgonsLayers = sValue
		End Set
	End Property



	Public ReadOnly Property ExportCondition As String
		Get
			Select Case miGraphType
				Case enGraphType.Topology, enGraphType.TopoOverlay
					Return msLineTopoName
				Case DMAcadExt.enGraphType.ClosedPolygons
					Return msMPgonLayers
				Case Else
					Return Nothing
			End Select

		End Get
	End Property
	Public ReadOnly Property GraphType As DMAcadExt.enGraphType
		Get
			Return miGraphType
		End Get
	End Property
	Public Property SourceMapThemeID As DMAcadExt.enMapTheme
		Get
			Return miSourceMapThemeID
		End Get
		Set(iValue As DMAcadExt.enMapTheme)
			miSourceMapThemeID = iValue
		End Set
	End Property

	Public Property OverlayMapThemeID As DMAcadExt.enMapTheme
		Get
			Return miOverlayMapThemeID
		End Get
		Set(iValue As DMAcadExt.enMapTheme)
			miOverlayMapThemeID = iValue
		End Set
	End Property
	Public Property OverlayMapThemeID_A As DMAcadExt.enMapTheme
		Get
			Return miOverlayMapThemeID_A
		End Get
		Set(iValue As DMAcadExt.enMapTheme)
			miOverlayMapThemeID_A = iValue
		End Set
	End Property

	Public Property CleanupType As Integer
		Get
			Return miCleanupType
		End Get
		Set(iValue As Integer)
			miCleanupType = iValue
		End Set
	End Property
	Public Property TopoPurpose As DMAcadExt.enTopoPurpose
		Get
			Return miTopoPurpose
		End Get
		Set(iValue As DMAcadExt.enTopoPurpose)
			miTopoPurpose = iValue
		End Set
	End Property
	Public Property DesignElement As Integer
		Get
			Return miDesignElement
		End Get
		Set(iValue As Integer)
			miDesignElement = iValue
		End Set
	End Property
	Public Property TopoPriority As Integer
		Get
			Return miTopoPriority
		End Get
		Set(iValue As Integer)
			miTopoPriority = iValue
		End Set
	End Property
	Private Sub zzParseClosedPgonsDBValue(oValue As System.Object)
		If Not IsDBNull(oValue) Then
			Dim saValue() As String = Split(DMCommon.Functions.CStrN(oValue), ":")
			'DMCommon.Debug.MsgBox("02_118c", saValue)

			If saValue.GetUpperBound(0) = 1 Then
				msClosedPgonsMethod = saValue(0)
				msClosedPgonsLayers = saValue(1)
			Else
				msClosedPgonsLayers = saValue(0)
			End If
			'	DMCommon.Debug.MsgBox("02_118d", saValue.GetUpperBound(0), msClosedPgonsMethod, msClosedPgonsLayers)
		End If


	End Sub
End Structure
