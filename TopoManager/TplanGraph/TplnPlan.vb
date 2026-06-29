Option Explicit On
Option Strict On
Imports DMAcadExt
Imports Autodesk.Gis.Map.Topology
Imports System.Data
Namespace TPlanGraph
   Public Structure PlanData
      Dim Name As String
      Dim Exists As Boolean
      Dim IsError As Boolean
      Public Sub New(saValues() As String)

         If saValues IsNot Nothing Then
            Dim iAttribUB As Integer = -1
            Try
               If saValues IsNot Nothing Then
                  iAttribUB = saValues.GetUpperBound(0)
               End If
            Catch oEx As System.Exception
               TplnProject.WriteMessageBox(oEx.Message, "PlanData - New_1")
            End Try
            If iAttribUB >= 0 Then

               Try
                  Name = saValues(0).Trim()
						'DMCommon.Debug.MsgBox("!New PlanData", saValues(0), Name)

					Catch oEx As System.Exception
                  TplnProject.WriteMessageBox(oEx.Message, "PlanData - New_2")
               End Try
            End If




            Exists = True
         End If

      End Sub
   End Structure

   Public Class TplnPlan
      Inherits TPlanGraph.TplnTopoPgon

		Private Const msNameAttribTag As String = "KAYNO"
		Private Enum enPlanCentroidAttribIndices
         Name
         UB = Name
      End Enum

      Private Shared mtPlanMapThemeData As DMAcadExt.MapThemeData
      Private Shared msCentroidBlockName As String
      Private Shared msAddCentroidBlockName As String


      Private Shared miaBlockAttribIndex(enPlanCentroidAttribIndices.UB) As Integer
      Private mtPlanData As PlanData
      Public Shared Sub Initialize(tPlanMapThemeData As DMAcadExt.MapThemeData)
         Try
            'MapThemeData.
            '	System.Windows.Forms.MessageBox.Show(tApprMapThemeData.TopoName, "##TplnLot - Initialize")
            mtPlanMapThemeData = tPlanMapThemeData

            msCentroidBlockName = mtPlanMapThemeData.CentroidBlocks
            msAddCentroidBlockName = DMAcadExt.AcadBlock.GetAdditionalBlockName(msCentroidBlockName, 1)

            'System.Windows.Forms.MessageBox.Show(msCentroidApprovedBlockName & ":" & msCentroidProposedBlockName, "04_596")
            If Not String.IsNullOrEmpty(msCentroidBlockName) Then
               zzInitBlockAttribIndex(msCentroidBlockName, miaBlockAttribIndex)
            End If

            '   DMCommon.Functions.DispArray(miaApprovedBlockAttribIndex, "2miaApprovedBlockAttribIndex")
         Catch oEx As System.Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnLot - Initialize")
         End Try
      End Sub
      Public Shared Sub CreateMainDataTable()

      End Sub
		Public Shared Function GetTopoName() As String

			Return mtPlanMapThemeData.TopoName
		End Function
		Public Sub New(ByRef oPolygon As Autodesk.Gis.Map.Topology.Polygon)
         MyBase.New(oPolygon)
         ' 


         MyBase.SetAttributeOrder()   'miaBlockAttribIndex

			'DMCommon.Functions.DispArray(miaBlockAttribIndex, "iaBlockAttribIndex")
			'	DMCommon.Functions.DispArray(dsaBlockAttribText, "saBlockAttribValue", True)

			'  System.Windows.Forms.MessageBox.Show(MyBase.diCentroidAcObjID.ToString() & vbCrLf & oPolygon.Entity.ToString() & vbCrLf & mbPgonExists.ToString(), "04_422a")


			mtPlanData = New PlanData(dsaBlockAttribText)
         If mtPlanData.IsError Then
            DMAcadExt.AppMessages.AddMessage(True, Me.CentroidX, Me.CentroidX, "", "Attribute problem 1", False)
         End If

         '   System.Windows.Forms.MessageBox.Show(MyBase.diCentroidAcObjID.ToString() & vbCrLf & Me.BlockStatusStr & vbCrLf & Me.IsAnalytic.ToString, "04_422b")
      End Sub
      Public ReadOnly Property Name As String
         Get
            Return mtPlanData.Name
         End Get
      End Property
      Private Shared Sub zzInitBlockAttribIndex(ByVal sCentroidBlockName As String, ByRef iaBlockAttribIndex() As Integer)
         Try
            Dim saBlockAttribTag() As String = AcadTransaction.GetAttribDef(sCentroidBlockName, True)

				If saBlockAttribTag IsNot Nothing Then
					For iAttribIndex As Integer = 0 To saBlockAttribTag.GetUpperBound(0)

						Select Case Strings.UCase(saBlockAttribTag(iAttribIndex))
							Case msNameAttribTag
								miaBlockAttribIndex(enPlanCentroidAttribIndices.Name) = iAttribIndex

						End Select
						'DMCommon.Debug.MsgBox("!zzInitBlockAttribIndex", iAttribIndex, Strings.UCase(saBlockAttribTag(iAttribIndex)), miaBlockAttribIndex(enPlanCentroidAttribIndices.Name))
					Next
				End If

         Catch oEx As System.Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "TplnPlan - zzInitBlockAttribIndex")
         End Try
      End Sub
      Protected Overrides ReadOnly Property BlockAttribIndex As Integer()
         Get
            Return miaBlockAttribIndex
         End Get
         
      End Property

      Public Overrides Sub Terminate()

      End Sub

      Protected Overrides ReadOnly Property AddBlockAttribIndex As Integer()
         Get
            Return Nothing
         End Get
      End Property
   End Class
End Namespace