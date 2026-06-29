Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map.ObjectData
Imports Autodesk.Gis.Map.Utilities
Namespace TPlanGraph

   ''' <summary>
   ''' 12/08/2007 
   ''' 
   ''' </summary>
   ''' <remarks></remarks>
   ''' 
   Public Structure OverlayOD
      Dim NewID As Integer
      Dim SourceID As Integer
      Dim OverlayID As Integer
      Dim SourceAreaPcnt As Double
      Dim OverlayAreaPcnt As Double
   End Structure

   Public Class TplnOverlayODRecordset
      Const msNameDel As String = "_"
      Const msODOverlayTablePrefix As String = "UNION"
      Const msIDODFldName As String = "TNEW_ID"
      Const msIDSuffix As String = "ID"
      Const msPercentAreaSuffix As String = "PERCENTAREA"

      Private msTopoSourceName As String
      Private msTopoOverlayName As String
      Private msODTableName As String
      Private msODTableDescription As String
      Private moODTable As Table
      Private moFieldDefinitions As FieldDefinitions
      Public Sub New(ByVal sTopoSourceName As String, ByVal sTopoOverlayName As String, ByVal sODTableName As String)
         msTopoSourceName = sTopoSourceName
         msTopoOverlayName = sTopoOverlayName
         msODTableName = sODTableName
         moODTable = ODEditor.GetODTable(sODTableName)
         moFieldDefinitions = moODTable.FieldDefinitions
      End Sub
      Private Sub zzInitDefAAA()

         Dim oFieldDef As FieldDefinition
         For iFieldIndex As Integer = 0 To moFieldDefinitions.Count - 1
            oFieldDef = moFieldDefinitions.Item(iFieldIndex)
         Next

         'oMapValue = oODRecord.Item(iFieldIndex)
      End Sub
		Public Function GetOverlayOD(ByVal tAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId) As OverlayOD
			Dim oFieldDef As FieldDefinition
			Dim tOverlayOD As OverlayOD
			Dim oODRecord As Autodesk.Gis.Map.ObjectData.Record = ODEditor.GetODRecord(tAcObjID, msODTableName)
			Dim sTest As String
			Dim iTestIndex As Integer
			Static Dim iTest As Integer

			If oODRecord IsNot Nothing AndAlso moFieldDefinitions IsNot Nothing Then
				iTest += 1
				sTest = "x"
				Try
					Dim oMapValue As MapValue
					sTest = "a"
					For iFieldIndex As Integer = 0 To oODRecord.Count - 1
						iTestIndex = iFieldIndex
						sTest = "b"
						oFieldDef = moFieldDefinitions.Item(iFieldIndex)
						sTest = "c"
						oMapValue = oODRecord.Item(iFieldIndex)
						sTest = "d"
						Select Case oFieldDef.Name
							Case msIDODFldName
								sTest = "e"
								tOverlayOD.NewID = oMapValue.Int32Value
							Case msTopoSourceName & msNameDel & msIDSuffix
								sTest = "f"
								tOverlayOD.SourceID = oMapValue.Int32Value
							Case msTopoOverlayName & msNameDel & msIDSuffix
								sTest = "g"
								tOverlayOD.OverlayID = oMapValue.Int32Value
							Case msTopoSourceName & msNameDel & msPercentAreaSuffix
								sTest = "h"
								tOverlayOD.SourceAreaPcnt = oMapValue.DoubleValue
							Case msTopoOverlayName & msNameDel & msPercentAreaSuffix
								sTest = "k"
								tOverlayOD.OverlayAreaPcnt = oMapValue.DoubleValue
						End Select
						sTest = "l"
					Next
					sTest = "m"

				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(moFieldDefinitions.Count) & ":" & CStr(iTestIndex) & ":" & oODRecord.TableName & vbCrLf & CStr(iTest) & ":" & sTest, "38_210")
				End Try
				oODRecord.Dispose()
			Else
				If oODRecord Is Nothing Then
					System.Windows.Forms.MessageBox.Show("3377", "GetOverlayODRecord-oODRecord Is Nothing")
				Else
					System.Windows.Forms.MessageBox.Show("3378", "GetOverlayODRecord-oODRecord Is Nothing")
				End If
			End If
			Return tOverlayOD

		End Function
	End Class
  

End Namespace

