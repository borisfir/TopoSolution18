Option Explicit On
Option Strict On
Public Class MarkBlock
	Private Const mdMakerStepToSize As Double = 0.0 '0.05
	Public Shared MarkBlockNames() As String = New String() {"dmMarkCircle", "dmMarkTriangle", "dmMarkSquare", "dmMarkRhombus", "dmMarkSaltire", "dmMarkCell", "dmMarkV"}
	Private mdMakerSizeStep As Double = 0.0 ' 0.00001
	Private Shared mdMakerSizeInitial As Double = 0.01
	'Private Shared mdMakerSizeInitial As Double = 10000.0

	Private mdMakerSize As Double

	Private mdCurrentViewDim As Double
	Private Shared CurrentViewDim As Double
	Private mdConstScale As Double
	Private mbHasConstScale As Boolean

	Private mdConstRotation As Double

	Private mcolBlockRefObjIds As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
	Private msBlockName As String

	Public Enum enMarkBlockType
		Circle
		Triangle
		Square
		Rhombus
		Saltire
		Cell
		V
	End Enum
	Public Shared Function GetMarkBlockList() As String
		Return Join(MarkBlockNames, ",")
	End Function
	Public Shared Sub EraseAllReferences()
		For iIndex As Integer = 0 To MarkBlockNames.GetUpperBound(0)
			'  System.Windows.Forms.MessageBox.Show(MarkBlockNames(iIndex), "05_490")
			zzEraseReferences(MarkBlockNames(iIndex))
		Next

	End Sub
	Public Shared Sub ResetAllRefs()
		CurrentViewDim = AcadDocument.GetCurrentViewDim()
		For iIndex As Integer = 0 To MarkBlockNames.GetUpperBound(0)
			ResetRefs(MarkBlockNames(iIndex))
		Next
	End Sub
	Public Shared Sub ResetRefs(sBlockName As String)
		Dim colRefs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = AcadTransaction.GetAllBlockRefs(sBlockName)
		If colRefs IsNot Nothing AndAlso colRefs.Count > 0 Then
			Dim oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference
			Dim dScale As Double = mdMakerSizeInitial * CurrentViewDim
			'  System.Windows.Forms.MessageBox.Show(sBlockName, "05_590")
			For Each tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId In colRefs
				oBlockRef = AcadTransaction.GetBlockRef(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
				oBlockRef.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(dScale)
			Next
		End If
	End Sub
	Private miMarkBlockType As enMarkBlockType
	'Private mdScale As Double

	Private mtBlockObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
	Public Sub New(ByVal iMarkBlockType As enMarkBlockType, Optional dConstScale As Double = 0.0, Optional dConstRotation As Double = 0.0)
		Dim iIntMarkBlockType As Integer = CType(iMarkBlockType, Integer)
		miMarkBlockType = iMarkBlockType
		Select Case iMarkBlockType
			Case enMarkBlockType.Circle
				msBlockName = MarkBlockNames(iIntMarkBlockType)
				mtBlockObjID = AcadTransaction.GetCircleBlock(msBlockName)
			Case enMarkBlockType.Triangle
				msBlockName = MarkBlockNames(iIntMarkBlockType)
				mtBlockObjID = AcadTransaction.GetTriangleBlock(msBlockName)
			Case enMarkBlockType.Square
				msBlockName = MarkBlockNames(enMarkBlockType.Square)
				mtBlockObjID = AcadTransaction.GetSquareBlock(msBlockName)
			Case enMarkBlockType.Rhombus
				msBlockName = MarkBlockNames(iIntMarkBlockType)
				mtBlockObjID = AcadTransaction.GetRhombusBlock(msBlockName)
			Case enMarkBlockType.Saltire
				msBlockName = MarkBlockNames(iIntMarkBlockType)
				mtBlockObjID = AcadTransaction.GetSalltireBlock(msBlockName)
			Case enMarkBlockType.Cell
				msBlockName = MarkBlockNames(iIntMarkBlockType)
				mtBlockObjID = AcadTransaction.GetCellBlock(msBlockName)
			Case enMarkBlockType.V
				msBlockName = MarkBlockNames(iIntMarkBlockType)
				mtBlockObjID = AcadTransaction.GetVBlock(msBlockName)
		End Select
		mdMakerSize = mdMakerSizeInitial
		mdMakerSizeStep = mdMakerSize * mdMakerStepToSize
		If dConstScale = 0.0 Then
			mdCurrentViewDim = AcadDocument.GetCurrentViewDim()
			mbHasConstScale = False
		Else
			mdConstScale = dConstScale
			mbHasConstScale = True
		End If
		mdConstRotation = dConstRotation

	End Sub
	Private Function zzGetBlockRefScale() As Double
		If mbHasConstScale Then
			Return mdConstScale
		Else
			Return mdMakerSize * mdCurrentViewDim
		End If
	End Function
	Private Function zzGetBlockRefScaleByCurrentView() As Double
		Return mdCurrentViewDim
	End Function
	Private Function zzGetBlockRefConstScale() As Double
		Return mdConstScale
	End Function
	Public Property MakerSize As Double
		Get
			Return mdMakerSize
		End Get
		Set(dValue As Double)
			mdMakerSize = dValue
		End Set
	End Property

	Public ReadOnly Property BlockName As String
		Get
			Return msBlockName
		End Get
	End Property
	Public Sub ResetMakerSize()
		mdMakerSize = mdMakerSizeInitial
	End Sub
	Public Sub MarkPoint(ByVal tPoint As Autodesk.AutoCAD.Geometry.Point2d, ByVal shColor As Short)
		Dim dScale As Double = zzGetBlockRefScale()
		'DMCommon.ExcelLogAW5.SetNextValue(0, "MPoint2d", BlockName, dScale, shColor)
		Dim iAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId = AcadTransaction.InsertBlockRef(mtBlockObjID, TPlnPoint.Point2dTo3d(tPoint),,,, )
		' AcadDocument.WriteMessage("iAcObjID=" & iAcObjID.ToString() & "; S=" & dScale.ToString())
		mdMakerSize += mdMakerSizeStep
		'   Dim oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference = AcadTransaction.GetBlockRef(iAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
		'   AcadDocument.WriteMessage("iAcObjID=" & iAcObjID.ToString() & " H=" & oBlockRef.Handle.ToString() & " Name=" & oBlockRef.Name)

	End Sub
	Public Sub MarkPoint(ByVal tPoint As Autodesk.AutoCAD.Geometry.Point3d, ByVal shColor As Short)
		Dim dScale As Double = zzGetBlockRefScale()
		'	DMCommon.ExcelLogAW5.SetNextValue(0, "MPoint3d", BlockName, dScale)
		AcadTransaction.InsertBlockRef(mtBlockObjID, tPoint, , , dScale, shColor)
		mdMakerSize += mdMakerSizeStep
	End Sub
	Public Function InsertPoint(ByVal tPoint As Autodesk.AutoCAD.Geometry.Point3d, ByVal shColor As Short) As Autodesk.AutoCAD.DatabaseServices.BlockReference
		Dim dScale As Double = zzGetBlockRefScale()
		DMCommon.Debug.MsgBox("141122_3", dScale)
		Return AcadTransaction.InsertGetBlockRef(mtBlockObjID, tPoint, dScale, shColor)
		'mdMakerSize += mdMakerSizeStep
	End Function
	Public Sub ScaledMarkPoint(ByVal tPoint As Autodesk.AutoCAD.Geometry.Point2d, ByVal shColor As Short, dUserScaleX As Double, Optional dUserScaleY As Double = 0.000)
		Dim dScale As Double = zzGetBlockRefScaleByCurrentView()


		If dUserScaleY = 0.0 Then
			dUserScaleY *= dUserScaleX
		End If

		Dim iAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId = AcadTransaction.InsertBlockRef(mtBlockObjID, TPlnPoint.Point2dTo3d(tPoint), , , dScale, shColor)
		' AcadDocument.WriteMessage("iAcObjID=" & iAcObjID.ToString() & "; S=" & dScale.ToString())

		'   Dim oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference = AcadTransaction.GetBlockRef(iAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead)
		'   AcadDocument.WriteMessage("iAcObjID=" & iAcObjID.ToString() & " H=" & oBlockRef.Handle.ToString() & " Name=" & oBlockRef.Name)

	End Sub

	Public Sub MarkBox(ByVal oBox As TPlnBoundingBox, ByVal shColor As Short)
		'Dim dScale As Double = mdMakerSize * mdCurrentViewDim
		Dim dK As Double = Math.Sin(Math.PI / 4.0)
		Dim dScaleX As Double = oBox.Width * dK
		Dim dScaleY As Double = oBox.Height * dK



		AcadTransaction.InsertBlockRef(mtBlockObjID, oBox.GetCenterPoint().AcGePoint3d, , , dScaleX, shColor, , , dScaleY)

	End Sub

	Public Sub EraseMyReferences()
		zzEraseReferences(msBlockName)
	End Sub

	Public Sub LoadAllReferences()
		mcolBlockRefObjIds = DMAcadExt.AcadTransaction.GetAllBlockRefs(msBlockName)
	End Sub
	Public Sub ResetScales()
		mdCurrentViewDim = AcadDocument.GetCurrentViewDim()
		Dim dScale As Double = mdMakerSize * mdCurrentViewDim
		Dim oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference
		For Each tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId In mcolBlockRefObjIds
			oBlockRef = AcadTransaction.GetBlockRef(tAcObjID, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite)
			oBlockRef.ScaleFactors = New Autodesk.AutoCAD.Geometry.Scale3d(dScale)
		Next
	End Sub
	Public Shared Function GetLinePoints(iMarkBlockType As enMarkBlockType) As TPlnPoint()   '               Autodesk.AutoCAD.Geometry.Point2d()
		Dim dSegment As Double = Math.Sin(Math.PI / 4.0)
		Dim taResPoints(3) As TPlnPoint
		taResPoints(0) = New TPlnPoint(dSegment, dSegment)
		taResPoints(1) = New TPlnPoint(dSegment, -dSegment)
		taResPoints(2) = New TPlnPoint(-dSegment, -dSegment)
		taResPoints(3) = New TPlnPoint(-dSegment, dSegment)

		Return taResPoints
	End Function
	Private Function zzGetMarkBlockName() As String
		Select Case miMarkBlockType
			Case enMarkBlockType.Circle
				Return MarkBlockNames(0)
			Case enMarkBlockType.Triangle
				Return MarkBlockNames(1)
			Case Else
				Return String.Empty
		End Select
	End Function

	Private Shared Sub zzEraseReferences(sBlockName As String)
		Dim colBlockRefObjIds As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetAllBlockRefs(sBlockName)
		If colBlockRefObjIds IsNot Nothing AndAlso colBlockRefObjIds.Count > 0 Then
			DMAcadExt.AcadTransaction.EraseDBObjects(colBlockRefObjIds)
		End If



	End Sub
End Class