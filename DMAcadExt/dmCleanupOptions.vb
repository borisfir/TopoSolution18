Option Explicit On
Option Strict On
Public Enum enClenupMethods
	Modify
	RetainCreateNew
	DeleteCreateNew
End Enum
Public Enum enCleanupAction
	EraseShortObjects = 1
	BreakCrossingObjects = 2
	ExtendUndershoots = 4
	DeleteDuplicates = 8
	SnapClusteredNodes = 16
	DissolvePseudoNodes = 32
	EraseDanglingObjects = 64
	SimplifyObjects = 128
	ZeroLengthObjects = 256
	ApparentIntersection = 512
   WeedPolylines = 1024
	ExplodePLines = 524288
	ConvertArcs = 524304
	ShortLines = 524320
	RoundingPoints = 1048576
	MergingPoints = 2097152
	PointsNearLine = 4194304
   RemoveDuplicates = 8388608
   NetPointsLines = 16777216
   RoundingLines = 33554432
   RoundingAll = 67108864
   First_dmAction = ExplodePLines
End Enum
Public Enum enMerging
	None
	Merging
   Rounding
   RoundingSPoints
   Temporary
End Enum
Public Structure dmCleanupOptions
	Public Const dmActionsUB As Integer = 1
	Private Const mdToleranceMin As Double = 0.00009 '0.00011 '
	Private mdStraightenTolerance As Double
	Private mdStraightenMinRadius As Double
	Private mdStraightenMaxArea As Double


	Private miStraightenRowIndex As Integer
	Private mbStraighten As Boolean

	'Private mbExplode As Boolean
	Private mdRoundingTolerance As Double
	Private miRoundingRowIndex As Integer
	Private miRounding As enMerging
	Private mbRoundingSPoints As Boolean
	Private mbRoundingLines As Boolean
	Private mbPointsNearLines As Boolean

	Private mbNetPointsLines As Boolean

	Private mdPointLineTolerance As Double
	Private miPointLineRowIndex As Integer
	Private mbPointLine As Boolean

	Private mdRemoveDuplicatesTolerance As Double
	Private miRemoveDuplicatesRowIndex As Integer
	Private mbRemoveDuplicates As Boolean

	Private miRoundAllDecimals As Integer
	Private mbRoundAll As Boolean
	Private miRoundAllRowIndex As Integer
	Private mbExplodePLines As Boolean
	Private miExplodePLinesRowIndex As Integer
	Private mbConvertArcs As Boolean
	Private miConvertArcsRowIndex As Integer


	Private miPointMergingRowIndex As Integer
	Private mbPointMerging As Boolean

	Private mdShortLinesTolerance As Double
	Private miShortLinesRowIndex As Integer
	Private mbShortLines As Boolean

	Private msBlockNames As String
	Private msSourceLayers As String
	Private msDestLayers As String
	Private moTopoDef As TopoDef
	Private mtMapThemeData As MapThemeData
	Private miClenupMethods As enClenupMethods
	Private mbHasAction As Boolean
	Public Property ClenupMethods() As enClenupMethods
		Get
			Return miClenupMethods
		End Get
		Set(ByVal iValue As enClenupMethods)
			miClenupMethods = iValue
		End Set
	End Property

	Public ReadOnly Property PointLine() As Boolean
		Get

			Return mbPointLine AndAlso mdPointLineTolerance > mdToleranceMin
		End Get
	End Property

	Public Property PointLineTolerance() As Double
		Get
			Return mdPointLineTolerance
		End Get
		Set(ByVal bValue As Double)
			mdPointLineTolerance = bValue
			mbPointLine = True
		End Set
	End Property
	Public Property ShortLinesTolerance() As Double
		Get
			Return mdShortLinesTolerance
		End Get
		Set(ByVal bValue As Double)
			mdShortLinesTolerance = bValue
			mbShortLines = True
		End Set
	End Property
	Public ReadOnly Property PointLineDigit() As Integer
		Get
			Dim iDigit As Integer = Convert.ToInt32(-Math.Log10(mdPointLineTolerance))
			If iDigit >= 0 AndAlso iDigit <= 12 Then
				Return iDigit
			Else
				Return 0
			End If


		End Get

	End Property

	Public ReadOnly Property PointLineRowindex() As Integer
		Get
			Return miPointLineRowIndex
		End Get
	End Property


	Public ReadOnly Property Straighten() As Boolean
		Get
			Return mbStraighten AndAlso mdStraightenTolerance > mdToleranceMin
		End Get
	End Property

	Public Property StraightenTolerance() As Double
		Get
			Return mdStraightenTolerance
		End Get
		Set(ByVal bValue As Double)
			mdStraightenTolerance = bValue
			mbStraighten = True
		End Set
	End Property

	Public Property StraightenMinRadius() As Double
		Get
			Return mdStraightenMinRadius
		End Get
		Set(ByVal bValue As Double)
			mdStraightenMinRadius = bValue

		End Set
	End Property

	Public Property StraightenMaxArea() As Double
		Get
			Return mdStraightenMaxArea
		End Get
		Set(ByVal bValue As Double)
			mdStraightenMaxArea = bValue

		End Set
	End Property

	Public ReadOnly Property StraightenRowindex() As Integer
		Get
			Return miStraightenRowIndex
		End Get
	End Property
	Public Property TopoDef() As TopoDef
		Get
			Return moTopoDef
		End Get
		Set(ByVal oValue As TopoDef)
			moTopoDef = oValue
			zzSetTopoDef()
		End Set
	End Property

	Public Property MapThemeData() As MapThemeData
		Get
			Return mtMapThemeData
		End Get
		Set(ByVal oValue As MapThemeData)
			mtMapThemeData = oValue
			zzSetMapThemeData()
		End Set
	End Property
	Public ReadOnly Property Rounding() As enMerging
		Get
			If miRounding = enMerging.None Then
				Return enMerging.None
			ElseIf mdRoundingTolerance > mdToleranceMin Then
				Return miRounding
			Else
				Return enMerging.None
			End If
		End Get
	End Property
	Public Property RoundingTolerance() As Double
		Get
			Return mdRoundingTolerance
		End Get
		Set(ByVal bValue As Double)
			mdRoundingTolerance = bValue
		End Set
	End Property
	Public ReadOnly Property RoundingRowIndex() As Integer
		Get
			Return miRoundingRowIndex
		End Get
	End Property
	Public ReadOnly Property ShortLinesRowIndex() As Integer
		Get
			Return miShortLinesRowIndex
		End Get
	End Property
	Public ReadOnly Property RoundingSPoints() As Boolean
		Get
			Return mbRoundingSPoints
		End Get
	End Property
	Public ReadOnly Property RoundingLines() As Boolean
		Get
			Return mbRoundingLines
		End Get
	End Property
	Public ReadOnly Property PointsNearLines() As Boolean
		Get
			Return mbPointsNearLines
		End Get
	End Property


	Public ReadOnly Property NetPointsLines() As Boolean
		Get
			Return mbNetPointsLines
		End Get
	End Property

	Public ReadOnly Property RemoveDuplicates() As Boolean
		Get
			Return mbRemoveDuplicates AndAlso mdRemoveDuplicatesTolerance > mdToleranceMin
		End Get
	End Property
	Public Property RemoveDuplicatesTolerance() As Double
		Get
			Return mdRemoveDuplicatesTolerance
		End Get
		Set(ByVal bValue As Double)
			mdRemoveDuplicatesTolerance = bValue
			mbRemoveDuplicates = True
		End Set
	End Property
	Public ReadOnly Property RemoveDuplicatesRowIndex() As Integer
		Get
			Return miRemoveDuplicatesRowIndex
		End Get
	End Property
	Public ReadOnly Property RoundAllDecimals() As Integer
		Get
			Return miRoundAllDecimals
		End Get
	End Property
	Public ReadOnly Property ExplodePLines As Boolean
		Get
			Return mbExplodePLines
		End Get
	End Property
	Public ReadOnly Property ExplodePLinesRowIndex() As Integer
		Get
			Return miExplodePLinesRowIndex
		End Get
	End Property
	Public ReadOnly Property ConvertArcs As Boolean
		Get
			Return mbConvertArcs
		End Get
	End Property
	Public ReadOnly Property ConvertArcsRowIndex() As Integer
		Get
			Return miConvertArcsRowIndex
		End Get
	End Property


	Public ReadOnly Property RoundAll As Boolean
		Get
			Return mbRoundAll
		End Get
	End Property
	Public ReadOnly Property RoundAllRowIndex() As Integer
		Get
			Return miRoundAllRowIndex
		End Get
	End Property

	Public ReadOnly Property PointMergingRowIndex() As Integer
		Get
			Return miPointMergingRowIndex
		End Get
	End Property
	Public ReadOnly Property PointMerging() As Boolean
		Get
			Return mbPointMerging
		End Get
	End Property
	Public ReadOnly Property DuplicateLayer() As String
		Get
			If moTopoDef IsNot Nothing Then
				Return moTopoDef.DuplicateLayer
			Else
				Return Nothing
			End If
		End Get
	End Property
	Private Sub zzSetMapThemeData()
		msSourceLayers = mtMapThemeData.LinkLayers
		msBlockNames = mtMapThemeData.NodeBlocks
		miClenupMethods = enClenupMethods.Modify
		msDestLayers = String.Empty
	End Sub
	Private Sub zzSetTopoDef()   '05/05/09
		Dim sNewLinesLayer As String = String.Empty
		Dim sBaseLayer As String = String.Empty
		If moTopoDef.LinkLayersExists Then
			msSourceLayers = mtMapThemeData.LinkLayers
			miClenupMethods = enClenupMethods.Modify
			msDestLayers = String.Empty
		Else
			msSourceLayers = moTopoDef.IncludeLayers
			miClenupMethods = enClenupMethods.Modify
		End If
	End Sub

	Public ReadOnly Property CurrentLayer() As String
		Get
			If Me.CreateNew Then

			End If
			Return "" 'miClenupMethods <> enClenupMethods.Modify
		End Get
	End Property

	Public ReadOnly Property CreateNew() As Boolean
		Get
			Return miClenupMethods <> enClenupMethods.Modify
		End Get
	End Property
	Public ReadOnly Property EraseSource() As Boolean
		Get
			Return miClenupMethods = enClenupMethods.DeleteCreateNew
		End Get
	End Property
	Public ReadOnly Property HasAction() As Boolean
		Get
			Return mbHasAction
		End Get
	End Property
	Public Property BlockNames() As String
		Get
			Return msBlockNames
		End Get
		Set(ByVal sValue As String)
			msBlockNames = sValue
		End Set
	End Property
	Public Property SourceLayers() As String
		Get
			Return msSourceLayers
		End Get
		Set(ByVal sValue As String)
			msSourceLayers = sValue
		End Set
	End Property
	Public Property DestLayers() As String
		Get
			Return msDestLayers
		End Get
		Set(ByVal sValue As String)
			msDestLayers = sValue
		End Set
	End Property
	Public Sub AddDmAction(ByVal iCleanupAction As Integer, ByVal dTolerance As Double, ByVal iRowIndex As Integer)
		Dim iAction As enCleanupAction
		mbHasAction = True
		If [Enum].IsDefined(GetType(enCleanupAction), iCleanupAction) Then
			iAction = CType(iCleanupAction, enCleanupAction)
			Select Case iAction
				Case enCleanupAction.PointsNearLine
					Me.PointLineTolerance = dTolerance
					mbPointsNearLines = True
					miPointLineRowIndex = iRowIndex
				Case enCleanupAction.RoundingPoints
					Me.RoundingTolerance = dTolerance
					miRoundingRowIndex = iRowIndex
					miRounding = enMerging.RoundingSPoints
					mbRoundingSPoints = True
				Case enCleanupAction.NetPointsLines
					Me.RoundingTolerance = dTolerance
					miRoundingRowIndex = iRowIndex
					''''''''  miRounding = enMerging.RoundingSPoints
					mbNetPointsLines = True
				Case enCleanupAction.RoundingLines
					Me.PointLineTolerance = dTolerance
					miRoundingRowIndex = iRowIndex
					mbRoundingLines = True
				Case enCleanupAction.MergingPoints
					If miRounding <> enMerging.Rounding Then
						miRounding = enMerging.Merging
						Me.RoundingTolerance = dTolerance
						miRoundingRowIndex = iRowIndex
					End If
				Case enCleanupAction.ShortLines


					Me.ShortLinesTolerance = dTolerance
					miShortLinesRowIndex = iRowIndex


				Case enCleanupAction.RemoveDuplicates
               Me.RemoveDuplicatesTolerance = dTolerance
               mbRemoveDuplicates = True
            Case enCleanupAction.RoundingAll
               miRoundAllDecimals = Convert.ToInt32(dTolerance)
               mbRoundAll = True
            Case enCleanupAction.ExplodePLines
					mbExplodePLines = True
				Case enCleanupAction.ConvertArcs
					mbConvertArcs = True
					Me.StraightenTolerance = dTolerance
			End Select
      End If
   End Sub
