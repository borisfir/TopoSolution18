Option Explicit On
Option Strict On
Imports System.Data
Public Enum enTopoLayersOption
	[Default] = 0
	StraightInclude = 1
End Enum
Public Enum enTopoCreationTypes
	[Default] = 0
	ByLayers = 1
	ByUnionLayers = 2
	OverlayUnion = 3
	Dissolve = 4
End Enum
Public Class TopoDef
	Private mtTopoDefID As TopoDefID
	Private msName As String
	Private msDescription As String = String.Empty

	Private msIncludeLayers As String = String.Empty
	Private msLinkLayers As String = String.Empty
	Private msDuplicateLayer As String = String.Empty
	Private miLayersOption As enTopoLayersOption
	Private miLinkColorIndex As Integer

	Private msaCentroidBlocks As String()
	Private msCentroidLayers As String = String.Empty
	Private mdCentroidScale As Double
	Private msCentroidBlocks As String = String.Empty
	Private msMissingCentroidLayer As String = String.Empty
	Private msClosedPgonsLayer As String = String.Empty


	Private msODTableName As String = String.Empty
	Private msAttribExpession As String = String.Empty
	Private miPriority As Integer
	Private miSourceID As Integer
	Private miOverlayID As Integer

	Private mshMarkColor As Short

	Private mbCreateCentroidDflt As Boolean
	Private mbCreateCentroid As Boolean

	Private moSourceTopoDef As TopoDef
	Private moOverlayTopoDef As TopoDef
	Private moAdditionalTopoDef As TopoDef

	Private mdicDoubleLayers As Dictionary(Of String, String)
	Private mdicDoubleLayersR As Dictionary(Of String, String)


	Private mbLinkLayerEmpty As Boolean = False
	Private Shared miFormat As Integer = 1
	Private miCreationType As enTopoCreationTypes
	Public Const ValueDel As String = ","

	Public Sub NewAAA(ByVal oDataReader As Common.DbDataReader)
		zzNew(oDataReader)
	End Sub
	Public Sub New(ByVal sTopoName As String)
		Dim sComText As String = "SELECT * FROM TopoDefs WHERE Name='" & sTopoName & "'"
		Dim oDataReader As Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
		If oDataReader IsNot Nothing Then
			zzNew(oDataReader)
		Else
			System.Windows.Forms.MessageBox.Show("Topology '" & sTopoName & "' was not found", "Topodef - New ")
		End If
	End Sub
	Public Sub New(ByVal iAppID As DMAcadExt.enApplications, ByVal iTopoDefID As Integer)
		Dim sComText As String = "SELECT * FROM TopoDefs WHERE (AppID=" & CStr(iAppID) & ") AND (FormatID=" & CStr(miFormat) & ") AND (ID=" & CStr(iTopoDefID) & ")"

		Try
			Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
			If oDataReader IsNot Nothing Then
				zzNew(oDataReader)
			Else
				System.Windows.Forms.MessageBox.Show("Topology #" & CStr(iTopoDefID) & " was not found", "Topodef - New_01")
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "TopoDef - New")
		End Try


	End Sub
	Public Sub New(ByVal iAppID As DMAcadExt.enApplications, ByVal iTopoDefID As TopoDefID)
		Dim sComText As String = "SELECT * FROM TopoDefs WHERE (AppID=" & CStr(iAppID) & ") AND (FormatID=" & CStr(miFormat) & ") AND (ID=" & CStr(iTopoDefID.ID) & ")"

		Try
			Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
			If oDataReader IsNot Nothing Then
				zzNew(oDataReader)
			Else
				System.Windows.Forms.MessageBox.Show("Topology #" & CStr(iTopoDefID.ID) & " was not found", "Topodef - New_01")
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "TopoDef - New")
		End Try


	End Sub

	Public Shared Property Format() As Integer
		Get
			Return miFormat
		End Get
		Set(ByVal iValue As Integer)
			miFormat = iValue
		End Set
	End Property
	Public Property ID() As TopoDefID
		Get
			Return mtTopoDefID
		End Get
		Set(ByVal stValue As TopoDefID)
			mtTopoDefID = stValue
		End Set
	End Property
	Public Property Name() As String
		Get
			Return msName
		End Get
		Set(ByVal sValue As String)
			msName = sValue
		End Set
	End Property
	Public Property Description() As String
		Get
			Return msDescription
		End Get
		Set(ByVal sValue As String)
			msDescription = sValue
		End Set
	End Property
	Public ReadOnly Property BaseOrSourceTopoDef() As TopoDef
		Get
			If mtTopoDefID.TopoIsBase Then
				Return Me
			Else
				Return moSourceTopoDef
			End If
		End Get
	End Property
	Public Property SourceTopoDef() As TopoDef
		Get
			Return moSourceTopoDef
		End Get
		Set(ByVal oValue As TopoDef)
			moSourceTopoDef = oValue
		End Set
	End Property
	Public Property OverlayTopoDef() As TopoDef
		Get
			Return moOverlayTopoDef
		End Get
		Set(ByVal oValue As TopoDef)
			moOverlayTopoDef = oValue
		End Set
	End Property

	Public Property AdditionalTopoDef() As TopoDef
		Get
			Return moAdditionalTopoDef
		End Get
		Set(ByVal oValue As TopoDef)
			moAdditionalTopoDef = oValue
		End Set
	End Property
	Public ReadOnly Property DuplicateLayer() As String
		Get
			Return msDuplicateLayer
		End Get

	End Property
	Public ReadOnly Property LinkLayers() As String
		Get
			Return msLinkLayers
		End Get
	End Property
	Public ReadOnly Property LinkAndDuplicateLayers() As String
		Get
			If msDuplicateLayer.Length = 0 Then
				Return msLinkLayers
			ElseIf msLinkLayers.Length = 0 Then
				Return msDuplicateLayer
			Else
				Return msLinkLayers & ValueDel & msDuplicateLayer
			End If
		End Get
	End Property
	Public ReadOnly Property FDOLayerName() As String
		Get
		 
			Return msName '"topo" & 

		End Get
	End Property
	Public ReadOnly Property FDOConnectionName() As String
		Get
			 
			Return "shp" & msName

		End Get
	End Property
	Public ReadOnly Property CreationType() As enTopoCreationTypes
		Get
			Return miCreationType
		End Get
	End Property

	Public ReadOnly Property AttribExpession() As String
		Get
			Return msAttribexpession
		End Get
	End Property
	Public ReadOnly Property MissingCentroidLayer() As String
		Get
			Return msMissingCentroidLayer
		End Get
	End Property
	Public ReadOnly Property ClosedPgonsLayer() As String
		Get
			Return msClosedPgonsLayer
		End Get
	End Property

	Public ReadOnly Property ODTableName() As String
		Get
			Return msODTableName
		End Get
	End Property
	Public Function IncludeLayersContain(ByVal sLayersDel As String) As Boolean
		Return AddDelim(msIncludeLayers).Contains(sLayersDel)
	End Function
	Public Property IncludeLayers() As String
		Get
			Return msIncludeLayers
		End Get
		Set(ByVal sValue As String)
			msIncludeLayers = sValue
		End Set
	End Property
	Public ReadOnly Property TopoLayers() As String
		Get
			If msLinkLayers.Length = 0 Then
				Return msIncludeLayers
			Else
				Return msLinkLayers
			End If
		End Get
	End Property
	Public ReadOnly Property TopoStraightLayers() As String
		Get
			If miLayersOption = enTopoLayersOption.StraightInclude Then
				Return msIncludeLayers
			Else
				Return String.Empty
			End If
		End Get
	End Property
	Public ReadOnly Property CentroidBlockExists() As Boolean
		Get
			Return (msCentroidBlocks.Length <> 0)
		End Get
	End Property
	Public ReadOnly Property CentroidBlocksText() As String
		Get
			'zzDispArray(msaCentroidBlocks, "!!!Property")
			If CentroidBlockExists Then
				Return "Block - " & msaCentroidBlocks(0)
			Else
				Return String.Empty
			End If

		End Get

	End Property
	Public Property CentroidBlocks() As String()
		Get
			'zzDispArray(msaCentroidBlocks, "!!!Property")
			Return msaCentroidBlocks
		End Get
		Set(ByVal saValue As String())
			msaCentroidBlocks = saValue
		End Set
	End Property
	Public ReadOnly Property CentroidBlock() As String
		Get
			If msCentroidBlocks.Length = 0 Then
				Return String.Empty
			Else
				Return msaCentroidBlocks(0)
			End If

		End Get

	End Property
	Public ReadOnly Property CentroidLayers() As String
		Get
			Return msCentroidLayers
		End Get
	End Property
	Public ReadOnly Property CentroidLayer() As String
		Get
			If msCentroidLayers.Length = 0 Then
				Return String.Empty
			Else
				Dim saLayers() As String = Split(msCentroidLayers, ",")
				Return saLayers(0)
			End If
		End Get
	End Property
	Public ReadOnly Property CentroidLayersText() As String
		Get
			If msCentroidLayers.Length <> 0 Then
				Return "Layer(s) - " & msCentroidLayers
			Else
				Return String.Empty
			End If

		End Get
	End Property
	Public ReadOnly Property CreateTopologyLayer() As String
		Get
			If mbCreateCentroid Then
				Return CentroidLayer
			Else
				Return msMissingCentroidLayer
			End If
		End Get
	End Property
	Public ReadOnly Property LayersOption() As enTopoLayersOption
		Get
			Return miLayersOption
		End Get

	End Property
	Public ReadOnly Property CentroidScale() As Double
		Get
			Return mdCentroidScale
		End Get

	End Property
	Public Property LinkColorIndex() As Integer
		Get
			Return miLinkColorIndex
		End Get
		Set(ByVal iValue As Integer)
			miLinkColorIndex = iValue
		End Set
	End Property

	Public Property SourceID() As Integer
		Get
			Return miSourceID
		End Get
		Set(ByVal iValue As Integer)
			miSourceID = iValue
		End Set
	End Property
	Public Property OverlayID() As Integer
		Get
			Return miOverlayID
		End Get
		Set(ByVal iValue As Integer)
			miOverlayID = iValue
		End Set
	End Property
	Public Property Priority() As Integer
		Get
			Return miPriority
		End Get
		Set(ByVal iValue As Integer)
			miPriority = iValue
		End Set
	End Property

	Public ReadOnly Property MarkColor() As Short
		Get
			Return mshMarkColor
		End Get

	End Property
	Public ReadOnly Property LinkText() As String
		Get
			Dim sRes As String
			If LinkLayersExists Then
				sRes = "(Layer(s) - " & msLinkLayers & ")"
				If miLayersOption = enTopoLayersOption.StraightInclude Then
					sRes &= vbCrLf & "(Layer(s)/without arcs/ - " & msIncludeLayers & ")"
				End If
				Return sRes
			ElseIf msIncludeLayers.Length <> 0 Then
				Return "(Layer(s) - " & msIncludeLayers & ")"
			Else
				Return String.Empty
			End If
		End Get
	End Property
	Public ReadOnly Property CentroidText() As String
		Get
			Dim sRes As String
			If CentroidBlockExists OrElse msCentroidLayers.Length <> 0 Then
				sRes = "(" & Me.CentroidBlocksText
				If CentroidBlockExists AndAlso msCentroidLayers.Length <> 0 Then
					sRes &= "; "
				End If
				sRes &= Me.CentroidLayersText & ")"
			Else
				sRes = String.Empty
			End If
			Return sRes
		End Get
	End Property
	Public ReadOnly Property CleanupEnabled() As Boolean
		Get
			Return Not mtTopoDefID.TopoIsUnion
		End Get
	End Property
	Public ReadOnly Property PrepareEnabled() As Boolean
		Get
			Return mtTopoDefID.TopoIsMerge OrElse mtTopoDefID.TopoIsAdditional
		End Get
	End Property
	Public ReadOnly Property LinkLayersExists() As Boolean
		Get
			Return (msLinkLayers.Length <> 0)
		End Get

	End Property
	Public Property LinkLayerEmpty() As Boolean
		Get
			Return mbLinkLayerEmpty
		End Get
		Set(ByVal bValue As Boolean)
			mbLinkLayerEmpty = bValue
		End Set
	End Property
	

	Public Property CreateCentroid() As Boolean
		Get
			Return mbCreateCentroid
		End Get
		Set(ByVal bValue As Boolean)
			If bValue Then
				mbCreateCentroid = True
			Else
				mbCreateCentroid = mbCreateCentroidDflt
			End If

		End Set
	End Property
	Public Shared Function AddDelim(ByVal sValue As String) As String
		Return ValueDel & sValue & ValueDel	'ValueDel & UCase(sValue) & ValueDel
	End Function
	Public Shared Function IsOneLayer(ByVal sValue As String) As Boolean
		Return Not sValue.Contains(ValueDel)
	End Function
	Public Shared Function ValueSplit(ByVal sValue As String) As String()

		Return (Strings.Split(sValue, ValueDel))
	End Function
	Public Function GetLocalFileName(sExtension As String, Optional iAdd As Integer = 0) As String
		'	System.Windows.Forms.MessageBox.Show(msName, "06_150")
		Return DMCommon.Functions.GetLocalFileNameInDir(msName, sExtension, iAdd)

	End Function
	Public Function GetDoubleLayer(ByVal sLayer As String, ByVal bReturn As Boolean) As String
		Dim sResLayer As String = Nothing
		Dim bRes As Boolean

		If bReturn Then
			bRes = mdicDoubleLayers.TryGetValue(sLayer, sResLayer)
		Else
			bRes = mdicDoubleLayersR.TryGetValue(sLayer, sResLayer)
		End If
		If bRes Then
			Return sResLayer
		Else
			Return Nothing
		End If

	End Function

	Public Function GetAllLayers() As String()
		'	Const sDelim As String = ","
		Dim sOutput As String = Me.TopoLayers

		If msCentroidLayers.Length <> 0 Then
			sOutput &= ValueDel & msCentroidLayers
		End If
		If msMissingCentroidLayer.Length <> 0 Then
			sOutput &= ValueDel & msMissingCentroidLayer
		End If
		Return ValueSplit(sOutput)
	End Function
	Public Function GetTopoDefByLayer(ByVal sLayersDel As String, ByRef oTopoDef As TopoDef) As Boolean
		If mtTopoDefID.TopoIsBase Then
			If Me.IncludeLayersContain(sLayersDel) Then
				oTopoDef = Me
				Return False
			Else
				oTopoDef = Nothing
				Return False
			End If
		Else
			If moSourceTopoDef.IncludeLayersContain(sLayersDel) Then
				oTopoDef = moSourceTopoDef
				Return False
			ElseIf moOverlayTopoDef.IncludeLayersContain(sLayersDel) Then
				oTopoDef = moOverlayTopoDef
				Return True
			Else
				oTopoDef = Nothing
				Return False
			End If
		End If
	End Function
	Private Sub zzNew(ByVal oDataReader As Common.DbDataReader)
		Try
			If oDataReader.Read Then

				For iIndex As Integer = 0 To oDataReader.FieldCount - 1

					Select Case oDataReader.GetName(iIndex)
						Case "ID"
							mtTopoDefID.ID = oDataReader.GetInt32(iIndex)
						Case "Name"
							msName = oDataReader.GetString(iIndex)
						Case "Description"
							If Not oDataReader.IsDBNull(iIndex) Then
								msDescription = oDataReader.GetString(iIndex)
							End If
						Case "Type"
							Dim iValue As Integer = oDataReader.GetInt32(iIndex)
							If [Enum].IsDefined(GetType(enTopoCreationTypes), iValue) Then
								miCreationType = CType(iValue, enTopoCreationTypes)
							Else
								miCreationType = enTopoCreationTypes.Default
							End If
						Case "IncludeLayers"
							If Not oDataReader.IsDBNull(iIndex) Then
								msIncludeLayers = oDataReader.GetString(iIndex)
								' msaIncludeLayers = Split(oDataReader.GetString(iIndex), ValueDel)
							End If
						Case "LinkLayers"
							If Not oDataReader.IsDBNull(iIndex) Then
								msLinkLayers = oDataReader.GetString(iIndex)
							End If
						Case "LinkColor"
							If oDataReader.IsDBNull(iIndex) Then
								miLinkColorIndex = 0
							Else
								miLinkColorIndex = oDataReader.GetInt32(iIndex)
							End If
						Case "DuplicateLayer"
							If Not oDataReader.IsDBNull(iIndex) Then
								msDuplicateLayer = oDataReader.GetString(iIndex)
							End If
						Case "LayersOption"
							If Not oDataReader.IsDBNull(iIndex) Then
								miLayersOption = CType(oDataReader.GetInt32(iIndex), enTopoLayersOption)
							End If
						Case "CentroidBlocks"
							If Not oDataReader.IsDBNull(iIndex) Then
								msCentroidBlocks = oDataReader.GetString(iIndex)
								msaCentroidBlocks = ValueSplit(msCentroidBlocks)
								'  zzDispArray(msaCentroidBlocks, "!!!TopoDef-zzNew-2")
							End If
						Case "SourceID"
							If Not oDataReader.IsDBNull(iIndex) Then
								miSourceID = oDataReader.GetInt32(iIndex)
							End If
						Case "OverlayID"
							If Not oDataReader.IsDBNull(iIndex) Then
								miOverlayID = oDataReader.GetInt32(iIndex)
							End If
						Case "CentroidLayers"
							If Not oDataReader.IsDBNull(iIndex) Then
								msCentroidLayers = oDataReader.GetString(iIndex)
							End If
						Case "CreateCentroid"
							mbCreateCentroidDflt = oDataReader.GetBoolean(iIndex)
							mbCreateCentroid = mbCreateCentroidDflt
						Case "CentroidScale"
							If Not oDataReader.IsDBNull(iIndex) Then
								mdCentroidScale = oDataReader.GetDouble(iIndex)
							End If
						Case "MissingCentroidLayer"
							If Not oDataReader.IsDBNull(iIndex) Then
								msMissingCentroidLayer = oDataReader.GetString(iIndex)
							End If
						Case "ClosedPgonsLayer"
							If Not oDataReader.IsDBNull(iIndex) Then
								msClosedPgonsLayer = oDataReader.GetString(iIndex)
							End If

						Case "ODTableName"
							If Not oDataReader.IsDBNull(iIndex) Then
								msODTableName = oDataReader.GetString(iIndex)
							End If
						Case "Priority"
							miPriority = oDataReader.GetInt32(iIndex)
						Case "MarkColor"
							mshMarkColor = oDataReader.GetInt16(iIndex)
						Case "AttribExpression"
							If Not oDataReader.IsDBNull(iIndex) Then
								msAttribExpession = oDataReader.GetString(iIndex)
							End If

					End Select
				Next
				zzSetDoubleLayers()
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TopoDef - zzNew")
		End Try
		oDataReader.Close()
	End Sub
	Private Sub zzSetDoubleLayers()
		mdicDoubleLayers = New Dictionary(Of String, String)
		mdicDoubleLayersR = New Dictionary(Of String, String)

		If msLinkLayers IsNot Nothing AndAlso msIncludeLayers IsNot Nothing Then
			Dim msaIncludeLayers() As String = ValueSplit(msIncludeLayers)
			Dim msaLinkLayers() As String = ValueSplit(msLinkLayers)
			Dim iUB As Integer = msaIncludeLayers.GetUpperBound(0)

			If iUB = msaLinkLayers.GetUpperBound(0) Then
				For iIndex As Integer = 0 To iUB
					mdicDoubleLayers.Add(msaLinkLayers(iIndex), msaIncludeLayers(iIndex))
					mdicDoubleLayersR.Add(msaIncludeLayers(iIndex), msaLinkLayers(iIndex))
				Next
			End If
		End If
	End Sub
	Private Sub zzSetTopoNames()
		'   Dim iTopoPurpose As TPlanGraph.enTopoPurpose
		If mtTopoDefID.TopoIsBase Then
			Select Case mtTopoDefID.BaseID
				Case enTopoPurpose.Parcel

			End Select
		End If

	End Sub
	Private Sub zzDispArrayAA(ByVal saVal() As String, ByVal sTitle As String)
		Dim sOut As String = String.Empty
		If saVal IsNot Nothing Then
			For iIndex As Integer = 0 To saVal.GetUpperBound(0)
				sOut += ":" & saVal(iIndex)
			Next
		Else
			sOut = "Nothing"
		End If

		System.Windows.Forms.MessageBox.Show(sOut, sTitle)
	End Sub
End Class

