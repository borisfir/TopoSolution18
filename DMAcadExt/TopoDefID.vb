Option Explicit On
Option Strict On
Public Enum enTopoType
	Base = 2
	Dissolve = 4
	Additional = 8

	Merge = 128
	Union = 256
	FDO_Overlay = 512
End Enum
Public Structure TopoDefID
	Private Const miTopoRange As Integer = 128
	Private Const miTopoAdditional As Integer = 64
	Private Const miTopoDissolve As Integer = 72


	Public ID As Integer
	Private miBaseID As Integer
	Private miDissolveID As Integer
	Private miAdditionalID As Integer
	Private miSourceAID As Integer
	Private miSourceBID As Integer
	Private miTopoPurpose As DMAcadExt.enTopoPurpose
	Private miOverlayMethod As DMAcadExt.enOverlayMethod
	Private miOverlayIndex As DMAcadExt.enOverlayIndex
	Public Shared Function GetTopoPurpose(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex) As DMAcadExt.enTopoPurpose
		Select Case iOverlayIndex
			Case DMAcadExt.enOverlayIndex.ApprMerge, DMAcadExt.enOverlayIndex.ApprUnion, DMAcadExt.enOverlayIndex.ApprFDO_Overlay
				Return DMAcadExt.enTopoPurpose.Approved
			Case DMAcadExt.enOverlayIndex.PropMerge, DMAcadExt.enOverlayIndex.PropUnion, DMAcadExt.enOverlayIndex.PropFDO_Overlay
				Return DMAcadExt.enTopoPurpose.Proposed
			Case Else
				Return Nothing
		End Select
	End Function

	Public Sub New(ByVal iSourceTopoID As enTopoPurpose, ByVal iOverlayTopoID As enTopoPurpose, ByVal iOverlayMethod As enOverlayMethod)
		If iSourceTopoID <> enTopoPurpose.Undefined And iOverlayTopoID <> enTopoPurpose.Undefined Then
			If iOverlayMethod = enOverlayMethod.Union Then
				ID = miTopoRange * iSourceTopoID + miTopoRange * miTopoRange + iOverlayTopoID
			ElseIf iOverlayMethod = enOverlayMethod.Merge Then
				ID = miTopoRange * iSourceTopoID + iOverlayTopoID
			ElseIf iOverlayMethod = enOverlayMethod.FDO_Overlay Then
				ID = miTopoRange * iSourceTopoID + 2 * miTopoRange * miTopoRange + iOverlayTopoID
			End If
		Else
			ID = -1
		End If

	End Sub
	Public Sub New(ByVal iOverlayIndex As DMAcadExt.enOverlayIndex)
		miOverlayIndex = iOverlayIndex
		miTopoPurpose = GetTopoPurpose(iOverlayIndex)
		miOverlayMethod = GetOverlayMethod(iOverlayIndex)
		ID = 40000



	End Sub
	Public Sub New(ByVal iSourceTopoID As enTopoPurpose, ByVal iOverlayMethod As enOverlayMethod)
		If iSourceTopoID <> enTopoPurpose.Undefined Then
			If iOverlayMethod = enOverlayMethod.Dissolve Then
				ID = iSourceTopoID + miTopoDissolve
			End If
		Else
			ID = -1
		End If

	End Sub
	Public Sub New(ByVal iTopoID As Integer)
		ID = iTopoID
	End Sub
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

	Public Sub NewAAA(ByVal iTopoPurpose As enTopoPurpose)
		ID = CType(iTopoPurpose, Integer)
	End Sub
	Public ReadOnly Property TopoIsAdditional() As Boolean
		Get
			Return (ID > miTopoAdditional) AndAlso (ID <= miTopoDissolve)
		End Get
	End Property
	Public ReadOnly Property TopoIsBase() As Boolean
		Get
			Return (ID <= miTopoAdditional)
		End Get
	End Property

	Public ReadOnly Property TopoIsUnion() As Boolean
		Get
			Return (ID > miTopoRange * miTopoRange) And (ID < 2 * miTopoRange * miTopoRange)
		End Get
	End Property

	Public ReadOnly Property TopoIsMerge() As Boolean
		Get
			Return (ID > miTopoRange) And (ID < miTopoRange * miTopoRange)
		End Get
	End Property
	Public ReadOnly Property TopoIsDissolve() As Boolean
		Get
			Return (ID > miTopoDissolve) AndAlso (ID < miTopoRange)
		End Get
	End Property
	Public ReadOnly Property TopoIsFDO_Overlay() As Boolean
		Get
			Return (ID > 2 * miTopoRange * miTopoRange) And (ID < 3 * miTopoRange * miTopoRange)
		End Get
	End Property
	Public ReadOnly Property SourceID() As TopoDefID
		Get
			Dim tSourceTopoDefID As TopoDefID
			If Me.TopoIsMerge Then
				tSourceTopoDefID.ID = CInt((ID - ID Mod miTopoRange) / miTopoRange)
				Return tSourceTopoDefID
			ElseIf Me.TopoIsUnion Then
				Dim iID As Integer = ID - miTopoRange * miTopoRange
				tSourceTopoDefID.ID = CInt((iID - iID Mod miTopoRange) / miTopoRange)
				Return tSourceTopoDefID
			ElseIf Me.TopoIsFDO_Overlay Then
				Dim iID As Integer = ID - 2 * miTopoRange * miTopoRange
				tSourceTopoDefID.ID = CInt((iID - iID Mod miTopoRange) / miTopoRange)
				System.Windows.Forms.MessageBox.Show(CStr(CInt((iID - iID Mod miTopoRange) / miTopoRange)), "SourceID.D")
				Return tSourceTopoDefID
			ElseIf Me.TopoIsDissolve Then
				tSourceTopoDefID.ID = ID - miTopoDissolve
				Return tSourceTopoDefID
			End If
		End Get
	End Property

	Public ReadOnly Property OverlayID() As TopoDefID
		Get
			Dim tOverlayTopoDefID As TopoDefID
			If Me.TopoIsMerge Then
				tOverlayTopoDefID.ID = ID Mod miTopoRange
				Return tOverlayTopoDefID
			ElseIf Me.TopoIsUnion Then
				Dim iID As Integer = ID - miTopoRange * miTopoRange
				tOverlayTopoDefID.ID = ID Mod miTopoRange
				Return tOverlayTopoDefID
			End If
		End Get
	End Property
	Public ReadOnly Property DissolveID() As TopoDefID
		Get
			Dim tDissolveTopoDefID As TopoDefID
			If Me.TopoIsBase Then
				tDissolveTopoDefID.ID = ID + miTopoDissolve
				Return tDissolveTopoDefID
			End If
		End Get
	End Property
	Public ReadOnly Property AdditionalID() As TopoDefID
		Get
			Dim tAdditionalTopoDefID As TopoDefID
			If Me.TopoIsBase Then
				tAdditionalTopoDefID.ID = (ID + miTopoAdditional)
				Return tAdditionalTopoDefID
			End If
		End Get
	End Property
	 
	

	Public ReadOnly Property BaseID() As enTopoPurpose
		Get
			Select Case ID
				Case Is <= miTopoAdditional
					Return CType(ID, enTopoPurpose)
				Case Is <= miTopoRange
					Return CType(ID - miTopoAdditional, enTopoPurpose)
				Case Is < miTopoRange * miTopoRange
					Return CType(ID Mod miTopoRange, enTopoPurpose)
				Case Else
					Return CType((ID - miTopoRange * miTopoRange) Mod miTopoRange, enTopoPurpose)
			End Select
		End Get
	End Property
	Public ReadOnly Property TopoPurpose As DMAcadExt.enTopoPurpose
		Get
			Return miTopoPurpose
		End Get
	End Property
	Public ReadOnly Property OverlayMethod As DMAcadExt.enOverlayMethod
		Get
			Return miOverlayMethod
		End Get
	End Property


	Public ReadOnly Property OverlayIndex As DMAcadExt.enOverlayIndex
		Get
			Return miOverlayIndex
		End Get
	End Property
	Public ReadOnly Property CleanupEnabled() As Boolean
		Get
			Return TopoIsBase
		End Get
	End Property
End Structure

