Option Explicit On
Option Strict On
Public Structure TopoDefID
   Private Const miTopoRange As Integer = 128
   Public ID As Integer
   Public Sub New(ByVal iSourceTopoID As TPlanGraph.enTopoPurpose, ByVal iOverlayTopoID As TPlanGraph.enTopoPurpose, ByVal iOverlayMethod As TPlanGraph.enOverlayMethod)
      If iSourceTopoID <> TPlanGraph.enTopoPurpose.Undefined And iOverlayTopoID <> TPlanGraph.enTopoPurpose.Undefined Then
         If iOverlayMethod = TPlanGraph.enOverlayMethod.Union Then
            ID = miTopoRange * iSourceTopoID + miTopoRange * miTopoRange + iOverlayTopoID
         ElseIf iOverlayMethod = TPlanGraph.enOverlayMethod.Merge Then
            ID = miTopoRange * iSourceTopoID + iOverlayTopoID
         End If
      Else
         ID = -1
      End If
	End Sub
	Public Sub New(ByVal iTopoID As Integer)
		ID = iTopoID
	End Sub

	Public ReadOnly Property TopoIsBase() As Boolean
		Get
			Return (ID < miTopoRange)
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
	Public ReadOnly Property SourceID() As TopoDefID
		Get
			Dim stSourceTopoDefID As TopoDefID
			If Me.TopoIsMerge Then
				stSourceTopoDefID.ID = CInt((ID - ID Mod miTopoRange) / miTopoRange)
				Return stSourceTopoDefID
			ElseIf Me.TopoIsUnion Then
				Dim iID As Integer = ID - miTopoRange * miTopoRange
				stSourceTopoDefID.ID = CInt((iID - iID Mod miTopoRange) / miTopoRange)
				Return stSourceTopoDefID
			End If

		End Get
	End Property

	Public ReadOnly Property OverlayID() As TopoDefID
		Get
			Dim stOverlayTopoDefID As TopoDefID
			If Me.TopoIsMerge Then
				stOverlayTopoDefID.ID = ID Mod miTopoRange
				Return stOverlayTopoDefID
			ElseIf Me.TopoIsUnion Then
				Dim iID As Integer = ID - miTopoRange * miTopoRange
				stOverlayTopoDefID.ID = ID Mod miTopoRange
				Return stOverlayTopoDefID
			End If
		End Get
	End Property
	Public Sub New(ByVal iTopoPurpose As TPlanGraph.enTopoPurpose)
		ID = CType(iTopoPurpose, Integer)
	End Sub

	Public ReadOnly Property BaseID() As TPlanGraph.enTopoPurpose
		Get
			Select Case ID
				Case Is < miTopoRange
					Return CType(ID, TPlanGraph.enTopoPurpose)
				Case Is < miTopoRange * miTopoRange
					Return CType(ID Mod miTopoRange, TPlanGraph.enTopoPurpose)
				Case Else
					Return CType((ID - miTopoRange * miTopoRange) Mod miTopoRange, TPlanGraph.enTopoPurpose)
			End Select
		End Get
	End Property

End Structure
