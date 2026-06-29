Option Explicit On
Option Strict On
Namespace TPlanGraph

   Public Enum enZoneType
      [Default]
   End Enum
   Public Structure ZoneData
      Dim ZoneID As Integer
      Dim ZoneName As String
      Dim ZoneType As enZoneType
      Dim Correct As Boolean
      Dim Exists As Boolean
      Public Sub New(saValues() As String)
         If saValues IsNot Nothing Then
            Dim iAttribUB As Integer = -1
            'DMCommon.Functions.DispArray(saValues, "03_954", True)
            Try
               iAttribUB = saValues.GetUpperBound(0)
            Catch oEx As System.Exception
               TplnProject.WriteMessageBox(oEx.Message, "TplnZone - New_1")
            End Try
            If iAttribUB >= 0 Then
               Try
                  Dim sZoneID As String = saValues(0).Trim()

                  '	ExproTypeID = [Enum].Parse(GetType(enExproType), sExproTypeID)
                  If Integer.TryParse(sZoneID, ZoneID) Then
                     Correct = True
                  Else
                     Correct = False
                  End If
               Catch oEx As System.Exception
                  Correct = False
                  TplnProject.WriteMessageBox(oEx.Message, "TplnZone - New_1")
               End Try

            End If



            If iAttribUB >= 1 Then
               Try
                  ZoneName = saValues(1).Trim()
               Catch oEx As System.Exception
                  TplnProject.WriteMessageBox(oEx.Message, "TplnZone - New_2")
               End Try
            End If

            If iAttribUB >= 2 Then
               Try
                  Dim sZoneType As String = saValues(2).Trim()
                  Dim iZoneType As Integer
                  '	ExproTypeID = [Enum].Parse(GetType(enExproType), sExproTypeID)
                  If Integer.TryParse(sZoneType, iZoneType) Then

                     If [Enum].IsDefined(GetType(enZoneType), iZoneType) Then
                        ZoneType = CType(iZoneType, enZoneType)
                        Correct = True
                     End If
                  End If


               Catch oEx As System.Exception
                  TplnProject.WriteMessageBox(oEx.Message, "TplnZone - New_3")
               End Try
            End If
            Exists = True
         Else
            System.Windows.Forms.MessageBox.Show("Data was not found" & vbCrLf & "", "TplnZone - 4")
         End If

      End Sub
   End Structure
   Public Class TplnZone
      Inherits TPlanGraph.TplnTopoPgon


      Public Const msZoneIDFieldName As String = "ZoneID"
      Public Const msZoneNameFieldName As String = "ZoneName"
      Public Const msZoneTypeFieldName As String = "ZoneType"

      Private Const msZoneIDAttribTag As String = "CODE"
      Private Const msZoneNameAttribTag As String = "NAME"
      Private Const msZoneTypeAttribTag As String = "TYPE"



      Private mtZoneData As ZoneData
      Private Shared moMainDataTable As System.Data.DataTable
      Private Shared mtZoneMapThemeData As DMAcadExt.MapThemeData
      Private Shared msCentroidBlockName As String
      Private Shared miaBlockAttribIndex(2) As Integer
      Private Shared moMainHiddenColumns As Dictionary(Of Integer, Integer)

      Public Shared Sub Initialize(tZoneMapThemeData As DMAcadExt.MapThemeData)
         Try
            mtZoneMapThemeData = tZoneMapThemeData
            msCentroidBlockName = mtZoneMapThemeData.CentroidBlocks
            '	MessageBox.Show(msCentroidBlockName, "11_179")
            Dim saBlockAttribTag() As String = DMAcadExt.AcadTransaction.GetAttribDef(msCentroidBlockName, True)
            If saBlockAttribTag IsNot Nothing Then
               'DMCommon.Functions.DispArray(saBlockAttribTag, "01_599d", True)
               For iAttribIndex As Integer = 0 To saBlockAttribTag.GetUpperBound(0)
                  Select Case saBlockAttribTag(iAttribIndex)
                     Case msZoneIDAttribTag
                        miaBlockAttribIndex(0) = iAttribIndex
                     Case msZoneNameAttribTag
                        miaBlockAttribIndex(1) = iAttribIndex
                     Case msZoneTypeAttribTag
                        miaBlockAttribIndex(2) = iAttribIndex
                  End Select
               Next

               '		DMCommon.Functions.DispArray(miaBlockAttribIndex, "01_577d")


               '	Erase saBlockAttribTag
            Else
               MessageBox.Show("saBlockAttribTag Is Nothing" & vbCrLf & msCentroidBlockName, "11_182")
            End If

            '07/08/07 mdicSumLanduses = New TplnLanduses(enTopoPurpose.Parcel)
         Catch oEx As System.Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "TplnParcel - Initialize")
         End Try
      End Sub
      Public Shared Sub CreateMainDataTable()
         moMainDataTable = New System.Data.DataTable("Zone")

         Dim dicMainHiddenColumns As Dictionary(Of Integer, Integer) = New Dictionary(Of Integer, Integer)
         moMainHiddenColumns = New Dictionary(Of Integer, Integer)


         With moMainDataTable.Columns

            .Add(msZoneIDFieldName, GetType(System.Int32))
            .Add(msZoneNameFieldName, GetType(System.String))
            .Add(msZoneTypeFieldName, GetType(System.Int32))


            .Add(TopoReader.msAreaFldName, GetType(System.Double))
            '	.Add(TopoReader.msSumPgonAreaUnFldName, GetType(System.Double))
            .Add(TopoReader.msCentroidXFldName, GetType(System.Double))          '12 8
            .Add(TopoReader.msCentroidYFldName, GetType(System.Double))          '13 9


            .Add(TopoReader.msPerimeterFldName, GetType(System.Double))          '15 11
            .Add(TopoReader.msTopoIDFldName, GetType(System.Int32))                 '16 12
            .Add(TopoReader.msAcObjIDFldName, GetType(Autodesk.AutoCAD.DatabaseServices.ObjectId))          '17 13
            '19 15
         End With
         '    DMCommon.Debug.MsgBox("Zone-CreateMainDataTable", moMainDataTable.Columns.Count)
      End Sub
      Public Shared Function GetTopoName() As String
         Return mtZoneMapThemeData.LineTopoName
      End Function

      Public Sub AddDataToMainTable()
         Dim oNewRow As System.Data.DataRow

         If moMainDataTable IsNot Nothing Then
            Try
               oNewRow = moMainDataTable.NewRow()

               With oNewRow
                  .Item(msZoneIDFieldName) = Me.ZoneID

                  .Item(msZoneNameFieldName) = Me.ZoneName

                  .Item(msZoneTypeFieldName) = Me.ZoneType
 

 


                  .Item(TopoReader.msAreaFldName) = MyBase.AcadArea(False)
                  .Item(TopoReader.msCentroidXFldName) = MyBase.ddCentroidX
                  .Item(TopoReader.msCentroidYFldName) = MyBase.ddCentroidY
                  '	.Item(msPlanStateFieldName) = CType(Me.PlanState, Integer)
                  '	.Item(msPlanStateTextFieldName) = Me.PlanStateText

                  .Item(TopoReader.msPerimeterFldName) = MyBase.ddPerimeter
                  .Item(TopoReader.msTopoIDFldName) = MyBase.TopoID
                  .Item(TopoReader.msAcObjIDFldName) = MyBase.dtCentroidAcObjID
               End With
               moMainDataTable.Rows.Add(oNewRow)
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "01_433")
            End Try

         Else
            System.Windows.Forms.MessageBox.Show("moMainDataTable Is  Nothing" & vbCrLf & CStr(2015), "01_453")
         End If
         '// 0-5 Parcel data 

      End Sub
      Public Shared ReadOnly Property MainView() As System.Data.DataView
         Get
            Dim sSort As String = String.Empty
            If moMainDataTable IsNot Nothing Then
               Dim oDataView As System.Data.DataView = New System.Data.DataView(moMainDataTable, String.Empty, sSort, System.Data.DataViewRowState.CurrentRows)
               oDataView.AllowEdit = False
               oDataView.AllowDelete = False
               oDataView.AllowNew = False
               Return oDataView
            Else
               Return Nothing
            End If
         End Get
      End Property
      Public Sub New(ByRef oPolygon As Autodesk.Gis.Map.Topology.Polygon)
         MyBase.New(oPolygon)
         MyBase.SetAttributeOrder()  'miaBlockAttribIndex
         '	MyBase.diTopoPurpose = DMAcadExt.enTopoPurpose.Parcel
         '	
         '    DMCommon.Functions.DispArray(dsaBlockAttribText, "03_878", True)
         mtZoneData = New ZoneData(dsaBlockAttribText)
         '   DMCommon.Debug.MsgBox("10_766", mtZoneData.ZoneName)
         If Not mtZoneData.Correct Then
            TplnProject.WriteMessageBox("Data is incorrect " & CStr(ddCentroidX) & "," & CStr(ddCentroidY), "TplnExpro - New_14")
         End If

      End Sub

      Protected Overrides ReadOnly Property AddBlockAttribIndex As Integer()
         Get
            Return Nothing
         End Get
      End Property

      Protected Overrides ReadOnly Property BlockAttribIndex As Integer()
         Get
            Return miaBlockAttribIndex
         End Get
      End Property
      Public ReadOnly Property ZoneID() As Integer
         Get
            Return mtZoneData.ZoneID

         End Get
      End Property
      Public ReadOnly Property ZoneName() As String
         Get
            Return mtZoneData.ZoneName

         End Get
      End Property
      Public ReadOnly Property ZoneType() As enZoneType
         Get
            Return mtZoneData.ZoneType

         End Get
      End Property


      Public Overrides Sub Terminate()

      End Sub
   End Class
End Namespace
