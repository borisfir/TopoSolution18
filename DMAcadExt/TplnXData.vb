Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Public Class TplnXData
	Private Enum enAppStatus
		AppStatusDefault
		AppStatusDefined
		AppStatusOpened
		AppStatusOK
   End Enum
   ' DataBase Handle	1005. The handle of an entity.
	Private Enum enCodes
		[String] = 1000
		AppName = 1001
      OpenClose = 1002
      Handle = 1005
		[Double] = 1040
		[Short] = 1070
		[Integer] = 1071

	End Enum
	Private Shared mtAppOpen As Autodesk.AutoCAD.DatabaseServices.TypedValue = New TypedValue(enCodes.OpenClose, "{")
	Private Shared mtAppClose As TypedValue = New TypedValue(enCodes.OpenClose, "}")
	Private mtAppName As TypedValue
	Private mtaTypedValue() As TypedValue
	Private miStart As Integer = -1
	Dim miEnd As Integer = -1
	Dim miAppStatus As enAppStatus = enAppStatus.AppStatusDefault
	'Private miTopoID As Integer
	Public Sub New(oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer, sXDAppName As String, Optional bTest As Boolean = False)
		If oResBuffer IsNot Nothing Then
			mtaTypedValue = oResBuffer.AsArray()
			zzSetAppName(sXDAppName)

			For iIndex As Integer = 0 To mtaTypedValue.GetUpperBound(0)
				If bTest Then
					zzDisp(mtaTypedValue(iIndex))
				End If

				Select Case mtaTypedValue(iIndex)
					Case mtAppName
						'System.Windows.Forms.MessageBox.Show(CStr(iIndex), "02_330")
						If miAppStatus = enAppStatus.AppStatusDefault Then
							miAppStatus = enAppStatus.AppStatusDefined
							miStart = iIndex + 2
						End If
					Case mtAppOpen
						'	System.Windows.Forms.MessageBox.Show(CStr(iIndex), "02_331")
						If miAppStatus = enAppStatus.AppStatusDefined AndAlso miStart = iIndex + 1 Then
							miAppStatus = enAppStatus.AppStatusOpened
						End If
					Case mtAppClose
						'System.Windows.Forms.MessageBox.Show(CStr(iIndex), "02_339")
						If miAppStatus = enAppStatus.AppStatusOpened Then
							miAppStatus = enAppStatus.AppStatusOK
							miEnd = iIndex - 1
						End If
					Case Else
				End Select
			Next
			If bTest Then
				System.Windows.Forms.MessageBox.Show(miAppStatus.ToString() & ":" & CStr(miEnd), "02_350a")
			End If
			'	System.Windows.Forms.MessageBox.Show(CStr(miStart) & ":" & CStr(miEnd), "02_350")
			'	zzDisp(tAppName)
		End If
	End Sub
	Public Sub New(iValuesCount As Integer, sXDAppName As String)
		ReDim mtaTypedValue(iValuesCount - 1)
		zzSetAppName(sXDAppName)
		miStart = 0
		miEnd = iValuesCount - 1
		miAppStatus = enAppStatus.AppStatusOK
	End Sub


	Public ReadOnly Property HasData As Boolean
		Get
			Return (miAppStatus = enAppStatus.AppStatusOK)
		End Get
	End Property
	Public Function GetInt(iIndex As Integer, ByRef iValue As Integer) As Boolean
		If (miAppStatus = enAppStatus.AppStatusOK) AndAlso (iIndex <= miEnd - miStart) Then
			Dim tValue As TypedValue = mtaTypedValue(iIndex + miStart)
			If Convert.ToInt32(tValue.TypeCode) = enCodes.Integer Then
				Try
					iValue = DirectCast(tValue.Value, Integer)
					Return True
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & tValue.Value.ToString(), "TplnXData - GetInt")
					Return False
				End Try
			Else
				Return False
			End If
		Else
			Return False
		End If
	End Function
	Public Sub SetInt(iIndex As Integer, iValue As Integer)
		If (miAppStatus = enAppStatus.AppStatusOK) AndAlso (iIndex <= miEnd - miStart) Then
			Dim tValue As TypedValue = New TypedValue(enCodes.Integer, iValue)
			mtaTypedValue(iIndex + miStart) = tValue
		End If
	End Sub

	Public Function GetDbl(iIndex As Integer, ByRef dValue As Double) As Boolean
		If (miAppStatus = enAppStatus.AppStatusOK) AndAlso (iIndex <= miEnd - miStart) Then
			Dim tValue As TypedValue = mtaTypedValue(iIndex + miStart)
         If tValue.TypeCode = zzShortCode(enCodes.Double) Then

            Try
               dValue = DirectCast(tValue.Value, Double)
               Return True
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & tValue.Value.ToString(), "TplnXData - GetInt")
               Return False
            End Try
         Else
            Return False
         End If
		Else
			Return False
		End If

	End Function
	Public Sub SetDbl(iIndex As Integer, dValue As Double)
		If (miAppStatus = enAppStatus.AppStatusOK) AndAlso (iIndex <= miEnd - miStart) Then
			Dim tValue As TypedValue = New TypedValue(enCodes.Double, dValue)
			mtaTypedValue(iIndex + miStart) = tValue
		End If
	End Sub
	Public Function GetStr(iIndex As Integer, ByRef sValue As String) As Boolean

		If (miAppStatus = enAppStatus.AppStatusOK) AndAlso (iIndex <= miEnd - miStart) Then
			Dim tValue As TypedValue = mtaTypedValue(iIndex + miStart)
         If tValue.TypeCode = zzShortCode(enCodes.String) Then

            Try
               sValue = DirectCast(tValue.Value, String)
               Return True
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & tValue.Value.ToString(), "TplnXData - GetStr")
               Return False
            End Try
         Else
            Return False
         End If
		Else
			Return False
		End If

	End Function
	Public Sub SetStr(iIndex As Integer, sValue As String)

		If (miAppStatus = enAppStatus.AppStatusOK) AndAlso (iIndex <= miEnd - miStart) Then
			Dim tValue As TypedValue = New TypedValue(enCodes.String, sValue)
			mtaTypedValue(iIndex + miStart) = tValue
		End If
   End Sub

   Public Function GetHandle(iIndex As Integer, ByRef tValue As Handle) As Boolean

      Dim lHandleValue As Long
      If (miAppStatus = enAppStatus.AppStatusOK) AndAlso (iIndex <= miEnd - miStart) Then
         Dim tTypedValue As TypedValue = mtaTypedValue(iIndex + miStart)
         If tTypedValue.TypeCode = zzShortCode(enCodes.Handle) Then
            Try
               Dim sValue As String = DirectCast(tTypedValue.Value, String)
               lHandleValue = Convert.ToInt64(sValue, 16)
               tValue = New Handle(lHandleValue)
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & tTypedValue.Value.ToString() & vbCrLf & tTypedValue.Value.GetType().ToString() & vbCrLf & tTypedValue.TypeCode.ToString(), "!TplnXData - GetHandle")
               Return False
            End Try
         Else
            Return False
         End If
      Else
         Return False
      End If

   End Function
   Public Sub SetHandle(iIndex As Integer, tValue As Handle)

      If (miAppStatus = enAppStatus.AppStatusOK) AndAlso (iIndex <= miEnd - miStart) Then
         Dim tTypedValue As TypedValue = New TypedValue(enCodes.Handle, tValue)
         mtaTypedValue(iIndex + miStart) = tTypedValue
      End If
   End Sub
	Public Function GetResBuffer() As Autodesk.AutoCAD.DatabaseServices.ResultBuffer
		Try

			Dim oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer = New Autodesk.AutoCAD.DatabaseServices.ResultBuffer()
			oResBuffer.Add(mtAppName)
			oResBuffer.Add(mtAppOpen)
			For iIndex As Integer = 0 To mtaTypedValue.GetUpperBound(0)
				If mtaTypedValue(iIndex).Value Is Nothing Then
					mtaTypedValue(iIndex) = New TypedValue(enCodes.Integer, -1)
				End If
				oResBuffer.Add(mtaTypedValue(iIndex))
			Next
			oResBuffer.Add(mtAppClose)
			'	zzDispBuffer(oResBuffer)
			Return oResBuffer
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "02_214")
			Return Nothing
		End Try

	End Function
	Public Function GetNullResBuffer() As Autodesk.AutoCAD.DatabaseServices.ResultBuffer
		Try

         Dim oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer = New Autodesk.AutoCAD.DatabaseServices.ResultBuffer()
         System.Windows.Forms.MessageBox.Show(mtAppName.ToString(), "20_001")
			oResBuffer.Add(mtAppName)
			'oResBuffer.Add(mtAppOpen)
			'oResBuffer.Add(mtAppClose)

			Return oResBuffer
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "02_215")
			Return Nothing
		End Try

   End Function
   Private Shared Function zzShortCode(iCode As enCodes) As Short
      Return Convert.ToInt16(iCode)
   End Function
	Private Sub zzSetAppName(sXDAppName As String)
		mtAppName = New TypedValue(enCodes.AppName, sXDAppName)
	End Sub
	Private Shared Sub zzDispBuffer(oResBuffer As Autodesk.AutoCAD.DatabaseServices.ResultBuffer)
		Dim mtaTypedValue() As TypedValue = oResBuffer.AsArray()
		Dim sRes As String = ""
		For iIndex As Integer = 0 To mtaTypedValue.GetUpperBound(0)
			sRes &= CStr(mtaTypedValue(iIndex).TypeCode) & ":" & mtaTypedValue(iIndex).Value.ToString() & vbCrLf
		Next
		System.Windows.Forms.MessageBox.Show(sRes, "02_386")
	End Sub
	Private Shared Sub zzDisp(tTypedValue As TypedValue)
		System.Windows.Forms.MessageBox.Show(CStr(tTypedValue.TypeCode) & ":" & tTypedValue.Value.ToString(), "02_321")
	End Sub
End Class
