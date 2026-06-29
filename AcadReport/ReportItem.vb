Option Explicit On
Option Strict On
Public Enum enReportOptions
	[Default] = 1
	AllAreaOptionsEnabled = 2
	AcadAreaEnabled = 4
	CalcMergeAreaEnabled = 8
	CalcMergeArea2Enabled = 16
	RoundedAreaEnabled = 32
	OverlayEnabled = 256
   CalcMergeArea2Dflt = 512
End Enum
Public Enum enReportModifications
	[Default] = 1
	Multi = 2
	Additional = 3
	Template = 4
End Enum

Public Class ReportItem
   Inherits DMCommon.ItemData
   Private miOptions As enReportOptions
   Private miTopoPurpose As DMAcadExt.enTopoPurpose
   Private miReportModification As enReportModifications
	Private mbUseOverlay As Boolean
	Private mbExcel As Boolean
	Private mbAcad As Boolean
	Private mbAcadTable As Boolean
	'Private enReportOptions

    Public Sub New(ByVal iListIndex As TPlServerDB.enResourceTheme, ByVal sListDispData As String, ByVal iOptions As enReportOptions, ByVal iTopoPurpose As DMAcadExt.enTopoPurpose, iReportModification As enReportModifications, ByVal bExcel As Boolean, ByVal bAcad As Boolean, ByVal bAcadTable As Boolean)
        MyBase.New(iListIndex, sListDispData)
        miOptions = iOptions
        miTopoPurpose = iTopoPurpose
        miReportModification = iReportModification
        mbExcel = bExcel
        mbAcad = bAcad
        mbAcadTable = bAcadTable
    End Sub
	Public Sub New(ByVal iListIndex As TPlServerDB.enResourceTheme, ByVal sListDispData As String, ByVal iOptions As enReportOptions, ByVal bExcel As Boolean, ByVal bAcad As Boolean, ByVal bAcadTable As Boolean)
		MyBase.New(iListIndex, sListDispData)
		miOptions = iOptions


		mbExcel = bExcel
		mbAcad = bAcad
		mbAcadTable = bAcadTable
	End Sub
	Public Property RepIndex() As TPlServerDB.enResourceTheme
		Get
			Try
				'	DMCommon.Debug.MsgBox("13_124z", MyBase.ListIndex, CType(MyBase.ListIndex, Integer), TPlServerDB.enResourceTheme.AcRepOwnership)
				Return CType(MyBase.ListIndex, TPlServerDB.enResourceTheme)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "frmTopoActionsBase - RepIndex")
			End Try
		End Get
		Set(ByVal iValue As TPlServerDB.enResourceTheme)
			MyBase.ListIndex = iValue
		End Set
	End Property
	Public Property Options() As enReportOptions
		Get
			Return miOptions
		End Get
		Set(ByVal iValue As enReportOptions)
			miOptions = iValue
		End Set
	End Property
	Public Property TopoPurpose() As DMAcadExt.enTopoPurpose
		Get
			Return miTopoPurpose
		End Get
		Set(ByVal iValue As DMAcadExt.enTopoPurpose)
			miTopoPurpose = iValue
		End Set
	End Property
	Public Property ReportModification() As enReportModifications
        Get
            Return miReportModification
        End Get
        Set(ByVal iValue As enReportModifications)
            miReportModification = iValue
        End Set
    End Property

	Public Property Excel() As Boolean
		Get
			Return mbExcel
		End Get
		Set(ByVal bValue As Boolean)
			mbExcel = bValue
		End Set
	End Property
	Public Property Acad() As Boolean
		Get
			Return mbAcad
		End Get
		Set(ByVal bValue As Boolean)
			mbAcad = bValue
		End Set
	End Property
	Public Property AcadTable() As Boolean
		Get
			Return mbAcadTable
		End Get
		Set(ByVal bValue As Boolean)
			mbAcadTable = bValue
		End Set
	End Property
	Public ReadOnly Property AcadBlock() As Boolean
		Get
			Return mbAcad AndAlso (Not mbAcadTable)
		End Get
	End Property
	Public Property UseOverlay() As Boolean
		Get
			Return mbUseOverlay
		End Get
		Set(ByVal bValue As Boolean)
			mbUseOverlay = bValue
		End Set
	End Property


End Class

