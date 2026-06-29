Option Explicit On
Option Strict On
Public Class tmNode
	Private miID As Integer
	Private mtPoint As Autodesk.AutoCAD.Geometry.Point3d
	Private msetExteriorRings As HashSet(Of Integer)
	Private moTrueNode As tmNode
	Public Sub New(iID As Integer, tPoint As Autodesk.AutoCAD.Geometry.Point3d, iRingA_ID As Integer, iRingB_ID As Integer)
		miID = iID
		mtPoint = tPoint
		msetExteriorRings = New HashSet(Of Integer)
		msetExteriorRings.Add(iRingA_ID)
		msetExteriorRings.Add(iRingB_ID)

	End Sub
	Public ReadOnly Property ID As Integer
		Get
			Return miID
		End Get
	End Property
	Public ReadOnly Property Point2d As Autodesk.AutoCAD.Geometry.Point2d
		Get
			Return New Autodesk.AutoCAD.Geometry.Point2d(mtPoint.X, mtPoint.Y)
		End Get
	End Property
	Public ReadOnly Property Point3d As Autodesk.AutoCAD.Geometry.Point3d
		Get
			Return mtPoint
		End Get
	End Property
	Public ReadOnly Property TrueID As Integer
		Get
			If moTrueNode Is Nothing Then
				Return miID
			Else
				Return moTrueNode.TrueID
			End If
		End Get
	End Property
	Public Property TrueNode As tmNode
		Get
			If moTrueNode Is Nothing Then
				Return Me
			Else
				Return moTrueNode.TrueNode
			End If
		End Get
		Set(oValue As tmNode)
			moTrueNode = oValue
		End Set
	End Property

	Public ReadOnly Property Rings As HashSet(Of Integer)
		Get
			Return msetExteriorRings
		End Get
	End Property
	Public Shared Operator And(oNodeA As tmNode, oNodeB As tmNode) As tmNode
		Dim oTrueNodeA As tmNode = oNodeA.TrueNode
		Dim oTrueNodeB As tmNode = oNodeB.TrueNode

		If oNodeA.ID < oNodeB.ID Then
			oNodeA.AddTrueNode(oNodeB)
			Return oNodeA
		ElseIf oNodeA.ID < oNodeB.ID Then
			oNodeB.AddTrueNode(oNodeA)
			Return oNodeB
		Else
			Return oNodeA
		End If
	End Operator
	Public Shared Operator =(oNodeA As tmNode, oNodeB As tmNode) As Boolean
		Dim setRingsA As HashSet(Of Integer) = oNodeA.Rings
		Dim setRingsB As HashSet(Of Integer) = oNodeB.Rings
		Return setRingsA.SetEquals(setRingsB)

	End Operator
	Public Shared Operator <>(oNodeA As tmNode, oNodeB As tmNode) As Boolean
		Return Not (oNodeA = oNodeB)
	End Operator
	Public Sub AddTrueNode(oTrueNode As tmNode)
		msetExteriorRings.UnionWith(oTrueNode.Rings)
		oTrueNode.TrueNode = Me
	End Sub
	Public Function DebugString() As String
      Return CStr(miID) & ":" & DMAcadExt.TPlnPoint.DispPoint(mtPoint) & " TrueID:" & CStr(Me.TrueNode.ID) & "; Rings:" & RingList()
	End Function
	Public Sub DebugWrite()
      DMAcadExt.AcadDocument.WriteLog(CStr(miID) & ":" & DMAcadExt.TPlnPoint.DispPoint(mtPoint) & " TrueID:" & CStr(Me.TrueNode.ID) & ", " & "Rings:" & RingList())
	End Sub
	Public Function RingList() As String
		Dim sRes As String = String.Empty
		If moTrueNode Is Nothing Then
			For Each iRingID As Integer In msetExteriorRings
				If sRes.Length <> 0 Then
					sRes &= ","
				End If
				sRes &= CStr(iRingID)
			Next
		End If
		Return sRes
	End Function
End Class
