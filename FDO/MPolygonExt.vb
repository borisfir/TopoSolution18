Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Public Enum enMPgonDirection
	[Single]
	Exterior
	Interior
End Enum
Public Class MPolygonExt
	Inherits MPolygon
	Private moSourcePolyline As Polyline
	Private miInnerID As Integer
	Private mtParentID As ObjectId
	Private miDirection As enMPgonDirection
	Private moOuterMPgon As MPolygonExt
	Private mlstInnerMPgons As IList(Of MPolygonExt)
	Private mtSourceAcObjID As ObjectId
	Public Sub New(oPolyline As Polyline)
		MyBase.AppendLoopFromBoundary(oPolyline, False, 0.001)
		miDirection = enMPgonDirection.Single
		mtSourceAcObjID = oPolyline.ObjectId
	End Sub
	Public Sub New(oPolyline As MPolygon)

	End Sub

	Public Property Direction As enMPgonDirection
		Get
			Return miDirection
		End Get
		Set(iValue As enMPgonDirection)
			miDirection = iValue
		End Set
	End Property
	Public ReadOnly Property SourceAcObjID As ObjectId
		Get
			Return mtSourceAcObjID
		End Get
	End Property

	Public Property OuterMPgon As MPolygonExt
		Get
			Return moOuterMPgon
		End Get
		Set(oValue As MPolygonExt)
			moOuterMPgon = oValue
			If miDirection = enMPgonDirection.Single Then
				miDirection = enMPgonDirection.Interior
			End If
		End Set
	End Property
	Public Sub AddInnerMPgon(oMPgon As MPolygonExt)
		mlstInnerMPgons.Add(oMPgon)
		If miDirection = enMPgonDirection.Single Then
			miDirection = enMPgonDirection.Exterior
		End If
	End Sub
	Public Shared Sub Compare(oMPgonA As MPolygonExt, oMPgonB As MPolygonExt)
		Dim oMPgonBase As MPolygonExt
		Dim oMPgonAdd As MPolygonExt
		Dim iLoopsExist As Integer
		Dim oMPgonLoop As MPolygonLoop
		Dim oMPgonLoopB As MPolygonLoop = oMPgonB.GetMPolygonLoopAt(0)


		If oMPgonA.Direction = enMPgonDirection.Single AndAlso oMPgonA.Direction = enMPgonDirection.Exterior Then
			oMPgonBase = oMPgonB
			oMPgonAdd = oMPgonA
		Else
			oMPgonBase = oMPgonA
			oMPgonAdd = oMPgonB
		End If
		iLoopsExist = oMPgonBase.NumMPolygonLoops



		Try
			oMPgonBase.InsertMPolygonLoopAt(iLoopsExist, oMPgonLoopB, False, 0.001)
		Catch oEx As Exception

			Return
		End Try
		oMPgonBase.BalanceTree()


		Dim iDirBase As Autodesk.AutoCAD.DatabaseServices.LoopDirection
		Dim iDirAdd As Autodesk.AutoCAD.DatabaseServices.LoopDirection
		'	Dim bCrossed As Boolean

		oMPgonLoop = oMPgonBase.GetMPolygonLoopAt(0)
		If oMPgonA.LoopCrossesMPolygon(oMPgonLoopB, 0.001) Then
			'ERROR
		Else

		End If

		iDirBase = oMPgonBase.GetLoopDirection(0)
		iDirAdd = oMPgonBase.GetLoopDirection(iLoopsExist)
		'	oEditor.WriteMessage("Dir of Loop # " & CStr(iIndex) & " " & iDir.ToString() & " : " & bCrossed.ToString() & vbCrLf)
		If iDirBase = LoopDirection.Exterior AndAlso iDirAdd = LoopDirection.Interior Then
			oMPgonBase.Direction = enMPgonDirection.Exterior
			oMPgonBase.AddInnerMPgon(oMPgonAdd)
		ElseIf iDirBase = LoopDirection.Interior AndAlso iDirAdd = LoopDirection.Exterior Then

		End If
	End Sub


End Class
