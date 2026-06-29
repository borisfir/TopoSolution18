Option Explicit On
Option Strict On

Public Class FragmentPolygon
	Inherits TopoManager.TPlanGraph.TplnTopoPgon

	Private Shared moAcadBlockDef As DMAcadExt.AcadBlockDef
	Private mdgaParseAttribute() As DMAcadExt.AcadBlockDef.ParseAttribute = {New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetParcelName), New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetBlockNo), New DMAcadExt.AcadBlockDef.ParseAttribute(AddressOf zzGetLegalArea)}
	Private miBlockNo As Integer
	Private msName As String
	Private miOrder As Integer
	Private mdLegalArea As Double = 0.0
	Public Sub New(ByVal oPolygon As Autodesk.Gis.Map.Topology.Polygon)
		MyBase.New(oPolygon, True)
      MyBase.InputAttributeData(moAcadBlockDef, mdgaParseAttribute, True)


	End Sub
	Public Shared Sub Initialize()
		moAcadBlockDef = New DMAcadExt.AcadBlockDef(DMAcadExt.enAcadBlocks.UD_Parcel)

	End Sub
	Public ReadOnly Property Name() As String
		Get
			Return msName
		End Get

	End Property
	Private Sub zzGetParcelName(ByVal sAttribValue As String)
		Dim oComplexNum As TopoManager.NumerationPair.ComplexNum
		msName = sAttribValue.Trim()
		If msName.Length <> 0 Then
			Try
				oComplexNum = New TopoManager.NumerationPair.ComplexNum(msName)
				miOrder = oComplexNum.Order
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, " zzGetParcelName")
			End Try
		End If
	End Sub
	Private Sub zzGetBlockNo(ByVal sAttribValue As String)
		Try
			If sAttribValue.Length <> 0 Then
				miBlockNo = TopoManager.Common.NumberFilter(sAttribValue)
			End If

		Catch oEx As Exception
			DMAcadExt.AcadDocument.WriteMessage(oEx.Message, "01_808")
		End Try
	End Sub
	Private Sub zzGetLegalArea(ByVal sAttribValue As String)
		Try
			If sAttribValue.Length <> 0 Then
				mdLegalArea = Convert.ToDouble(sAttribValue)
			End If

		Catch oEx As Exception
			DMAcadExt.AcadDocument.WriteMessage(oEx.Message, "01_809")
		End Try
	End Sub
	Public Overrides Sub Terminate()

	End Sub

   Protected Overrides ReadOnly Property BlockAttribIndex As Integer()
      Get
         Return Nothing
      End Get

   End Property
   Protected Overrides ReadOnly Property AddBlockAttribIndex As Integer()
      Get
         Return Nothing
      End Get
   End Property

End Class