End Structure
Public Structure dmCleanupResult
	Private miMaxActionsUB As Integer
	Private miActionsUB As Integer

	Private miCurrentActionIndex As Integer
	Private miRowIndices() As Integer
	Private miErrNums() As Integer
	Private miErrAddNums() As Integer

	Private moaErrorPoints() As DMAcadExt.TplnPointArray
	Private moaErrorAddPoints() As DMAcadExt.TplnPointArray


	Private mbStraighten As Boolean
	Private mdStraightenTolerance As Double

	Private mdRoundingTolerance As Double
	Private miRoundingRowIndex As Integer
	Private mbRounding As Boolean
	Private msSourceLayers As String
	Public Sub New(ByVal iMaxActionsUB As Integer)
		ReDim miRowIndices(iMaxActionsUB)
		ReDim miErrNums(iMaxActionsUB)
		ReDim miErrAddNums(iMaxActionsUB)

		ReDim moaErrorPoints(iMaxActionsUB)
		ReDim moaErrorAddPoints(iMaxActionsUB)

		miMaxActionsUB = iMaxActionsUB
		miActionsUB = -1
		miCurrentActionIndex = -1
	End Sub
	Public ReadOnly Property RowIndex() As Integer
		Get
			If miActionsUB <> -1 AndAlso miCurrentActionIndex <= miActionsUB Then
				Return miRowIndices(miCurrentActionIndex)
			Else
				Return -1
			End If

		End Get
	End Property
	Public ReadOnly Property ErrNums() As Integer
		Get
			Dim iRes As Integer = -1
			If miActionsUB <> -1 AndAlso miCurrentActionIndex <= miActionsUB Then
				Try
					iRes = miErrNums(miCurrentActionIndex)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(miErrNums.GetUpperBound(0)) & " : " & CStr(miCurrentActionIndex) & vbCrLf & oEx.StackTrace, "ErrNums ***")
				End Try
			End If
			Return iRes
		End Get
	End Property
	Public ReadOnly Property ErrAddNums() As Integer
		Get
			Dim iRes As Integer = -1
			If miActionsUB <> -1 AndAlso miCurrentActionIndex <= miActionsUB Then
				Try
					iRes = miErrAddNums(miCurrentActionIndex)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(miErrNums.GetUpperBound(0)) & " : " & CStr(miCurrentActionIndex) & vbCrLf & oEx.StackTrace, "ErrNums ***")
				End Try
			End If
			Return iRes
		End Get
	End Property


	Public ReadOnly Property ErrorPoints() As TplnPointArray
		Get
			If miActionsUB <> -1 AndAlso miCurrentActionIndex <= miActionsUB Then
				Try
					Return moaErrorPoints(miCurrentActionIndex)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(miErrNums.GetUpperBound(0)) & " : " & CStr(miCurrentActionIndex) & vbCrLf & oEx.StackTrace, "ErrNums ***")
					Return Nothing
				End Try
			Else
				Return Nothing
			End If
		End Get
	End Property
	Public ReadOnly Property HasErrorAddPoints() As Boolean
		Get
			If moaErrorAddPoints IsNot Nothing AndAlso moaErrorAddPoints.GetUpperBound(0) >= 0 Then
				For iIndex As Integer = 0 To moaErrorAddPoints.GetUpperBound(0)
					If moaErrorAddPoints(iIndex) IsNot Nothing AndAlso moaErrorAddPoints(iIndex).Count > 0 Then
						Return True
					End If
				Next
			End If
			Return False
		End Get
	End Property


	Public Function NextAction() As Boolean
		miCurrentActionIndex += 1
		Return miCurrentActionIndex <= miActionsUB
	End Function
	Public Sub AddResult(ByVal iRowIndex As Integer, ByVal oPointArray As DMAcadExt.TplnPointArray, ByVal iErrNum As Integer)
		miActionsUB += 1
		If miActionsUB <= miMaxActionsUB Then
			Try
				miRowIndices(miActionsUB) = iRowIndex
				miErrNums(miActionsUB) = iErrNum
				moaErrorPoints(miActionsUB) = oPointArray

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "dmCleanupResult")
			End Try
		End If
	End Sub

	Public Sub AddResultNew(ByVal iRowIndex As Integer, ByVal oPointArray As DMAcadExt.TplnPointArray, ByVal oPointAddArray As DMAcadExt.TplnPointArray)
		miActionsUB += 1
		If miActionsUB <= miMaxActionsUB Then
			Try
				miRowIndices(miActionsUB) = iRowIndex
				miErrNums(miActionsUB) = oPointArray.Count
				moaErrorPoints(miActionsUB) = oPointArray
				miErrAddNums(miActionsUB) = oPointAddArray.Count
				moaErrorAddPoints(miActionsUB) = oPointAddArray
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "dmCleanupResult")
			End Try
		End If
	End Sub
End Structure
