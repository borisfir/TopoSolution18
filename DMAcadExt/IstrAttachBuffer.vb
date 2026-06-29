Option Explicit On 
Option Strict On
Imports AutoCAD
Imports Infrastr.IstrGraph
Imports System.Data
Imports System.Windows.Forms

Friend Class IstrAttachBuffer
   Private Const msDWGAliasFldName As String = "DWGAlias"
   Private Const msXFldName As String = "X"
   Private Const msYFldName As String = "Y"
   Private Const msXMinFldName As String = "XMin"
   Private Const msXMaxFldName As String = "XMax"
   Private Const msYMinFldName As String = "YMin"
   Private Const msYMaxFldName As String = "YMax"
   Private Const msKeyNameFldName As String = "KeyName"
   Private Const msStatusFldName As String = "Status"
   Private Const msEntityFldName As String = "Entity"
   Private Const msBranchFldName As String = "Branch"
   Private Const msPointLinesXMLName As String = "PointLines.XML"
   Private Shared mtPoints As DataTable
   Private Shared mtMainFilePoints As DataTable
   Private Shared mtScaleFilePoints As DataTable


   Private Shared mtLines As DataTable
   Private Shared mtMainLines As DataTable

   Private Shared mtNodes As DataTable
   Private Shared msErrPointTest As String
   Private Shared miErrCounter As Integer
   Private Shared miMode As enBufferMode
   Private Shared mlstScaledLines As System.Collections.Generic.List(Of IstrLine)
   Public Shared Tolerance As Double = 0.01
   Public Shared PointTolerance As Double = 0.01
   Public Shared oXMLDoc As Xml.XmlDataDocument
   Public Shared Sub AddEntity(ByRef oEntity As IstrEntity)
      Try
         Select Case oEntity.ClassName
				Case IstrLine.LineClassName
					AddLine(DirectCast(oEntity, IstrLine))
            Case IstrBlock.BlockClassName
               If oEntity.Functionality = enEntityFunction.Node Then
                  AddNode(DirectCast(oEntity, IstrBlock))
               End If
					If oEntity.Role = enEntityRole.Attached Then
						AddPoint(DirectCast(oEntity, IstrBlock))
					End If
         End Select
      Catch oEx As Exception
			MessageBox.Show(oEx.Message & vbCrLf & oEntity.ClassName & vbCrLf & oEx.StackTrace, Application.ProductName & " IstrAttachBuffer - AddEntity")
      End Try
	End Sub
   Public Shared Sub AddPoint(ByRef oBlock As IstrBlock)
      Try
			Dim oPoint As TPlnPoint = oBlock.GetPoint()
			Dim oDataRow As DataRow = mtPoints.NewRow()
			If IstrInnerApplication.MultiDrawing Then
				oDataRow.Item(msDWGAliasFldName) = oBlock.DWGAlias
			End If

			oDataRow.Item(msXFldName) = oPoint.X
			oDataRow.Item(msYFldName) = oPoint.Y
			oDataRow.Item(msKeyNameFldName) = oBlock.KeyName
			oDataRow.Item(msEntityFldName) = oBlock
			mtPoints.Rows.Add(oDataRow)
		Catch oEx As Exception
			MessageBox.Show(oEx.Message, "IstrAttachBuffer - AddPoint")
		End Try
	End Sub


   Private Shared Sub zzAddPoint(ByRef oEntity As IstrEntity, ByVal oPoint As TPlnPoint, ByVal bMainFile As Boolean)
      Dim tPoints As DataTable
      If bMainFile Then
         tPoints = mtMainFilePoints
      Else
         tPoints = mtScaleFilePoints
      End If
      Dim oDataRow As DataRow = tPoints.NewRow()

      oDataRow.Item(msXFldName) = oPoint.X
      oDataRow.Item(msYFldName) = oPoint.Y
      oDataRow.Item(msKeyNameFldName) = oEntity.KeyName
      oDataRow.Item(msEntityFldName) = oEntity
      tPoints.Rows.Add(oDataRow)

   End Sub


   Public Shared Sub AddLine(ByRef oLine As IstrLine)
      Dim oMinPoint As TPlnPoint = Nothing
		Dim oMaxPoint As TPlnPoint = Nothing
		Dim tExtents As Autodesk.AutoCAD.DatabaseServices.Extents3d
		Try
			tExtents = oLine.GetExtents()
			oMinPoint = New TPlnPoint(tExtents.MinPoint)
			oMaxPoint = New TPlnPoint(tExtents.MaxPoint)


			If oMinPoint IsNot Nothing AndAlso oMaxPoint IsNot Nothing Then
				Dim oDataRow As DataRow = mtLines.NewRow()
				 
				oDataRow.Item(msXMinFldName) = oMinPoint.X
				oDataRow.Item(msYMinFldName) = oMinPoint.Y
				oDataRow.Item(msXMaxFldName) = oMaxPoint.X
				oDataRow.Item(msYMaxFldName) = oMaxPoint.Y
				oDataRow.Item(msKeyNameFldName) = oLine.KeyName
				oDataRow.Item(msStatusFldName) = 0
				oDataRow.Item(msEntityFldName) = oLine
			 
				mtLines.Rows.Add(oDataRow)
			End If
		Catch oEx As Exception
			MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrAttachBuffer - AddLine")
		End Try
   End Sub
  
   Private Shared Function zzCreatePointTable() As DataTable
      Dim oDataColumn As DataColumn
      Dim tPoints As DataTable
      tPoints = New DataTable("Points")

      oDataColumn = New DataColumn(msDWGAliasFldName, System.Type.GetType("System.String"))
      tPoints.Columns.Add(oDataColumn)

      oDataColumn = New DataColumn(msXFldName, System.Type.GetType("System.Double"))
      tPoints.Columns.Add(oDataColumn)

      oDataColumn = New DataColumn(msYFldName, System.Type.GetType("System.Double"))
      tPoints.Columns.Add(oDataColumn)

      oDataColumn = New DataColumn(msKeyNameFldName, System.Type.GetType("System.String"))
      tPoints.Columns.Add(oDataColumn)

      oDataColumn = New DataColumn(msEntityFldName, System.Type.GetType("Infrastr.IstrEntity"))
      tPoints.Columns.Add(oDataColumn)
      Return tPoints
   End Function
   Private Shared Function zzCreateLineTable() As DataTable
      Dim oDataColumn As DataColumn
      Dim tLines As DataTable
      tLines = New DataTable("Lines")
      oDataColumn = New DataColumn(msDWGAliasFldName, System.Type.GetType("System.String"))
      tLines.Columns.Add(oDataColumn)
      oDataColumn = New DataColumn(msXMinFldName, System.Type.GetType("System.Double"))
      tLines.Columns.Add(oDataColumn)
      oDataColumn = New DataColumn(msXMaxFldName, System.Type.GetType("System.Double"))
      tLines.Columns.Add(oDataColumn)

      oDataColumn = New DataColumn(msYMinFldName, System.Type.GetType("System.Double"))
      tLines.Columns.Add(oDataColumn)
      oDataColumn = New DataColumn(msYMaxFldName, System.Type.GetType("System.Double"))
      tLines.Columns.Add(oDataColumn)

      oDataColumn = New DataColumn(msKeyNameFldName, System.Type.GetType("System.String"))
      tLines.Columns.Add(oDataColumn)

      oDataColumn = New DataColumn(msStatusFldName, System.Type.GetType("System.Int32"))
      tLines.Columns.Add(oDataColumn)

      oDataColumn = New DataColumn(msEntityFldName, System.Type.GetType("Infrastr.IstrEntity"))
      tLines.Columns.Add(oDataColumn)

      oDataColumn = New DataColumn(msBranchFldName, System.Type.GetType("Infrastr.IstrGraph.Branch"))
      tLines.Columns.Add(oDataColumn)

      Return tLines
   End Function
   Public Shared Sub CreateTablesAAA()
      Dim oDataColumn As DataColumn

      mtPoints = New DataTable("Points")

      oDataColumn = New DataColumn(msDWGAliasFldName, System.Type.GetType("System.String"))
      mtPoints.Columns.Add(oDataColumn)
      oDataColumn = New DataColumn(msXFldName, System.Type.GetType("System.Double"))
      mtPoints.Columns.Add(oDataColumn)
      oDataColumn = New DataColumn(msYFldName, System.Type.GetType("System.Double"))
      mtPoints.Columns.Add(oDataColumn)

      oDataColumn = New DataColumn(msKeyNameFldName, System.Type.GetType("System.String"))
      mtPoints.Columns.Add(oDataColumn)

      oDataColumn = New DataColumn(msEntityFldName, System.Type.GetType("Infrastr.IstrEntity"))
      mtPoints.Columns.Add(oDataColumn)

      mtLines = New DataTable("Lines")
      oDataColumn = New DataColumn(msDWGAliasFldName, System.Type.GetType("System.String"))
      mtLines.Columns.Add(oDataColumn)
      oDataColumn = New DataColumn(msXMinFldName, System.Type.GetType("System.Double"))
      mtLines.Columns.Add(oDataColumn)
      oDataColumn = New DataColumn(msXMaxFldName, System.Type.GetType("System.Double"))
      mtLines.Columns.Add(oDataColumn)

      oDataColumn = New DataColumn(msYMinFldName, System.Type.GetType("System.Double"))
      mtLines.Columns.Add(oDataColumn)
      oDataColumn = New DataColumn(msYMaxFldName, System.Type.GetType("System.Double"))
      mtLines.Columns.Add(oDataColumn)

      oDataColumn = New DataColumn(msKeyNameFldName, System.Type.GetType("System.String"))
      mtLines.Columns.Add(oDataColumn)

      oDataColumn = New DataColumn(msStatusFldName, System.Type.GetType("System.Int32"))
      mtLines.Columns.Add(oDataColumn)

      oDataColumn = New DataColumn(msEntityFldName, System.Type.GetType("Infrastr.IstrEntity"))
      mtLines.Columns.Add(oDataColumn)

      oDataColumn = New DataColumn(msBranchFldName, System.Type.GetType("Infrastr.IstrGraph.Branch"))
      mtLines.Columns.Add(oDataColumn)


      '/// Create Node Table 
      mtNodes = New DataTable("Nodes")
      oDataColumn = New DataColumn(msXFldName, System.Type.GetType("System.Double"))
      mtNodes.Columns.Add(oDataColumn)
      oDataColumn = New DataColumn(msYFldName, System.Type.GetType("System.Double"))
      mtNodes.Columns.Add(oDataColumn)

      oDataColumn = New DataColumn(msEntityFldName, System.Type.GetType("Infrastr.IstrGraph.Node"))
      mtNodes.Columns.Add(oDataColumn)

   End Sub
   Public Shared Sub CreateTables()
      Dim oDataColumn As DataColumn

      mtPoints = zzCreatePointTable()
      mtMainFilePoints = zzCreatePointTable()
      mtScaleFilePoints = zzCreatePointTable()

      mtLines = zzCreateLineTable()

      mtMainLines = zzCreateLineTable()
      '/// Create Node Table 
      mtNodes = New DataTable("Nodes")
      oDataColumn = New DataColumn(msXFldName, System.Type.GetType("System.Double"))
      mtNodes.Columns.Add(oDataColumn)
      oDataColumn = New DataColumn(msYFldName, System.Type.GetType("System.Double"))
      mtNodes.Columns.Add(oDataColumn)

      oDataColumn = New DataColumn(msEntityFldName, System.Type.GetType("Infrastr.IstrGraph.Node"))
      mtNodes.Columns.Add(oDataColumn)
      mlstScaledLines = New System.Collections.Generic.List(Of IstrLine)
   End Sub
   Public Shared Sub InitXMLDoc()
      oXMLDoc = New Xml.XmlDataDocument

   End Sub
   Public Shared Property Mode() As enBufferMode
      Get
         Return miMode
      End Get
      Set(ByVal iValue As enBufferMode)
         miMode = iValue
      End Set
   End Property

   Public Shared Sub EraseTables()
      mtPoints.Dispose()
      mtPoints = Nothing
      mtLines.Dispose()
      mtLines = Nothing
   End Sub

	Public Shared Sub AttachPointsToLines()
		'	IstrDrawing.WriteMessage("----- start AttachPointsToLines. Points:" & mtPoints.Rows.Count.ToString() & " Lines:" & mtLines.Rows.Count.ToString())
		'	MessageBox.Show("----- Points:" & mtPoints.Rows.Count.ToString() & " Lines:" & mtLines.Rows.Count.ToString(), "01_239")
		Dim oEntity As IstrEntity
		Dim oLine As IstrEntity
		Dim dX As Double
		Dim dY As Double
		Dim oPoint As TPlnPoint
		Dim sDWGAlias As String = String.Empty
		''   Dim o As IstrBlockType
		Dim oXMLDoc As Xml.XmlDocument
		Dim oXMLRec As Xml.XmlNode
		Dim oXMLElem As Xml.XmlNode

		IstrDrawing.WriteMessage("!!! start AttachPointsToLines:" & mtPoints.Rows.Count.ToString())
		oXMLDoc = New Xml.XmlDataDocument
		oXMLDoc.LoadXml("<PointLines></PointLines>")
		Dim oRootElement As Xml.XmlElement = oXMLDoc.DocumentElement()


		For Each oRow As DataRow In mtPoints.Rows
			Try
				oEntity = DirectCast(oRow.Item(msEntityFldName), IstrEntity)
				dX = DirectCast(oRow.Item(msXFldName), Double)
				dY = DirectCast(oRow.Item(msYFldName), Double)
				oPoint = New TPlnPoint(dX, dY)
				If IstrInnerApplication.MultiDrawing Then
					sDWGAlias = DirectCast(oRow.Item(msDWGAliasFldName), String)
				End If
				oLine = zzFindLineByPoint(oPoint, oEntity.BaseKeyName, sDWGAlias)
				If oLine IsNot Nothing Then
					zzAttach(oLine, oEntity)
					oXMLRec = oXMLDoc.CreateNode(Xml.XmlNodeType.Element, "Rec", String.Empty)
					oRootElement.AppendChild(oXMLRec)
					oXMLElem = oXMLDoc.CreateNode(Xml.XmlNodeType.Element, "Point", String.Empty)
					oXMLElem.InnerText = oEntity.Handle
					oXMLRec.AppendChild(oXMLElem)
					oXMLElem = oXMLDoc.CreateNode(Xml.XmlNodeType.Element, "Line", String.Empty)
					oXMLElem.InnerText = oLine.Handle
					oXMLRec.AppendChild(oXMLElem)
				Else
					oEntity.Highlight(True)
					IstrDrawing.WriteMessage("Point: " & sDWGAlias & "/ " & oEntity.KeyName & " | " & CStr(dX) & "," & CStr(dY))
				End If

			Catch oEx As Exception
				MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrAttachBuffer - AttachPointsToLines_1")
			End Try

		Next
		Try
			'	MessageBox.Show("Save:" & IstrDBManager.XMLFolder & "\" & msPointLinesXMLName, "IstrAttachBuffer - AttachPointsToLines_1")

			oXMLDoc.Save(IstrDBManager.XMLFolder & "\" & msPointLinesXMLName)
		Catch oEx As Exception
			MessageBox.Show(oEx.Message & vbCrLf & IstrDBManager.XMLFolder & "\" & msPointLinesXMLName & vbCrLf & oEx.StackTrace, "IstrAttachBuffer - AttachPointsToLines_2")
		End Try

	End Sub
   Public Shared Sub AttachPointsToPoints()
		Dim oMainEntity As IstrEntity
      Dim oScaledEntity As IstrEntity
      Dim sKeyName As String
		Dim dX As Double
      Dim dY As Double
      Dim oPoint As TPlnPoint
      Dim sDWGAlias As String = String.Empty
		Dim oXMLDoc As Xml.XmlDocument
      oXMLDoc = New Xml.XmlDataDocument
      oXMLDoc.LoadXml("<PointLines></PointLines>")
		Dim oRootElement As Xml.XmlElement = oXMLDoc.DocumentElement()
		IstrDrawing.WriteMessage("!!! start AttachPointsToPoints:" & mtMainFilePoints.Rows.Count.ToString())
      For Each oRow As DataRow In mtMainFilePoints.Rows
         Try
            oMainEntity = DirectCast(oRow.Item(msEntityFldName), IstrEntity)
            dX = DirectCast(oRow.Item(msXFldName), Double)
            dY = DirectCast(oRow.Item(msYFldName), Double)
            sKeyName = DirectCast(oRow.Item(msKeyNameFldName), String)
            oPoint = New TPlnPoint(dX, dY)

            oScaledEntity = zzFindPointByPoint(oPoint, sKeyName)
            If oScaledEntity IsNot Nothing Then
               zzAddScaledEntity(oMainEntity, oScaledEntity)
               '  oXMLRec = oXMLDoc.CreateNode(Xml.XmlNodeType.Element, "Rec", String.Empty)
               ' oRootElement.AppendChild(oXMLRec)
               ' oXMLElem = oXMLDoc.CreateNode(Xml.XmlNodeType.Element, "Point", String.Empty)
               ' oXMLElem.InnerText = oEntity.Handle
               ' oXMLRec.AppendChild(oXMLElem)
               ' oXMLElem = oXMLDoc.CreateNode(Xml.XmlNodeType.Element, "Line", String.Empty)
               ' oXMLElem.InnerText = oLine.Handle
               ' oXMLRec.AppendChild(oXMLElem)
            Else
               oMainEntity.Highlight(True)
            End If

         Catch oEx As Exception
				MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrAttachBuffer - AttachPointsToPoints_1")
			End Try

      Next
      Try
         ' oXMLDoc.Save(IstrDBManager.XMLFolder & "\" & msPointLinesXMLName)
      Catch oEx As Exception
         MessageBox.Show(oEx.Message, "IstrAttachBuffer - AttachPointsToPoints_2")
      End Try

   End Sub
 
  
   Public Shared Sub InputNotAttachedLines()
      Dim oLine As IstrLine
      Dim sRowFilter As String = msStatusFldName & "=0"
		Dim oLinesView As DataView = New DataView(mtLines, sRowFilter, String.Empty, DataViewRowState.Added)
		Dim sDWGAlias As String
		IstrDrawing.WriteMessage("!!!+1 start InputNotAttachedLines:" & oLinesView.Count.ToString())
		'	MessageBox.Show("InputNotAttachedLines", "IstrAttachBuffer - InputNotAttachedLines")
      If oLinesView.Count > 0 Then
         For Each oDataRow As DataRowView In oLinesView
            Try
					oLine = DirectCast(oDataRow.Item(msEntityFldName), IstrLine)
					If Not oLine.DBConnected Then	  'AndAlso IstrInnerApplication.LoadDrawingAddDB
						oLine.LoadData(IstrPrimaryKey.NullPrimaryKey, String.Empty, True)
					End If

					sDWGAlias = DirectCast(oDataRow.Item(msDWGAliasFldName), String)
					IstrDrawing.WriteMessage("Line: " & sDWGAlias & "/ " & oLine.KeyName & " | " & oLine.StartPoint.Coordinates)
            Catch oEx As Exception
					MessageBox.Show(oEx.Message, "IstrAttachBuffer - InputNotAttachedLines")
            End Try
         Next
      End If
   End Sub
   Private Shared ReadOnly Property PointCount() As Integer
      Get
         Return mtPoints.Rows.Count
      End Get
   End Property
   Private Shared ReadOnly Property LineCount() As Integer
      Get
         Return mtLines.Rows.Count
      End Get
   End Property

   Private Shared Sub zzAddMainLine(ByRef oLine As IstrLine)
      Dim oMinPoint As TPlnPoint = Nothing
      Dim oMaxPoint As TPlnPoint = Nothing
      Try
         oLine.GetBoundPoints(oMinPoint, oMaxPoint)
         Dim oDataRow As DataRow = mtMainLines.NewRow()

         oDataRow.Item(msXMinFldName) = oMinPoint.X
         oDataRow.Item(msYMinFldName) = oMinPoint.Y
         oDataRow.Item(msXMaxFldName) = oMaxPoint.X
         oDataRow.Item(msYMaxFldName) = oMaxPoint.Y
         oDataRow.Item(msKeyNameFldName) = oLine.KeyName
         oDataRow.Item(msStatusFldName) = 0
         oDataRow.Item(msEntityFldName) = oLine
         mtMainLines.Rows.Add(oDataRow)

      Catch oEx As Exception

      End Try
   End Sub
   Private Shared Function zzFindLineByPoint(ByVal oPoint As TPlnPoint, ByVal sKeyNames As String, Optional ByVal sDWGAlias As String = IstrGlobal.EmptyString) As IstrLine
		Dim oLine As IstrLine
		Dim sDWGCriteria As String = zzGetDWGCriteria(sDWGAlias)
		Dim saKeyNames() As String = Strings.Split(sKeyNames, ",")

      Dim sCriteriaKey As String = "("
      Dim bResp As Boolean
		Dim oDrawing As IstrDrawing = Nothing
		For iIndex As Integer = 0 To saKeyNames.GetUpperBound(0)
			If iIndex <> 0 Then sCriteriaKey &= " OR "
			sCriteriaKey &= msKeyNameFldName & "='" & saKeyNames(iIndex) & "'"
		Next
		sCriteriaKey &= ")"
		Dim sRowFilter As String
		Dim oLinesView As DataView
		''OLD 19/07/05   Dim sRowFilter As String = sStatusFldName & "=0 AND " & sKeyNameFldName & "='" & sKeyName & "' AND " & sXMinFldName & "<" & CStr(oPoint.X + Tolerance) & " AND " & sXMaxFldName & ">" & CStr(oPoint.X - Tolerance) & " AND " & sYMinFldName & "<" & CStr(oPoint.Y + Tolerance) & " AND " & sYMaxFldName & ">" & CStr(oPoint.Y - Tolerance)
		Try
			sRowFilter = sDWGCriteria & msStatusFldName & "=0 AND " & sCriteriaKey & " AND " & msXMinFldName & "<" & CStr(oPoint.X + Tolerance) & " AND " & msXMaxFldName & ">" & CStr(oPoint.X - Tolerance) & " AND " & msYMinFldName & "<" & CStr(oPoint.Y + Tolerance) & " AND " & msYMaxFldName & ">" & CStr(oPoint.Y - Tolerance)
			oLinesView = New DataView(mtLines, sRowFilter, String.Empty, DataViewRowState.Added)

		Catch oEx As Exception
			MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrAttachBuffer - zzFindLineByPoint_9")
			Return Nothing

		End Try

		If oLinesView.Count > 0 Then
			Try

				For Each oDataRow As DataRowView In oLinesView

					Try
						oLine = DirectCast(oDataRow.Item(msEntityFldName), IstrLine)
						'	oLine.ClearElevation()
						If sDWGAlias.Length = 0 Then
							oDrawing = IstrAcadApp.ActiveDrawing
						Else
							oDrawing = IstrAcadApp.Drawings.Item(sDWGAlias)
						End If
						oPoint.Z = oLine.GetElevation()
						bResp = oDrawing.IsIntersectWithPointX(oLine.AcadEntityX, oPoint, Tolerance)
						If bResp Then
							oDataRow.Item(msStatusFldName) = 1
							Return oLine
						Else
							'''''''''''''	IstrDrawing.WriteMessage("Line: " & oLine.KeyName & " | " & oLine.StartPoint.Coordinates & ";" & oLine.EndPoint.Coordinates)
						End If
					Catch oEx As Exception
						MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrAttachBuffer - zzFindLineByPoint_3")
						Return Nothing
					End Try
				Next
				IstrDrawing.WriteMessage("!!!!!! LineCount= " & CStr(oLinesView.Count))
				If oDrawing IsNot Nothing Then
					oDrawing.DeleteTriangle()
				End If

			Catch oEx As Exception
				MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "IstrAttachBuffer - zzFindLineByPoint_1")
			End Try
			Return Nothing
		End If
		Return Nothing
   End Function

   Private Shared Function zzFindPointByPoint(ByVal oPoint As TPlnPoint, ByVal sKeyName As String) As IstrEntity
      Dim oEntity As IstrEntity
      Dim sRowFilter As String = msXFldName & "<" & CStr(oPoint.X + Tolerance) & " AND " & msXFldName & ">" & CStr(oPoint.X - Tolerance) & " AND " & msYFldName & "<" & CStr(oPoint.Y + Tolerance) & " AND " & msYFldName & ">" & CStr(oPoint.Y - Tolerance)
      Dim oPointView As DataView = New DataView(mtScaleFilePoints, sRowFilter, String.Empty, DataViewRowState.Added)
      Dim oDataRow As DataRowView

      If oPointView.Count = 1 Then
         oDataRow = oPointView.Item(0)
         oEntity = DirectCast(oDataRow.Item(msEntityFldName), IstrEntity)
         Return oEntity
      Else
         Return Nothing
      End If
	End Function


   Private Shared Function zzGetDWGCriteria(ByVal sDWGAlias As String) As String
      If sDWGAlias.Length = 0 Then
         Return String.Empty
      Else
         Return "(" & msDWGAliasFldName & "='" & sDWGAlias & "') AND "
      End If
      Return String.Empty
   End Function
  

 
   Private Shared Sub zzAttach(ByRef oBaseEntity As IstrEntity, ByRef oEntityAttached As IstrEntity)
		Try

			oBaseEntity.AddAttachedEntity(oEntityAttached)
			oEntityAttached.BaseEntity = oBaseEntity
			If oBaseEntity.EntitySource = IstrEntity.enEntitySource.DWG AndAlso Not oBaseEntity.DBConnected Then
				oBaseEntity.LoadComplexData(True)
			End If

		Catch oEx As Exception
			MessageBox.Show(oEx.Message, Application.ProductName & " IstrAttachBuffer - zzAttach")
		End Try
   End Sub
   Private Shared Sub zzAddScaledEntity(ByRef oMainEntity As IstrEntity, ByRef oScaledEntity As IstrEntity)
      oScaledEntity.Scaled = True
      oScaledEntity.BaseEntity = oMainEntity
      oMainEntity.ScaledEntity = oScaledEntity
   End Sub
End Class
