Option Explicit On
Option Strict On
Public Class tmVertex
	Private miIndex As Integer
	Private miRingID As Integer
	Private mtPoint As Autodesk.AutoCAD.Geometry.Point3d
	Private moTrueNode As tmNode
	Private mbIsPseudo As Boolean
	Private mbIsDoubt As Boolean

	Public Sub New(iRingID As Integer, iIndex As Integer, tPoint As Autodesk.AutoCAD.Geometry.Point3d)
		miRingID = iRingID
		miIndex = iIndex
		mtPoint = tPoint
	End Sub
	Public Sub ConnectNode(oNode As tmNode)
		If moTrueNode Is Nothing Then
			moTrueNode = oNode
			'DMAcadExt.AcadDocument.WriteMessage("First -" & " R=" & CStr(miRingID) & ", In=" & CStr(miIndex) & "  ** Added N=" & CStr(oNode.ID))
		ElseIf moTrueNode.ID <> oNode.ID Then
			'	DMAcadExt.AcadDocument.WriteMessage("R=" & CStr(miRingID) & ", In=" & CStr(miIndex) & " || " & CStr(moTrueNode.ID) & ", TrueID=" & CStr(moTrueNode.TrueID) & " ** Added N=" & CStr(oNode.ID))
			moTrueNode = moTrueNode And oNode
			'''''''''''''''''''''	DMAcadExt.AcadDocument.WriteMessage("  After   TrueID=" & CStr(moTrueNode.TrueID) & "  trueId of N =" & oNode.TrueID)
		End If
	End Sub
	Public Property IsPseudo As Boolean
		Get
			Return mbIsPseudo
		End Get
		Set(bValue As Boolean)
			mbIsPseudo = bValue
		End Set
	End Property
	Public Property IsDoubt As Boolean
		Get
			Return mbIsDoubt
		End Get
		Set(bValue As Boolean)
			mbIsDoubt = bValue
		End Set
	End Property
	Public ReadOnly Property Node As tmNode
		Get
			Return moTrueNode
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
	Public ReadOnly Property Index As Integer
		Get
			Return miIndex
		End Get
	End Property
	Public ReadOnly Property RingID As Integer
		Get
			Return miRingID
		End Get
	End Property
	Public Sub TmpDisplay()
		If moTrueNode Is Nothing Then

			DMAcadExt.AcadDocument.WriteMessage("VertexInfo: Node=Ng  R=" & CStr(miRingID) & ", In=" & CStr(miIndex))
		Else
			DMAcadExt.AcadDocument.WriteMessage("VertexInfo: Node=" & CStr(moTrueNode.ID) & " R=" & CStr(miRingID) & ", In=" & CStr(miIndex) & " || " & CStr(moTrueNode.ID))

		End If
	End Sub
End Class
