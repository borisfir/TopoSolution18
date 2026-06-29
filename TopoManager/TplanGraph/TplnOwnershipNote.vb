Option Explicit On
Option Strict On
Imports System.Data
Namespace TPlanGraph



	Public Class TplnOwnershipNote
		Inherits TPlanGraph.TplnTopoPgon

		Private Const msParagraph19AttribTag As String = "Paragraph19"
		Private Const msLeasingAttribTag As String = "Leasing"

		Private Shared mtOwnershipNoteMapThemeData As DMAcadExt.MapThemeData
		Private Shared msCentroidBlockName As String

		Private Shared miaBlockAttribIndex(1) As Integer
		Private mtOwnershipNoteData As OwnershipNoteData

		Protected Overrides ReadOnly Property BlockAttribIndex As Integer()
			Get
				Return miaBlockAttribIndex
			End Get
		End Property

		Protected Overrides ReadOnly Property AddBlockAttribIndex As Integer()
			Get
				Return Nothing
			End Get
		End Property

		Public Sub New(ByRef oPolygon As Autodesk.Gis.Map.Topology.Polygon)
			MyBase.New(oPolygon, True)

			'	DMCommon.ExcelLog.SetNextValue(11, iTopoPurpose, iTopoPurpose.ToString())

			MyBase.SetAttributeOrder()



			mtOwnershipNoteData = New OwnershipNoteData(dsaBlockAttribText)



		End Sub
		Public Shared Function GetTopoName() As String
			Return mtOwnershipNoteMapThemeData.TopoName
		End Function
		Public Shared Function GetMapLayer() As String
			Return mtOwnershipNoteMapThemeData.MapLayer
		End Function

		Private Structure OwnershipNoteData
			Dim Paragraph19 As Boolean
			Dim Leasing As Boolean
			Dim Exists As Boolean

			Public Sub New(saValues() As String)

				If saValues IsNot Nothing Then

					Dim iAttribUB As Integer = -1
					Try

						iAttribUB = saValues.GetUpperBound(0)

					Catch oEx As System.Exception
						TplnProject.WriteMessageBox(oEx.Message, "OwnershipNoteData - New_1")
					End Try
					If iAttribUB >= 0 Then
						'   DMCommon.ExcelLogC.SetNextValue(iRow, 0, iAttribUB)
						'   DMCommon.ExcelLogC.SetArray(saValues, 4)

						Try
							Paragraph19 = zzAttribTextToBool(saValues(0))

						Catch oEx As System.Exception
							TplnProject.WriteMessageBox(oEx.Message, "OwnershipNoteData - New_2")
						End Try
					End If
					If iAttribUB >= 1 Then
						Try
							Leasing = zzAttribTextToBool(saValues(1))
						Catch oEx As System.Exception
							TplnProject.WriteMessageBox(oEx.Message, "OwnershipNoteData - New_3")
						End Try
					End If




					Exists = True
				End If

			End Sub
			Private Function zzAttribTextToBool(sText As String) As Boolean
				Select Case sText.Trim
					Case "Y", "y", "1"
						Return True
					Case "N", "n", "0"
						Return False
					Case Else
						Return False
				End Select

			End Function
		End Structure
		Public Shared Sub Initialize(tOwnershipNoteMapThemeData As DMAcadExt.MapThemeData)
			Try
				mtOwnershipNoteMapThemeData = tOwnershipNoteMapThemeData
				msCentroidBlockName = mtOwnershipNoteMapThemeData.CentroidBlock

				Dim saBlockAttribTag() As String = DMAcadExt.AcadTransaction.GetAttribDef(msCentroidBlockName, True)

				If saBlockAttribTag IsNot Nothing Then
					'DMCommon.Functions.DispArray(saBlockAttribTag, "01_599Parcel", True)
					For iAttribIndex As Integer = 0 To saBlockAttribTag.GetUpperBound(0)
						Select Case saBlockAttribTag(iAttribIndex)
							Case msParagraph19AttribTag
								miaBlockAttribIndex(0) = iAttribIndex
							Case msLeasingAttribTag
								miaBlockAttribIndex(1) = iAttribIndex

						End Select
					Next

					'  MessageBox.Show("Definition of '" & msCentroidBlockName & "' was not found", "11_180")
				End If


				'ParcelData.MapThemeID = tParcelMapThemeData.MapThemeID
				'ParcelData.BlockName = tParcelMapThemeData.CentroidBlock

				'	mlstMissingLegalAreaPoints = New List(Of DMAcadExt.TPlnPoint)
				'07/08/07 mdicSumLanduses = New TplnLanduses(enTopoPurpose.Parcel)
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - Initialize")
			End Try
		End Sub
		Public Shared Function GetLineTopoName() As String
			Return mtOwnershipNoteMapThemeData.LineTopoName
		End Function
		Public ReadOnly Property Paragraph19 As Boolean
			Get
				Return mtOwnershipNoteData.Paragraph19
			End Get
		End Property
		Public ReadOnly Property Leasing As Boolean
			Get
				Return mtOwnershipNoteData.Leasing
			End Get
		End Property
		Public ReadOnly Property Paragraph19Area As Double
			Get
				If mtOwnershipNoteData.Paragraph19 Then
					Return MyBase.AcadArea(False)
				Else
					Return 0.0
				End If
			End Get
		End Property
		Public ReadOnly Property LeasingArea As Double
			Get
				If mtOwnershipNoteData.Leasing Then
					Return MyBase.AcadArea(False)
				Else
					Return 0.0
				End If
			End Get
		End Property
		Public ReadOnly Property OverlayArea As Double
			Get
				If mtOwnershipNoteData.Paragraph19 AndAlso mtOwnershipNoteData.Leasing Then
					Return MyBase.AcadArea(False)
				Else
					Return 0.0
				End If
			End Get
		End Property


		Public Overrides Sub Terminate()
			Throw New NotImplementedException()
		End Sub
	End Class
End Namespace
