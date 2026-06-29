Option Explicit On
Option Strict On
Public Class bmSubproperties
	Inherits bmPolygonArray
	Private moaPgonGround() As BamashPolygon = {Nothing, Nothing, Nothing}
	Private miPgonGroundCount As Integer = 0
	Private moPgonRoof As BamashPolygon = Nothing
	Private moaPgonOthers() As BamashPolygon = {Nothing, Nothing, Nothing, Nothing, Nothing, Nothing}
	Private miPgonOthersCount As Integer = 0
	Private miFictCount As Integer = 0
	Private moaPgonReal() As BamashPolygon
	'	Private moUnderColorPgon As BamashPolygon
	Private moUnderWhitePgon As BamashPolygon

	Public Sub New()

	End Sub

	Public Overrides Sub AddPolygon2009(ByVal oPolygon As BamashPolygon)
		MyBase.AddPolygon2009(oPolygon)
		If oPolygon.PropertyType = enPropertyTypes.UnderBldTypeWhite Then
			miFictCount += 1
			moUnderWhitePgon = oPolygon
		End If
		'	DMAcadExt.AcadDocument.WriteMessage("PropertyID=" & CStr(oPolygon.PropertyID))
		'	Dim iPgonIndex As Integer = MyBase.Count - 1

	End Sub
	Public Sub Calculate()
		ReDim moaPgonReal(MyBase.Count - miFictCount)
		Dim iRealIndex As Integer = 0
		Dim oPolygon As BamashPolygon
		'	MyBase.SortPgons()
		For iIndex As Integer = 0 To MyBase.Count - 1
			oPolygon = MyBase.Item(iIndex)
			If oPolygon.PropertyType <> enPropertyTypes.UnderBldTypeWhite Then
				moaPgonReal(iRealIndex) = oPolygon
				iRealIndex += 1
				If oPolygon.PropertyType = enPropertyTypes.UnderBldTypeColor AndAlso moUnderWhitePgon IsNot Nothing Then
					oPolygon.AdditionalArea = moUnderWhitePgon.AcadArea(False)
				End If

				Select Case oPolygon.AprtDescNum
					Case 7
						If moPgonRoof Is Nothing Then
							moPgonRoof = oPolygon
						Else
							' Print Error
						End If
					Case 8
						If miPgonGroundCount <= moaPgonGround.GetUpperBound(0) Then
							moaPgonGround(miPgonGroundCount) = oPolygon
							miPgonGroundCount += 1
						Else
							' Print Error
						End If
					Case Else
						If miPgonOthersCount <= moaPgonOthers.GetUpperBound(0) Then
							moaPgonOthers(miPgonOthersCount) = oPolygon
							miPgonOthersCount += 1
						Else
							' Print Error
						End If
				End Select
			End If
		Next
	End Sub
	Public Sub SubNumerateAAA(ByVal iCalcOption As bmBamash.SubNumerationOptions)
		Dim oBamashPgon As BamashPolygon

		Dim iColorPgonNum As Integer = 0
		For iIndex As Integer = 0 To moaPgonReal.GetUpperBound(0)
			oBamashPgon = moaPgonReal(iIndex)
			Select Case iCalcOption
				Case bmBamash.SubNumerationOptions.All
					oBamashPgon.SubPropNum = bmBamash.GetNext
				Case bmBamash.SubNumerationOptions.EmptyFirst, bmBamash.SubNumerationOptions.EmptyMax

					If oBamashPgon.SubPropNum = 0 Then
						oBamashPgon.SubPropNum = bmBamash.GetNextFree()
					End If
				Case bmBamash.SubNumerationOptions.None
					oBamashPgon.CheckSubPropNum()
			End Select
			If oBamashPgon.PropertyType = enPropertyTypes.UnderBldTypeColor Then
				moUnderWhitePgon.SubPropNum = oBamashPgon.SubPropNum
			End If
		Next
	End Sub
	Public Sub PrintList(Optional sPrefix As String = Nothing)
		Dim oPgon As BamashPolygon
		Dim sOut As String
		If Not String.IsNullOrEmpty(sPrefix) Then
			DMAcadExt.AcadDocument.WriteMessage(sPrefix)
		End If
		For iIndex As Integer = 0 To MyBase.Count - 1
			oPgon = MyBase.Item(iIndex)
			sOut = CStr(iIndex) & ":" & oPgon.BldFloor & ";" & oPgon.Caption & ";" & oPgon.PropertyID
			DMAcadExt.AcadDocument.WriteMessage(sOut)
		Next
	End Sub
	Public Function GetNewNumber() As Integer
		Me.SortSubprops(bmBamash.SubNumerationOptions.EmptyFirst)
	End Function
	Public ReadOnly Property RealCount() As Integer
		Get
			Return MyBase.Count - miFictCount
		End Get
	End Property
	Public ReadOnly Property PgonReal(ByVal iIndex As Integer) As BamashPolygon
		Get
			Return moaPgonReal(iIndex)
		End Get
	End Property
	Public ReadOnly Property PgonGround(ByVal iIndex As Integer) As BamashPolygon
		Get
			Return moaPgonGround(iIndex)
		End Get
	End Property
	Public ReadOnly Property PgonGroundCount() As Integer
		Get
			Return miPgonGroundCount
		End Get
	End Property
	Public ReadOnly Property PgonRoof() As BamashPolygon
		Get
			Return moPgonRoof
		End Get
	End Property
	Public ReadOnly Property PgonOthers(ByVal iIndex As Integer) As BamashPolygon
		Get
			Return moaPgonOthers(iIndex)
		End Get
	End Property
	Public ReadOnly Property PgonOthersCount() As Integer
		Get
			Return miPgonOthersCount
		End Get
	End Property
End Class
