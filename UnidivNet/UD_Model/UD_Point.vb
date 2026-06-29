Option Explicit On
Option Strict On
Public Class UD_Point
   Implements TopoManager.TopoScheme.INodeProperty

	Public Shared OldHanit As Boolean = True

	Public Const ShiftX As Decimal = 1000000000000D
   Public Const miRoundDigit = 3
   Private Shared mdcCenterX As Decimal
   Private Shared mdcCenterY As Decimal
	Private mtAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId

	Private msName As String
	Private mbIsOriginalName As Boolean
	Private msLayer As String

   Private msAcadName As String
   Private msBlockName As String


   Private miNumber As Integer
   Private mdcFixX As Decimal
   Private mdcFixY As Decimal
   Private mdcFixZ As Decimal
   Private mdX As Double
   Private mdY As Double
   Private mdZ As Double

   Private mtScaleFactors As Autodesk.AutoCAD.Geometry.Scale3d = New Autodesk.AutoCAD.Geometry.Scale3d(1.0)
   Private miCCode As Integer
   Private miSCode As Integer
   Private miMCode As Integer
   Private miDrMod As Integer
   Private miStage As Integer = -1
   Private miAction As Integer = 0

	Private mbIsOld As Boolean
	Private mbIsUserBlocking As Boolean

	Private mbIsScipped As Boolean
	Private mbIsRepeating As Boolean
	'  Private mbOriginalName As Boolean
	Private miFragmentID As Integer

   Private Shared moNewPointBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock("C1611", "")
	Private Shared moOldPointBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(UnidivNet.UD_App.OldPointBlockName)
	Private Shared moHorizontalPointBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(UnidivNet.UD_App.OldHorizontalPointBlockName)
	Private Shared moOldVerticalPointBlock As DMAcadExt.AcadBlock = New DMAcadExt.AcadBlock(UnidivNet.UD_App.OldVerticalPointBlockName)




	Private Shared msaPointFields() As String = {"POINT_NAME", "UID", "MARK", "TOPO", "SOURCE", "CLASS", "HEIGHT"}
	Private Shared msaPointDfltValuesFields() As String = {"", "", "15", "", "5", "3", ""}


	'Public Shared Function CoordToKeyAAA(dcX As Decimal, dcY As Decimal) As Decimal
	'   Return (dcX - mdcCenterX) * ShiftX + (dcY - mdcCenterY)
	'End Function
	'Public Shared Function CoordToKeyAAA(dX As Double, dY As Double) As Decimal
	'   dX = Math.Round(dX, miRoundDigit)
	'   dY = Math.Round(dY, miRoundDigit)
	'   Return CoordToKey(Convert.ToDecimal(dX), Convert.ToDecimal(dY))
	'End Function
	Public Shared Sub Init()
      moNewPointBlock.Fields = New String() {"POINT_NAME", "UID", "MARK", "TOPO", "SOURCE", "CLASS", "HEIGHT"}
      moOldPointBlock.Fields = New String() {"POINT_NAME"}
		moOldPointBlock.OpenForRead()
		moHorizontalPointBlock.Fields = New String() {"POINT_NAME"}
		moHorizontalPointBlock.OpenForRead()
		moOldVerticalPointBlock.Fields = New String() {"POINT_NAME"}
		moOldVerticalPointBlock.OpenForRead()

	End Sub
	Public Sub New(ByVal oDataReader As System.Data.Common.DbDataReader)
		If oDataReader IsNot Nothing Then
			msName = oDataReader.GetString(0)
			msAcadName = msName
			miNumber = Convert.ToInt32(oDataReader.GetInt16(1))
			mdcFixX = oDataReader.GetDecimal(2)
			mdcFixY = oDataReader.GetDecimal(3)
			mdcFixZ = oDataReader.GetDecimal(4)

			miCCode = Convert.ToInt32(oDataReader.GetInt16(5)) 'CCode
			miSCode = Convert.ToInt32(oDataReader.GetInt16(6)) 'SCode
			miMCode = Convert.ToInt32(oDataReader.GetInt16(7)) 'MCode
			miDrMod = Convert.ToInt32(oDataReader.GetInt16(9)) 'DrMod

			mdX = Math.Round(oDataReader.GetDouble(10), 6, MidpointRounding.AwayFromZero)
			mdY = Math.Round(oDataReader.GetDouble(11), 6, MidpointRounding.AwayFromZero)
			mdZ = Math.Round(oDataReader.GetDouble(12), 6, MidpointRounding.AwayFromZero)
		End If

	End Sub
	Public Sub New(ByVal sName As String)
		msName = sName
	End Sub
	Public Sub New(ByVal oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference)
      Dim tBlockRefData As DMAcadExt.BlockRefData
		If OldHanit Then
			Select Case oBlockRef.Name
				Case UD_App.OldPointBlockName
					miStage = 0
				Case Else
					miStage = -1
			End Select
		End If

		mtAcObjID = oBlockRef.ObjectId
      msBlockName = oBlockRef.Name
      msLayer = oBlockRef.Layer
		Select Case msBlockName
			Case UnidivNet.UD_App.OldPointBlockName
				moOldPointBlock.OpenForRead()
				tBlockRefData = moOldPointBlock.GetBlockRefData(oBlockRef)
				msName = tBlockRefData.AttribValues(0)
				mbIsOld = True
			Case UnidivNet.UD_App.OldHorizontalPointBlockName
				moHorizontalPointBlock.OpenForRead()
				tBlockRefData = moHorizontalPointBlock.GetBlockRefData(oBlockRef)
				msName = tBlockRefData.AttribValues(0)
				mbIsOld = True
			Case UnidivNet.UD_App.OldVerticalPointBlockName

				moOldVerticalPointBlock.OpenForRead()
				tBlockRefData = moOldVerticalPointBlock.GetBlockRefData(oBlockRef)
				msName = tBlockRefData.AttribValues(0)
				mbIsOld = True
				'  DMCommon.Debug.MsgBox("09_687", msName)
			Case "C1611"   'UPD_TAZAR
				moOldPointBlock.OpenForRead()
				tBlockRefData = moOldPointBlock.GetBlockRefData(oBlockRef)
				msName = tBlockRefData.AttribValues(0)
				mbIsOld = False
		End Select
		mbIsOriginalName = Not String.IsNullOrEmpty(msName)



		mdX = Math.Round(oBlockRef.Position.X, 6, MidpointRounding.AwayFromZero)
      mdY = Math.Round(oBlockRef.Position.Y, 6, MidpointRounding.AwayFromZero)
      mdZ = Math.Round(oBlockRef.Position.Z, 6, MidpointRounding.AwayFromZero)

      '  DMCommon.Debug.MsgBox("09_671", True, mtAcObjID, msBlockName, miStage)
   End Sub

   Public ReadOnly Property PointKey As ULong
      Get

         Return DMAcadExt.TplnPointKeyLong.CoordToKey(mdX, mdY)
      End Get
   End Property
   Public ReadOnly Property Point As DMAcadExt.TPlnPoint
      Get
         Return New DMAcadExt.TPlnPoint(mdX, mdY)
      End Get
   End Property


	Public Property Stage As Integer Implements TopoManager.TopoScheme.INodeProperty.Stage
      Get
         Return miStage
      End Get
      Set(iValue As Integer)
         miStage = iValue
      End Set
   End Property
   Public Property Action As Integer
      Get
         Return miAction
      End Get
      Set(iValue As Integer)
         miAction = iValue
      End Set
   End Property
   Public Property FragmentID As Integer
      Get
         Return miFragmentID
      End Get
      Set(iValue As Integer)
         miFragmentID = iValue
      End Set
   End Property


   Public ReadOnly Property SCode As Integer
      Get
         Return miSCode
      End Get

   End Property

   Public ReadOnly Property MCode As Integer
      Get
         Return miMCode
      End Get

   End Property

   Public ReadOnly Property CCode As Integer
      Get
         Return miCCode
      End Get

   End Property


   Public Property ScaleFactors As Autodesk.AutoCAD.Geometry.Scale3d
      Get
         Return mtScaleFactors
      End Get
      Set(tValue As Autodesk.AutoCAD.Geometry.Scale3d)
         mtScaleFactors = tValue
      End Set
   End Property

   Public Property AcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId
      Get
         Return mtAcObjID
      End Get
      Set(tValue As Autodesk.AutoCAD.DatabaseServices.ObjectId)
         mtAcObjID = tValue
      End Set
   End Property
	Public ReadOnly Property IsOriginalName As Boolean
		Get
			Return mbIsOriginalName
		End Get
	End Property
	Public ReadOnly Property IsOld As Boolean
		Get
			If OldHanit Then
				Select Case miCCode
					Case 1, 11, 12, 13, 14, 15
						Return True
					Case Else
						Return False
				End Select
			Else
				Return mbIsOld
			End If
		End Get
	End Property
	Public ReadOnly Property IsMustPoint As Boolean
		Get
			Return Me.IsOld OrElse Me.IsUserBlocking
		End Get
	End Property
	Public ReadOnly Property HasName As Boolean
		Get
			Return Not String.IsNullOrEmpty(msName)
		End Get
	End Property

	Public Property IsUserBlocking As Boolean
		Get
			Return mbIsUserBlocking OrElse TopoManager.TopoScheme.tsNode.IsGeoVertex
		End Get
		Set(bValue As Boolean)
			mbIsUserBlocking = bValue
		End Set
	End Property
	Public Property IsRepeating As Boolean
		Get
			Return mbIsRepeating
		End Get
		Set(bValue As Boolean)
			mbIsRepeating = bValue
		End Set
	End Property
	Public Property IsScipped As Boolean
		Get
			Return mbIsScipped
		End Get
		Set(bValue As Boolean)
			mbIsScipped = bValue
		End Set
	End Property

	Public ReadOnly Property Coordinates As String
		Get
			Return mdX.ToString() & "," & mdY.ToString()
		End Get
	End Property
	Public Sub UpdateByAcad(sText As String)
		msAcadName = sText
		Dim oHebText As DMCommon.HebrewTrans = New DMCommon.HebrewTrans(msAcadName, False)
		msName = oHebText.GetWinDest(False)
	End Sub
	Public Sub CheckBlock(sLayer As String)
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)
		Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
		If msBlockName <> moOldPointBlock.BlockName Then
			' DMCommon.Debug.MsgBox("!!Old Point Block", msBlockName, moOldPointBlock.BlockName)
		End If
		If msLayer <> UnidivNet.UD_App.GetStageUDPointLayer(0, True) Then
			'  DMCommon.Debug.MsgBox("!!Old Point Layer", msLayer)
		End If
	End Sub
	Public Sub CancelName()
		If Not mbIsOriginalName Then
			msName = String.Empty
		End If
	End Sub
	Public Sub Insert(tInsertPoint As Autodesk.AutoCAD.Geometry.Point3d, sLayer As String)

		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)
		Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
		'  tBlockRefData.Position = Me.Point.AcGePoint3d

		tBlockRefData.Layer = sLayer
		tBlockRefData.ScaleFactors = DMAcadExt.AcadDocument.GetDWGScale()
		tBlockRefData.Position = tInsertPoint

		dicAttribValues.Add(msaPointFields(0), DMCommon.Functions.CStrN(msName))
		dicAttribValues.Add(msaPointFields(2), msaPointDfltValuesFields(2))
		dicAttribValues.Add(msaPointFields(4), msaPointDfltValuesFields(4))
		dicAttribValues.Add(msaPointFields(5), msaPointDfltValuesFields(5))
		tBlockRefData.AtribValuesDic = dicAttribValues
		moOldPointBlock.OpenForRight()
		moOldPointBlock.InsertRefNew(tBlockRefData)
	End Sub
	Public Sub UpdateBlock(sLayer As String)
		Dim dicAttribValues As Dictionary(Of String, String) = New Dictionary(Of String, String)

		Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
      '  tBlockRefData.Position = Me.Point.AcGePoint3d

      tBlockRefData.Layer = sLayer
      tBlockRefData.ScaleFactors = DMAcadExt.AcadDocument.GetDWGScale()


		dicAttribValues.Add(msaPointFields(0), DMCommon.Functions.CStrN(msName))
		dicAttribValues.Add(msaPointFields(2), msaPointDfltValuesFields(2))
		dicAttribValues.Add(msaPointFields(4), msaPointDfltValuesFields(4))
		'   Cancelled 10/02/2020  	dicAttribValues.Add(msaPointFields(5), msaPointDfltValuesFields(5))
		tBlockRefData.AtribValuesDic = dicAttribValues
		'  DMCommon.Debug.MsgBox("09_660", tBlockRefData.Position)

		moNewPointBlock.UpdateAttribData(mtAcObjID, dicAttribValues)
      moNewPointBlock.UpdateBlockRef(mtAcObjID, tBlockRefData)

   End Sub
   Public Property Name As String
      Get

         Return msName
      End Get
      Set(sValue As String)
         msName = sValue
      End Set
   End Property
   Public Property AcadName As String
      Get
         Return msAcadName
      End Get
      Set(sValue As String)
         msAcadName = sValue
      End Set
   End Property
   Public Function GetHanitData() As DMAcadExt.BlockRefData
      Dim tBlockRefData As DMAcadExt.BlockRefData = New DMAcadExt.BlockRefData()
      tBlockRefData.Position = New Autodesk.AutoCAD.Geometry.Point3d(mdX, mdY, mdZ)
      tBlockRefData.ScaleFactors = DMAcadExt.AcadDocument.GetDWGScale()
      '{"POINT_NAME", "UID", "MARK", "TOPO", "SOURCE", "CLASS", "HEIGHT"}
      tBlockRefData.AttribValues = {DMCommon.Functions.CStrN(msAcadName, "nothing"), "", miMCode.ToString(), "", miSCode.ToString(), "", FormatNumber(mdcFixZ, 3, TriState.True, TriState.False, TriState.False)}
      Return tBlockRefData
   End Function
   Public Overrides Function ToString() As String
      Return msName & "," & CStr(miStage) & "," & New Autodesk.AutoCAD.Geometry.Point2d(mdX, mdY).ToString()
   End Function

	Protected Overrides Sub Finalize()
		MyBase.Finalize()
	End Sub

	Public Property IsMust As Boolean Implements TopoManager.TopoScheme.INodeProperty.IsMust
		Get
			Return (Stage <> -1)
		End Get
		Set(bValue As Boolean)

		End Set
	End Property



End Class
