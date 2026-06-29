Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Public MustInherit Class TplnXDataBasePgon
	Inherits TplnXData

	Protected Enum enBaseMembers
		ID
		DataID
		MapTheme
		Name
		AcadArea
		Perimeter
		NeigborList
		NeigborVertexList
		Exterior
		InteriorList
		InteriorCount
		ReservedA
		ReservedB
		Count
	End Enum
	Public Sub New(oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer, sXDataAppName As String, Optional bTest As Boolean = False)
		MyBase.New(oResBuffer, sXDataAppName, bTest)
	End Sub
	Public Sub New(iCount As Integer, sXDataAppName As String)
		MyBase.New(iCount, sXDataAppName)
	End Sub
	Public Property ID As Integer
		Get
			Dim iRes As Integer = 0
			MyBase.GetInt(enBaseMembers.ID, iRes)
			Return iRes
		End Get
		Set(iValue As Integer)
			MyBase.SetInt(enBaseMembers.ID, iValue)
		End Set
	End Property

	Public Property DataID As Integer
		Get
			Dim iRes As Integer = 0
			MyBase.GetInt(enBaseMembers.DataID, iRes)
			Return iRes
		End Get
		Set(iValue As Integer)
			MyBase.SetInt(enBaseMembers.DataID, iValue)
		End Set
	End Property
	Public Property MapTheme As DMAcadExt.enMapTheme
		Get
			Dim iValue As Integer = 0
			MyBase.GetInt(enBaseMembers.MapTheme, iValue)
			If [Enum].IsDefined(GetType(enMapTheme), iValue) Then
				Return CType(iValue, enMapTheme)
			Else
				Return enMapTheme.Undefined
			End If
		End Get
		Set(iValue As DMAcadExt.enMapTheme)
			MyBase.SetInt(enBaseMembers.MapTheme, iValue)
		End Set
	End Property

	Public Property Name As String
		Get
			Dim sRes As String = Nothing
			MyBase.GetStr(enBaseMembers.Name, sRes)
			Return sRes
		End Get
		Set(sValue As String)
			MyBase.SetStr(enBaseMembers.Name, sValue)
		End Set
	End Property


	Public Property AcadArea As Double
		Get
			Dim dRes As Double = 0.0
			MyBase.GetDbl(enBaseMembers.AcadArea, dRes)
			Return dRes
		End Get
		Set(dValue As Double)
			MyBase.SetDbl(enBaseMembers.AcadArea, dValue)
		End Set
	End Property
	Public Property Perimeter As Double
		Get
			Dim dRes As Double = 0.0
			MyBase.GetDbl(enBaseMembers.Perimeter, dRes)
			Return dRes
		End Get
		Set(dValue As Double)
			MyBase.SetDbl(enBaseMembers.Perimeter, dValue)
		End Set
	End Property
	Public Property NeigborList As String
		Get
			Dim sRes As String = Nothing
			MyBase.GetStr(enBaseMembers.NeigborList, sRes)
			Return sRes
		End Get
		Set(sValue As String)
			MyBase.SetStr(enBaseMembers.NeigborList, sValue)
		End Set
	End Property
	Public Property Exterior As Integer
		Get
			Dim iRes As Integer = 0
			MyBase.GetInt(enBaseMembers.Exterior, iRes)
			Return iRes
		End Get
		Set(iValue As Integer)
			MyBase.SetInt(enBaseMembers.Exterior, iValue)
		End Set
	End Property

	Public Property InteriorList As String
		Get
			Dim sRes As String = Nothing
			MyBase.GetStr(enBaseMembers.InteriorList, sRes)
			Return sRes
		End Get
		Set(sValue As String)
			MyBase.SetStr(enBaseMembers.InteriorList, sValue)
		End Set
	End Property
	Public Property InteriorCount As Integer
		Get
			Dim iRes As Integer = 0
			MyBase.GetInt(enBaseMembers.InteriorCount, iRes)
			Return iRes
		End Get
		Set(iValue As Integer)
			MyBase.SetInt(enBaseMembers.InteriorCount, iValue)
		End Set
	End Property
	Protected Sub StartPrintList()
		'NeigborVertexList()
		DMAcadExt.AcadDocument.WriteMessage("ID = " & Me.ID.ToString())
		DMAcadExt.AcadDocument.WriteMessage("DataID = " & Me.DataID.ToString())
		DMAcadExt.AcadDocument.WriteMessage("MapTheme = " & Me.MapTheme.ToString())
		DMAcadExt.AcadDocument.WriteMessage("Name = " & Me.Name)
		DMAcadExt.AcadDocument.WriteMessage("AcadArea = " & Me.AcadArea.ToString())
		DMAcadExt.AcadDocument.WriteMessage("Perimeter = " & Me.Perimeter.ToString())
		DMAcadExt.AcadDocument.WriteMessage("NeigborList = " & Me.NeigborList)
		DMAcadExt.AcadDocument.WriteMessage("Exterior = " & Me.Exterior.ToString())
		DMAcadExt.AcadDocument.WriteMessage("InteriorList = " & Me.InteriorList)
		DMAcadExt.AcadDocument.WriteMessage("InteriorCount = " & Me.InteriorCount.ToString())



	End Sub
	Public MustOverride Sub PrintList()

End Class
