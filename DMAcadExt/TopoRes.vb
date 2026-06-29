Option Explicit On
Option Strict On
Public Structure TopoRes
	Public TopoExists As Boolean
	Public CanOpen As Boolean
	Public PgonCount As Integer
	Public HasElements As Boolean
	Public LinkCount As Integer
	Public CentroidCount As Integer
	Public MissingCntrCount As Integer
   Public InsertedCntrCount As Integer
   Public OutsideCntrCount As Integer
	Public NodeCount As Integer
	Public MissingNodeCount As Integer
	Public InsertedNodeCount As Integer
	Public OutsideNodeCount As Integer

	Public IsComplete As Boolean
	Public IsCorrect As Boolean
	Private mbIsInstance As Boolean
	Public Sub New(bExists As Boolean)
		TopoExists = bExists
		mbIsInstance = True
	End Sub
	Public ReadOnly Property IsInstance As Boolean
		Get
			Return mbIsInstance
		End Get
	End Property
	Public ReadOnly Property IsOK As Boolean
		Get
         Return IsCorrect AndAlso IsComplete AndAlso PgonCount <> 0 AndAlso MissingCntrCount = 0 AndAlso OutsideCntrCount = 0
		End Get
   End Property
   Public ReadOnly Property IsTopoOK As Boolean
      Get
         Return IsCorrect AndAlso IsComplete AndAlso PgonCount <> 0
      End Get
   End Property
End Structure