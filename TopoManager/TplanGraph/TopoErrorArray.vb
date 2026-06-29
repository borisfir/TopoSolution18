Option Explicit On
Option Strict On
Public Class TopoErrorArray

	Private mtaTopoErrors() As TopoError
	Private miUpperBound As Integer
	Private miCurrentIndex As Integer = 0
	Private mbSourceTopo As Boolean

	Public Sub New()
		miUpperBound = -1
	End Sub
	Public Sub New(ByVal iUpperBound As Integer)
		miUpperBound = iUpperBound
		If miUpperBound >= 0 Then
			ReDim Preserve mtaTopoErrors(miUpperBound)
		End If
	End Sub
	Public Sub New(ByVal taTopoErrors() As TopoError)
		mtaTopoErrors = taTopoErrors
		miUpperBound = mtaTopoErrors.GetUpperBound(0)
	End Sub
	Public Sub Add(ByVal oaTopoErrorArray As TopoErrorArray)
		Dim oPoint As TopoError
		Dim iStartIndex As Integer = miUpperBound + 1
		Dim iIndex As Integer

		Me.AddDim(oaTopoErrorArray.UpperBound)
		For iIndex = 0 To oaTopoErrorArray.UpperBound
			oPoint = oaTopoErrorArray.Item(iIndex)
			Me.Add(oPoint, iStartIndex + iIndex)
		Next

	End Sub
	Public Sub AddDim(ByVal iUpperBound As Integer)
		miUpperBound += iUpperBound + 1
		If iUpperBound >= 0 Then
			ReDim Preserve mtaTopoErrors(miUpperBound)
		End If
	End Sub
	Public Sub Add(ByVal oPoint As TopoError, ByVal iIndex As Integer)
		mtaTopoErrors(iIndex) = oPoint
	End Sub
	Public Sub Add(ByVal oPoint As TopoError)
		miUpperBound += 1
		ReDim Preserve mtaTopoErrors(miUpperBound)
		mtaTopoErrors(miUpperBound) = oPoint

	End Sub
	Public Sub Clear()

	End Sub
	Public Sub Reset()
		If miUpperBound >= 0 Then
			miCurrentIndex = 0
		End If
	End Sub
	Public Function MoveNext() As Boolean
		If miUpperBound >= 0 AndAlso miCurrentIndex <= miUpperBound Then
			miCurrentIndex += 1
			Return miCurrentIndex <= miUpperBound
		Else
			Return False
		End If
	End Function
	Public ReadOnly Property Current() As TopoError
		Get
			If miUpperBound >= 0 AndAlso miCurrentIndex <= miUpperBound Then
				Return Me.Item(miCurrentIndex)
			Else
				Return Nothing
			End If
		End Get
	End Property
	Public Property SourceTopo As Boolean
		Get
			Return mbSourceTopo
		End Get
		Set(bValue As Boolean)
			mbSourceTopo = bValue
		End Set
	End Property
	Public Property UpperBound() As Integer
		Get
			Return miUpperBound
		End Get
		Set(ByVal iValue As Integer)
			Try
				ReDim Preserve mtaTopoErrors(iValue)
				miUpperBound = iValue
			Catch oEx As Exception

			End Try
		End Set
	End Property
	Public Property Item(ByVal iIndex As Integer) As TopoError
		Get
			Try
				If iIndex >= 0 AndAlso iIndex <= miUpperBound Then
					Return mtaTopoErrors(iIndex)
				End If


			Catch oEx As Exception
				Return Nothing
			End Try
			Return New TopoError
		End Get
		Set(ByVal oValue As TopoError)
			Try
				mtaTopoErrors(iIndex) = oValue
			Catch oEx As Exception

			End Try
		End Set
	End Property

End Class

