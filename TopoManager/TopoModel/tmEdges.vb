Option Explicit On
Option Strict On
Public Class tmEdges
	Inherits SortedList(Of Integer, tmEdge)
	Private miRingID As Integer
	Private mbDirection As Boolean
	Public Sub New(iRingID As Integer, bDirection As Boolean)
		miRingID = iRingID
		mbDirection = bDirection
	End Sub
	Public Sub AddEdge(oEdge As tmEdge)

		'	MyBase.Add(oEdge.StartVertex(miRingID, mbDirection), oEdge)
		MyBase.Add(oEdge.StartVertex(miRingID), oEdge)


	End Sub
	Public Sub AddEdges(colEdges As System.Collections.ObjectModel.Collection(Of tmEdge))
		For Each oEdge As tmEdge In colEdges
			Try
				Me.AddEdge(oEdge)
			Catch oEx As Exception
				DMAcadExt.AcadDocument.WriteMessage("Err#045:" & oEx.Message)
				oEdge.DebugWrite(miRingID, "AddErr.")
			End Try

		Next
	End Sub
	Public Sub DebugWrite(sCaption As String)
		For Each oEdge As tmEdge In MyBase.Values
			'oEdge.DebugWrite(sCaption, miRingID, mbDirection)
			oEdge.DebugWrite(sCaption, miRingID)

		Next
	End Sub
   Public Sub CheckVertices(sCaption As String)
      For Each oEdge As tmEdge In MyBase.Values
         'oEdge.DebugWrite(sCaption, miRingID, mbDirection)
         If oEdge.Check(miRingID) Then
            oEdge.DebugWrite(sCaption, miRingID)
         End If


      Next
   End Sub
	Public ReadOnly Property RingID As Integer
		Get
			Return miRingID
		End Get
	End Property
	Public ReadOnly Property Direction As Boolean
		Get
			Return mbDirection
		End Get
	End Property
End Class
