Option Explicit On 
Option Strict On
Public Class TPlResource
   Public ResourceTheme As enResourceTheme
   Public ResItems() As String
   Public ResourceGhild() As TPlResource
   Public ObjectNo As Integer = 0
   Public Level As Integer
   Public ChildCount As Integer
	Public Sub New(ByVal iResourceTheme As enResourceTheme, ByVal oDataReader As System.Data.Common.DbDataReader)
		''''DMOffice 29/06/06
		Dim iItemUB As Integer
		ResourceTheme = iResourceTheme
		Try
			ObjectNo = oDataReader.GetInt32(0)
			Level = oDataReader.GetInt32(2)
			iItemUB = oDataReader.GetInt32(1)
			ReDim ResItems(iItemUB)
			For iIndex As Integer = iItemUB To 0 Step -1
				If oDataReader.IsDBNull(3) Then
					ResItems(iIndex) = String.Empty
				Else
					ResItems(iIndex) = oDataReader.GetString(3)
				End If

				If iIndex > 0 Then oDataReader.Read()
			Next
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "TplResource - New")
		End Try

	End Sub
   Public Sub AddObject(ByRef oResource As TPlResource)
      ReDim Preserve ResourceGhild(ChildCount)
      ResourceGhild(ChildCount) = oResource
      ChildCount += 1
   End Sub
   Public Function GetStrItem(ByVal iItemNo As Integer) As String()
      Dim sItem As String = zzGetItem(iItemNo)
      If sItem.Length <> 0 Then
         Return Strings.Split(sItem, ",")
      Else
         Return Nothing
      End If
	End Function
	Public Overrides Function ToString() As String
		Return ObjectNo.ToString()
	End Function
    Public ReadOnly Property ItemsUB As Integer
        Get
            If ResItems IsNot Nothing Then
                Return ResItems.GetUpperBound(0)
            Else
                Return -1
            End If
        End Get
    End Property

	Public Function GetIntItem(ByVal iItemNo As Integer, Optional ByVal iDefault As Integer = -1, Optional ByVal bMsgTest As Boolean = False) As Integer()
		Dim sItem As String = zzGetItem(iItemNo)
		If sItem.Length <> 0 Then
			If bMsgTest Then
				System.Windows.Forms.MessageBox.Show(CStr(iItemNo) & ":" & sItem & ":", "01_900")
			End If
			Dim saElem() As String = Strings.Split(sItem, ",")
			Dim iaElem(saElem.GetUpperBound(0)) As Integer
			For iIndex As Integer = 0 To saElem.GetUpperBound(0)
				If saElem(iIndex).Length = 0 Then
					iaElem(iIndex) = iDefault
				Else
					Try
						iaElem(iIndex) = Convert.ToInt32(saElem(iIndex))
					Catch oEx As Exception
						System.Windows.Forms.MessageBox.Show(CStr(iItemNo) & vbCrLf & CStr(ResourceTheme) & vbCrLf & CStr(ObjectNo), "Err: Not Integer")
					End Try
				End If
			Next
			Return iaElem
		Else
			' System.Windows.Forms.MessageBox.Show(zzGetErrMessage(iItemNo), "TplResource - GetIntItem")
			Return Nothing
		End If
	End Function
   ''' <summary>
   ''' dmOffice 20/06/2006
   ''' </summary>
   ''' <param name="iItemNo"></param>
   ''' <returns></returns>
   ''' <remarks></remarks>
   Public Function GetDblItem(ByVal iItemNo As Integer) As Double()
      Dim sItem As String = zzGetItem(iItemNo)
      If sItem.Length <> 0 Then
         Dim saElem() As String = Strings.Split(sItem, ",")
         Dim daElem(saElem.GetUpperBound(0)) As Double
         For iIndex As Integer = 0 To saElem.GetUpperBound(0)
            If IsNumeric(saElem(iIndex)) Then
               daElem(iIndex) = Convert.ToDouble(saElem(iIndex))
            Else
					System.Windows.Forms.MessageBox.Show(CStr(iItemNo) & ":" & CStr(daElem(iIndex)), "Err: Not Double")
            End If
         Next
         Return daElem
      Else
         System.Windows.Forms.MessageBox.Show(zzGetErrMessage(iItemNo), "TplResource - GetDblItem")
         Return Nothing
      End If

   End Function

   Public Function GetPoint(ByVal iItemNo As Integer) As System.Drawing.Point
      Dim sItem As String = zzGetItem(iItemNo)
      If sItem.Length <> 0 Then
         Dim saElem() As String = Strings.Split(sItem, ",")
         Dim iaElem(saElem.GetUpperBound(0)) As Integer
         Dim oPoint As System.Drawing.Point = New System.Drawing.Point(CType(saElem(0), Integer), CType(saElem(1), Integer))
         Return oPoint
      Else
         System.Windows.Forms.MessageBox.Show(zzGetErrMessage(iItemNo), "TplResource - GetPoint")
         Return Nothing
      End If

   End Function
   Public Function GetPointArray(ByVal iItemNo As Integer) As System.Drawing.Point()
      Dim sItem As String = zzGetItem(iItemNo)
      If sItem.Length <> 0 Then
         Dim saPoints() As String = Strings.Split(sItem, ";")
         Dim oaElem(saPoints.GetUpperBound(0)) As System.Drawing.Point
         Dim sPoint As String
         Dim oaPoints(saPoints.GetUpperBound(0)) As System.Drawing.Point
         Dim sPointCoord() As String
         For iIndex As Integer = 0 To saPoints.GetUpperBound(0)
            sPoint = saPoints(iIndex)
            sPointCoord = Strings.Split(sPoint, ",")
            oaPoints(iIndex) = New System.Drawing.Point(CType(sPointCoord(0), Integer), CType(sPointCoord(1), Integer))
         Next

         Return oaPoints
      Else
         System.Windows.Forms.MessageBox.Show(zzGetErrMessage(iItemNo), "TplResource - GetRectangleArray")
         Return Nothing
      End If

   End Function
   Public Function GetRectangleArray(ByVal iItemNo As Integer) As System.Drawing.Rectangle()
      Dim sItem As String = zzGetItem(iItemNo)

      If sItem.Length <> 0 Then
         Dim saRectangles() As String = Strings.Split(sItem, ";")
         Dim oaElem(saRectangles.GetUpperBound(0)) As System.Drawing.Rectangle
         Dim sRectangle As String
         Dim oaRectangles(saRectangles.GetUpperBound(0)) As System.Drawing.Rectangle
         Dim sRectParam() As String
         For iIndex As Integer = 0 To saRectangles.GetUpperBound(0)
            sRectangle = saRectangles(iIndex)
            sRectParam = Strings.Split(sRectangle, ",")
            Try
					oaRectangles(iIndex) = New System.Drawing.Rectangle(CType(sRectParam(0), Integer), CType(sRectParam(1), Integer), CType(sRectParam(2), Integer), CType(sRectParam(3), Integer))
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "TplResource - GetRectangleArray")
            End Try

         Next

         Return oaRectangles
      Else
         '  System.Windows.Forms.MessageBox.Show(zzGetErrMessage(iItemNo), "TplResource - GetRectangleArray")
         Return Nothing
      End If

   End Function

   Public Function GetSize(ByVal iItemNo As Integer) As System.Drawing.Size
      Dim sItem As String = zzGetItem(iItemNo)
      If sItem.Length <> 0 Then
         Dim saElem() As String = Strings.Split(sItem, ",")
         Dim iaElem(saElem.GetUpperBound(0)) As Integer
         Dim oSize As System.Drawing.Size = New System.Drawing.Size(CType(saElem(0), Integer), CType(saElem(1), Integer))
         Return oSize
      Else
         System.Windows.Forms.MessageBox.Show(zzGetErrMessage(iItemNo), "TplResource - GetSize")
         Return Nothing
      End If

	End Function
	 
	Public Function GetChild(ByVal iChildIndex As Integer) As TPlResource
		Try
			If iChildIndex <= ResourceGhild.GetUpperBound(0) Then
				'	System.Windows.Forms.MessageBox.Show(CStr(iChildIndex) & ":" & CStr(ResourceGhild.GetUpperBound(0)), "01_580")
				Return ResourceGhild(iChildIndex)
			Else
				Return Nothing
			End If

		Catch oEx As Exception
			Return Nothing
		End Try
	End Function
   ''DMOffice  09/07/06
   Private Function zzGetItem(ByVal iItemNo As Integer) As String
      Try
         If iItemNo <= ResItems.GetUpperBound(0) Then
            Return ResItems(iItemNo)
         Else
            ' System.Windows.Forms.MessageBox.Show("Resource item: " & CStr(iItemNo) & " was not found", "TPlResource - zzGetItem")
            System.Windows.Forms.MessageBox.Show(zzGetErrMessage(iItemNo), "TplResource - zzGetItem_1")
            Return String.Empty
         End If
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & CStr(iItemNo), "TPlResource - zzGetItem_2")
         Return String.Empty
      End Try
   End Function
	Private Function zzGetErrMessage(ByVal iItemNo As Integer) As String

		Dim sMsg As String
		sMsg = "Theme: " & ResourceTheme.ToString() & vbCrLf & "" & "ObjectNo: " & CStr(ObjectNo) & vbCrLf & "Level No: " & CStr(Level) & vbCrLf & "ItemUB: " & CStr(ResItems.GetUpperBound(0)) & vbCrLf & "Item No: " & CStr(iItemNo) & vbCrLf & "Item 0:UB: " & CStr(ResItems(0) & ":" & ResItems(ResItems.GetUpperBound(0))) & " was not found"
		Return sMsg
	End Function
End Class

 