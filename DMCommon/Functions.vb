Option Explicit On
Option Strict On
Imports System.Data
Public Enum dmTriStateAAA
	[False] = 0
	[True] = -1
	UseDefault = -2
	Indeterminate = 2
End Enum
Public Enum TPlProvider
   ProviderNotDefined
   ProviderJet = 1
   ProviderSQLServer = 2
   ProviderOracle = 3
End Enum
Public NotInheritable Class Functions

   Const MOUSE_MOVED As Integer = &H1
   Const MOUSEEVENTF_ABSOLUTE As Integer = &H8000
   Const MOUSEEVENTF_LEFTDOWN As Integer = &H2
   Const MOUSEEVENTF_LEFTUP As Integer = &H4
   Const MOUSEEVENTF_MIDDLEDOWN As Integer = &H20
   Const MOUSEEVENTF_MIDDLEUP As Integer = &H40
   Const MOUSEEVENTF_MOVE As Integer = &H1
   Const MOUSEEVENTF_RIGHTDOWN As Integer = &H8
   Const MOUSEEVENTF_RIGHTUP As Integer = &H10


   Public Declare Function GetActiveWindow Lib "user32" Alias "GetActiveWindow" () As Integer
   Public Declare Function SetActiveWindow Lib "user32" Alias "SetActiveWindow" (ByVal hwnd As Integer) As Integer

   Declare Function GetWindowText Lib "user32" Alias "GetWindowTextA" (ByVal hwnd As Integer, ByVal lpString As String, ByVal cch As Integer) As Integer
   Public Declare Function GetWindowTextLength Lib "user32" Alias "GetWindowTextLengthA" (ByVal hwnd As Integer) As Integer
   Public Declare Function GetFocus Lib "user32" Alias "GetFocus" () As Integer
   Public Declare Function GetInputState Lib "user32" Alias "GetInputState" () As Integer
   Public Declare Function GetWindowDC Lib "user32" Alias "GetWindowDC" (ByVal hwnd As Long) As Integer
   Public Declare Sub mouse_event Lib "user32" Alias "mouse_event" (ByVal dwFlags As Integer, ByVal dx As Integer, ByVal dy As Integer, ByVal cButtons As Integer, ByVal dwExtraInfo As Integer)


	 

   Public Const AcadLWPolylineName As String = "AcadLWPolyline"
	'  Public Const Acad2dPolylineName As String = "AcDb2dPolyline"

	Public Const UnionTopoPrefix As String = "dmUn"

   Public Const AppFile As String = " "
   Public Const AppName As String = "TownPlanner"
   Public Const SettingSectionName As String = "Settings"
   Public Const DBName As String = "tblData.mdb"
  
   Public Shared Function GetActiveWinText() As String
      Dim iHWnd As Integer = GetActiveWindow()
      Dim iWinTextLen As Integer = GetWindowTextLength(iHWnd)
      Dim cch As Integer = 255
      Dim sBuffer As String = Strings.Space(255)
      Dim iResp As Integer = GetWindowText(iHWnd, sBuffer, cch)
      Dim sOut As String = Left(sBuffer, iWinTextLen)
      Return sOut
   End Function
   Public Shared Sub MouseClick()
      mouse_event(MOUSEEVENTF_LEFTDOWN Or MOUSEEVENTF_LEFTUP, 0, 0, 0, 0)


   End Sub
	Public Shared Function GetPlural(ByVal iCount As Integer, Optional ByVal sExt As String = "") As String
		Const sDefaultExt As String = "s"
		If iCount = 1 Then
			Return String.Empty
		ElseIf sExt.Length = 0 Then
			Return sDefaultExt
		Else
			Return sExt
		End If

   End Function
   Public Shared Function GetErrorText(ByVal iErrNumber As Integer, ParamArray saParams() As String) As String
      Dim sSettingname As String = "Err_" & Convert.ToString(iErrNumber)
      Dim sText As String
      ' sText = My.MySettings.Default.Err_1101
      sText = CType(My.Settings(sSettingname), String)
      For iIndex As Integer = 0 To saParams.GetUpperBound(0)
         sText = Replace(sText, "|", saParams(iIndex))
      Next
      Return sText
   End Function
   Friend Shared Function GetConnectionString(ByVal iProvider As TPlProvider) As String
      Dim sProviderName As String
      Dim sDataSource As String = Space(0)
      sProviderName = GetProviderName(iProvider)
      Select Case iProvider
         Case TPlProvider.ProviderJet
            Return "Jet OLEDB:Global Partial Bulk Ops=2;Jet OLEDB:Registry Path=;Jet OLEDB:Database Locking Mode=0;Data Source=" & sDataSource & ";Jet OLEDB:Engine " & _
            "Type=5;Provider=""" & sProviderName & """;Jet OLEDB:System database=;Jet OLEDB:SFP=False;persist security info=False;Extended Properties=;Mode=Share Deny None;" & _
            "Jet OLEDB:Encrypt Database=False;Jet OLEDB:Create System Database=False;Jet OLEDB:" & _
            "Don't Copy Locale on Compact=False;Jet OLEDB:Compact Without Replica Repair=False;" & _
            "User ID=Admin;Jet OLEDB:Global Bulk Transactions=1"
         Case TPlProvider.ProviderSQLServer
            Return "Persist Security Info=False;User ID=sa;Password=bf2679;Initial Catalog=ShopData;Data Source=DS;Packet Size=4096;Workstation ID=DM208"
         Case TPlProvider.ProviderOracle
            Return Space(0)
         Case Else
            Return Space(0)

      End Select
   End Function
   Friend Shared Function GetProviderName(ByVal iProvider As TPlProvider) As String
      Select Case iProvider
         Case TPlProvider.ProviderJet
            Return "Microsoft.Jet.OLEDB.4.0"
         Case TPlProvider.ProviderSQLServer
            Return Space(0)
         Case TPlProvider.ProviderOracle
            Return Space(0)
         Case Else
            Return Space(0)
      End Select
   End Function
	Friend Shared Function GetAppTypeAAA(ByVal iOdbcType As Odbc.OdbcType) As System.Type
		Select Case iOdbcType
			Case Odbc.OdbcType.Int
				Return System.Type.GetType("System.Int32")
			Case Odbc.OdbcType.Double
				Return System.Type.GetType("System.Double")
			Case Odbc.OdbcType.Text
				Return System.Type.GetType("System.String")
			Case Odbc.OdbcType.Bit
				Return System.Type.GetType("System.Boolean")

			Case Else
				Return Nothing
		End Select
	End Function

	Public Shared Sub Mid(ByRef sTarget As String, iStartPos As Integer, iLength As Integer, sReplace As String)
		Dim sStart As String
		Dim sEnd As String

		If iStartPos > 1 Then

			sStart = sTarget.Substring(0, iStartPos - 1)
		Else
			sStart = String.Empty
		End If


		If iStartPos - 1 + iLength <= sTarget.Length AndAlso iStartPos - 1 + iLength >= 0 Then
			sEnd = sTarget.Substring(iStartPos - 1 + iLength)
		Else
			sEnd = String.Empty
		End If
		sTarget = sStart & sReplace & sEnd
	End Sub
	Public Shared Sub ShowEx(ByVal oEx As Exception, Optional ByVal sTitle As String = "", Optional ByVal sAddMsg As String = "")
		Dim sMsg As String = oEx.Message & vbCrLf & oEx.StackTrace
		If sAddMsg.Length <> 0 Then
			sMsg &= vbCrLf & sAddMsg
		End If
		System.Windows.Forms.MessageBox.Show(sMsg, sTitle)
	End Sub
	'Dim sTest As String
	Public Shared Function GetLocalFileNameInEmptyDir(sFileName As String, sExtension As String) As String
		Dim iAdd As Integer = 0
		Dim sResFileName As String
		Do
			sResFileName = zzGetLocalFileNameInEmptyDir(sFileName, sExtension, iAdd)
			If sResFileName.Length <> 0 Then
				Return sResFileName
			End If
         DMCommon.Debug.MsgBox("02_444", sFileName & vbCrLf & CStr(iAdd))
			iAdd += 1
			If iAdd = 1000 Then
				Exit Do
			End If
		Loop
		Return String.Empty
	End Function
	Private Shared Function zzGetLocalFileNameInEmptyDir(sFileName As String, sExtension As String, iAdd As Integer) As String
		Dim oDirectoryInfo As IO.DirectoryInfo
		Dim sAdd As String
		If iAdd = 0 Then
			sAdd = String.Empty
		Else
			sAdd = "_" & CStr(iAdd)
		End If
		oDirectoryInfo = New IO.DirectoryInfo(System.Windows.Forms.Application.LocalUserAppDataPath & "\" & sFileName & sAdd)
		'	System.Windows.Forms.MessageBox.Show(sFileName & vbCrLf & oDirectoryInfo.FullName & vbCrLf & oDirectoryInfo.Exists, "09_000")
		If oDirectoryInfo.Exists Then
			If Not zzClearDir(oDirectoryInfo) Then
				Return String.Empty
			End If
		Else
			oDirectoryInfo.Create()
		End If
		'	System.Windows.Forms.MessageBox.Show(oDirectoryInfo.FullName & "\" & sFileName & sExtension, "09_111")
		Return oDirectoryInfo.FullName & "\" & sFileName & sExtension
	End Function
	Private Shared Function zzClearDir(oDirectoryInfo As IO.DirectoryInfo) As Boolean
		Dim oaFiles() As System.IO.FileInfo = oDirectoryInfo.GetFiles()
		If oaFiles IsNot Nothing Then
			Try
				'System.Windows.Forms.MessageBox.Show(CStr(oaFiles.GetUpperBound(0)) & vbCrLf & oDirectoryInfo.FullName, "06_293")
				For iIndex As Integer = 0 To oaFiles.GetUpperBound(0)
					oaFiles(iIndex).Delete()
				Next
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oDirectoryInfo.FullName, "06_288")
				Return False
			End Try
		End If
		Return True
	End Function
	Public Shared Function GetLocalFileNameInDir(sFileName As String, sExtension As String, Optional iAdd As Integer = 0) As String
		Dim oDirectoryInfo As IO.DirectoryInfo
		Dim sAdd As String
		If iAdd = 0 Then
			sAdd = String.Empty
		Else
			sAdd = "_" & CStr(iAdd)
		End If
		oDirectoryInfo = New IO.DirectoryInfo(System.Windows.Forms.Application.LocalUserAppDataPath & "\" & sFileName & sAdd)


		'	System.Windows.Forms.MessageBox.Show(sFileName & vbCrLf & oDirectoryInfo.FullName, "06_100")
		If Not oDirectoryInfo.Exists Then
			oDirectoryInfo.Create()
		End If

		Return oDirectoryInfo.FullName & "\" & sFileName & sExtension
	End Function
	Public Shared Function TestType(ByVal oObject As System.Object) As String
		If oObject Is Nothing Then
			Return "IsNothing"
		Else
			Return oObject.GetType().ToString()
		End If
	End Function
	Public Shared Function NameSplit(ByVal sName As String) As String()
		Return Split(sName, "_", 2)
	End Function
	Public Shared Function StringToIntArray(ByVal sValue As String, Optional sDelim As String = ",") As Integer()
		If Not String.IsNullOrEmpty(sValue) Then
			Dim saValue() As String = Strings.Split(sValue, sDelim)
			Dim iValue As Integer
			Dim iRes(saValue.GetUpperBound(0)) As Integer
			For iIndex As Integer = 0 To saValue.GetUpperBound(0)
				If Integer.TryParse(saValue(iIndex), iValue) Then
					iRes(iIndex) = iValue
				Else
					Return Nothing
				End If
			Next
			Return iRes
		Else
			Return Nothing
		End If



	End Function
	Public Shared Function GetDeviation(dBaseValue As Double, dValue As Double) As Double
		Return Math.Abs((dValue - dBaseValue) / dBaseValue)
	End Function
	Public Shared Function CheckDeviation(dBaseValue As Double, dValue As Double, ByVal dTolerance As Double) As Boolean
		Return (Math.Abs((dValue - dBaseValue) / dBaseValue)) <= dTolerance
	End Function
	Public Shared Function CheckAbsDeviation(dBaseValue As Double, dValue As Double, ByVal dTolerance As Double) As Boolean
		Return (Math.Abs(dValue - dBaseValue)) <= dTolerance
	End Function
	Public Shared Function TextToScale(ByVal sTextScale As String, ByVal dDefaultScale As Double) As Double
		Dim dScale As Double = 0.0
		Dim sScale As String = String.Empty

		If sTextScale IsNot Nothing AndAlso sTextScale.Length <> 0 Then
			sScale = Strings.Mid(sTextScale, 3)
			If IsNumeric(sScale) Then
				dScale = Convert.ToDouble(sScale)
			Else
				System.Windows.Forms.MessageBox.Show(sScale, "16_274")
				dScale = 2200
			End If
		Else
			dScale = dDefaultScale
		End If
		Return dScale
	End Function
	Public Shared Sub Disp2Arrays(ByVal iaValue() As Integer, ByVal saValue() As String, ByVal sTitle As String)
		Dim sMsg As String = String.Empty
		Dim iUB As Integer = Math.Min(iaValue.GetUpperBound(0), saValue.GetUpperBound(0))
		Try
			For iIndex As Integer = 0 To iUB
				If iIndex <> 0 Then
					sMsg &= vbCrLf
				End If
				sMsg &= iaValue(iIndex).ToString & " : " & CStrN(saValue(iIndex))
			Next
		Catch oEx As System.Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "Common - DispDblArray")
		End Try
		System.Windows.Forms.MessageBox.Show(sMsg, sTitle)

	End Sub

	Public Shared Sub DispArray(ByVal sTitle As String, ByVal daValue() As Double)
		If Debug.Debug Then
			Dim sMsg As String = String.Empty
			Try
				For iIndex As Integer = 0 To daValue.GetUpperBound(0)
					If iIndex <> 0 Then
						sMsg &= vbCrLf
					End If
					sMsg &= daValue(iIndex).ToString
				Next
			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "Common - DispDblArray")
			End Try
			System.Windows.Forms.MessageBox.Show(sMsg, sTitle)
		End If
	End Sub

	Public Shared Sub DispArray(ByVal sTitle As String, ByVal iaValue() As Integer)
		If Debug.Debug Then
			Dim sMsg As String = String.Empty
			If iaValue Is Nothing Then
				sMsg = "Array is Nohing"
			Else
				Try
					For iIndex As Integer = 0 To iaValue.GetUpperBound(0)
						If iIndex <> 0 Then
							sMsg &= vbCrLf
						End If
						sMsg &= iaValue(iIndex).ToString
					Next
				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "Common - DispIntArray")
				End Try
			End If

			System.Windows.Forms.MessageBox.Show(sMsg, sTitle)
		End If
	End Sub
	Public Shared Sub DispArray(ByVal iaValue() As Integer, ByVal sTitle As String)
		If Debug.Debug Then
			Dim sMsg As String = String.Empty
			If iaValue Is Nothing Then
				sMsg = "Array is Nohing"
			Else
				Try
					For iIndex As Integer = 0 To iaValue.GetUpperBound(0)
						If iIndex <> 0 Then
							sMsg &= vbCrLf
						End If
						sMsg &= iaValue(iIndex).ToString
					Next
				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "Common - DispIntArray")
				End Try
			End If

			System.Windows.Forms.MessageBox.Show(sMsg, sTitle)
		End If
	End Sub
	Public Shared Sub DispArray(ByVal laValue() As Long, ByVal sTitle As String)
      If Debug.Debug Then
         Dim sMsg As String = String.Empty
         If laValue Is Nothing Then
            sMsg = "Array is Nohing"
         Else
            Try
               For iIndex As Integer = 0 To laValue.GetUpperBound(0)
                  If iIndex <> 0 Then
                     sMsg &= vbCrLf
                  End If
                  sMsg &= laValue(iIndex).ToString
               Next
            Catch oEx As System.Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "Common - DispIntArray")
            End Try
         End If

         System.Windows.Forms.MessageBox.Show(sMsg, sTitle)
      End If
   End Sub
	Public Shared Function DispArray(ByVal sTitle As String, ByVal colValues As System.Collections.Generic.ICollection(Of String), ByVal bMsgBox As Boolean, Optional sDelim As String = vbCrLf) As String
		If Debug.Debug Then
			Dim sMsg As String = String.Empty
			If colValues Is Nothing Then
				sMsg = "Array Is Nothing"
			Else
				Try
					For Each sItem As String In colValues
						If sMsg.Length <> 0 Then
							sMsg &= sDelim
						End If
						sMsg &= sItem
					Next

				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "Common-DispStrArray")
				End Try
			End If
			If bMsgBox Then
				System.Windows.Forms.MessageBox.Show(sMsg, sTitle)
			End If
			Return sMsg
		Else
			Return String.Empty
		End If
	End Function
	Public Shared Function DispArray(ByVal saValue() As String, ByVal sTitle As String, ByVal bMsgBox As Boolean, Optional sDelim As String = vbCrLf) As String
      If Debug.Debug Then
         Dim sMsg As String = String.Empty
         If saValue Is Nothing Then
            sMsg = "Array Is Nothing"
         Else
            Try
               For iIndex As Integer = 0 To saValue.GetUpperBound(0)
                  If iIndex <> 0 Then
                     sMsg &= sDelim
                  End If
                  sMsg &= saValue(iIndex)
               Next
            Catch oEx As System.Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "Common-DispStrArray")
            End Try
         End If
         If bMsgBox Then
            System.Windows.Forms.MessageBox.Show(sMsg, sTitle)
         End If
         Return sMsg
      Else
         Return Nothing
      End If
   End Function

	Public Shared Function DispArray(ByVal sTitle As String, ByVal oaValue() As System.Object, ByVal bMsgBox As Boolean) As String
		Dim sMsg As String = String.Empty

		If Debug.Debug Then
			If oaValue Is Nothing Then
				sMsg = "Array Is Nothing"
			Else
				Try
					For iIndex As Integer = 0 To oaValue.GetUpperBound(0)
						If iIndex <> 0 Then
							sMsg &= vbCrLf
						End If
						If oaValue(iIndex) Is Nothing Then
							sMsg &= "-"
						ElseIf IsDBNull(oaValue(iIndex)) Then
							sMsg &= "!DBNull"
						ElseIf String.IsNullOrEmpty(oaValue(iIndex).ToString()) Then
							sMsg &= "!NullOrEmpty"
						Else

							sMsg &= oaValue(iIndex).ToString
						End If

					Next
				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "Common-DispStrArray")
				End Try
			End If
			If bMsgBox Then
				System.Windows.Forms.MessageBox.Show(sMsg, sTitle)
			End If

		End If
		Return sMsg
	End Function
	Public Shared Function DispArrayN(ByVal oaValue() As System.Object, ByVal sTitle As String, ByVal bMsgBox As Boolean) As String
		Dim sMsg As String = String.Empty

		If Debug.Debug Then
			If oaValue Is Nothing Then
				sMsg = "Array Is Nothing"
			Else
				Try
					sMsg = "***" & CStr(oaValue.GetUpperBound(0) + 1) & "***"
					For iIndex As Integer = 0 To oaValue.GetUpperBound(0)
						sMsg &= vbCrLf

						If oaValue(iIndex) Is Nothing Then
							sMsg &= "-"
						Else
							sMsg &= oaValue(iIndex).ToString
						End If

					Next
				Catch oEx As System.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message, "Common-DispStrArray")
				End Try
			End If
			If bMsgBox Then
				System.Windows.Forms.MessageBox.Show(sMsg, sTitle)
			End If

		End If
		Return sMsg
	End Function
	Public Shared Function DispDataTableCols(ByVal oDataTable As DataTable, ByVal sTitle As String, ByVal bMsgBox As Boolean) As String
		Dim oCol As DataColumn
		Dim sMsg As String = String.Empty
		For iIndex As Integer = 0 To oDataTable.Columns.Count - 1
			oCol = oDataTable.Columns.Item(iIndex)
			If sMsg.Length <> 0 Then
				sMsg &= vbCrLf
			End If
			sMsg &= oCol.ColumnName
		Next
		If bMsgBox Then
			System.Windows.Forms.MessageBox.Show(sMsg, sTitle)
		End If
		Return sMsg
	End Function
	Public Shared Function GetFirstStrFromList(sList As String) As String
		If Not String.IsNullOrEmpty(sList) Then
			Dim saVal() As String = Split(sList, ",")
			Return saVal(0)
		Else
			Return String.Empty
		End If
	End Function
	Public Shared Function RelRound(dValue As Double, iDigits As Integer) As Double
		Dim dFactor As Double = 1.0
		Dim iL As Integer = 0
		For iIndex As Integer = 1 To iDigits
			If dValue > dFactor Then
				iL = iIndex
			End If
			dFactor *= 10
		Next
		If iL <> 0 Then
			Return Math.Round(dValue, iDigits - iL)
		Else
			dFactor = 0.1
			For iIndex As Integer = 1 To 6
				If dValue > dFactor Then
					iL = iIndex
					Exit For
				End If
				dFactor *= 0.1
			Next
			Return Math.Round(dValue, iDigits + iL)
		End If
	End Function
	Public Shared Function InList(ByVal sList As String, ByVal sFind As String, Optional ByVal sDelim As String = ",", Optional ByVal bIgnoreCase As Boolean = False) As Boolean
		Dim iCompareMethod As CompareMethod
		sFind = sDelim & sFind & sDelim
		sList = sDelim & sList & sDelim
		If bIgnoreCase Then
			iCompareMethod = CompareMethod.Text
		Else
			iCompareMethod = CompareMethod.Binary
		End If
		Return InStr(sList, sFind, iCompareMethod) <> 0
	End Function
	Public Shared Function ObjToString(ByVal oValue As System.Object, Optional ByVal sDefaultValue As String = "") As String
		If oValue Is Nothing OrElse IsDBNull(oValue) Then
			Return sDefaultValue
		Else
			Return oValue.ToString()
		End If

	End Function
   Public Shared Function IntToString(ByVal iValue As System.Int32, Optional ByVal iDefaultValue As Integer = 0) As String
      If iValue = iDefaultValue Then
         Return String.Empty
      Else
         Return Convert.ToString(iValue)
      End If
   End Function
	Public Shared Function ValToString(ByVal oValue As System.Object, Optional ByVal sDefaultValue As String = "") As String
		If oValue Is Nothing OrElse IsDBNull(oValue) Then
			Return sDefaultValue
		Else
			Return oValue.ToString
		End If
	End Function
	Public Shared Function CStrN(ByVal oValue As System.Object, Optional ByVal sDefaultValue As String = "") As String
		If oValue Is Nothing OrElse IsDBNull(oValue) Then
			Return sDefaultValue
		Else
			Return DirectCast(oValue, String)
		End If

	End Function
	Public Shared Function CStrN(ByVal sValue As String, Optional ByVal sDefaultValue As String = "") As String
		If sValue Is Nothing Then
			Return sDefaultValue
		Else
			Return sValue
		End If

	End Function

	Public Shared Function CStrN(ByRef oReader As System.Data.Common.DbDataReader, ByVal iFieldIndex As Integer, Optional ByVal sDefaultValue As String = "") As String
		If oReader Is Nothing OrElse oReader.IsDBNull(iFieldIndex) Then
			Return sDefaultValue
		Else
			Return oReader.GetString(iFieldIndex)
		End If

	End Function

	Public Shared Function JoinString(sVal1 As String, sVal2 As String, Optional sDelim As String = ",") As String
		If String.IsNullOrEmpty(sVal2) Then
			Return sVal1
		ElseIf String.IsNullOrEmpty(sVal1) Then
			Return sVal2
		Else
			Return sVal1 & sDelim & sVal2
		End If
	End Function
	Public Shared Function CBoolN(ByVal oValue As System.Object, Optional ByVal bDefaultValue As Boolean = False) As Boolean
		If oValue Is Nothing OrElse IsDBNull(oValue) Then
			Return bDefaultValue
		Else
			Try
				Return DirectCast(oValue, Boolean)  'DirectCast
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oValue.ToString(), "Functions - CDblN")
				Return False
			End Try

		End If
	End Function

	Public Shared Function CDblN(ByVal oValue As System.Object, Optional ByVal dDefaultValue As Double = 0.0) As Double
		If oValue Is Nothing OrElse IsDBNull(oValue) Then
			Return dDefaultValue
		Else
			Try
				Return CType(oValue, Double)  'DirectCast
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oValue.ToString(), "Functions - CDblN")
				Return 0.0
			End Try

		End If
	End Function
	Public Shared Function CDblToIntN(ByVal oValue As System.Object, dFactor As Double, Optional ByVal iDefaultValue As Integer = 0) As Integer
		If oValue Is Nothing OrElse IsDBNull(oValue) Then
			Return iDefaultValue
		Else
			Try

				Return CInt(DirectCast(oValue, Double) * dFactor)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oValue.ToString(), "Functions - CDblToIntN")
				Return 0
			End Try

		End If
	End Function
	Public Shared Function CDblN(ByRef oReader As System.Data.Common.DbDataReader, ByVal iFieldIndex As Integer, Optional ByVal dDefaultValue As Double = 0.0) As Double
		If oReader Is Nothing OrElse oReader.IsDBNull(iFieldIndex) Then
			Return dDefaultValue
		Else
			Try
				Return oReader.GetDouble(iFieldIndex)  'DirectCast
			Catch oEx As Exception
				DMCommon.Debug.UserMsg("Functions - CDblN", oEx.Message, iFieldIndex, oReader.GetValue(iFieldIndex))
				Return 0.0
			End Try

		End If
	End Function

	Public Shared Function CIntN(ByVal oValue As System.Object, Optional ByVal iDefaultValue As Integer = 0) As Integer
		If oValue Is Nothing OrElse IsDBNull(oValue) Then
			Return iDefaultValue
		Else
			Try
				Return DirectCast(oValue, Integer)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oValue.GetType().ToString() & vbCrLf & oValue.ToString(), "Functions - CIntN")
			End Try

		End If

	End Function

	Public Shared Function CEnumN(Of TEnum As Structure)(oValue As System.Object, ByVal iDefaultValue As TEnum) As TEnum
		If oValue Is Nothing OrElse IsDBNull(oValue) OrElse Not [Enum].IsDefined(GetType(TEnum), oValue) Then
			DMCommon.Debug.ExcelLog.SetNextValue(0, "!oValue", oValue)
			Return iDefaultValue
		Else
			Try
				'Return CType(oValue, TEnum)
				Return DirectCast(oValue, TEnum)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oValue.GetType().ToString() & vbCrLf & oValue.ToString(), "Functions - CEnumN")
			End Try


		End If
	End Function

	Public Shared Function JoinInt(ByVal iaValue() As Integer) As String
      Dim iIndex As Integer
      Dim iValue As Integer
      Dim sOut As String = String.Empty
      For iIndex = 0 To iaValue.GetUpperBound(0)
         iValue = iaValue(iIndex)
         If sOut.Length <> 0 Then sOut += ","
         sOut += iValue.ToString
      Next
      Return sOut

   End Function

	Public Shared Function GetComplexName(iMain As Integer, iAdditional As Integer, sDelim As String) As String
		If iMain > 0 Then
			Dim sRes As String = Convert.ToString(iMain)
			If iAdditional <> 0 AndAlso sDelim IsNot Nothing Then
				sRes &= sDelim & Convert.ToString(iAdditional)

			End If
			Return sRes
		Else
			Return String.Empty
		End If

	End Function
	Public Shared Function DecomposeComplexName(sComplexName As String, sDelim As String, ByRef iMain As Integer, ByRef iAdditional As Integer) As Boolean
		Dim bSuccess As Boolean
		If Not String.IsNullOrEmpty(sComplexName) Then
			Dim saVal() As String = Split(sComplexName, sDelim)

			If Not (saVal.GetUpperBound(0) > 0 AndAlso Integer.TryParse(saVal(1).Trim(), iAdditional)) Then
				iAdditional = 0
			End If
			If Integer.TryParse(saVal(0).Trim(), iMain) Then
				bSuccess = True
			Else
				bSuccess = False
			End If
		Else
			bSuccess = False
		End If

		Return bSuccess
	End Function


	Public Shared Function CLngN(ByVal oValue As System.Object, Optional ByVal iDefaultValue As Long = 0L) As Long
      If oValue Is Nothing OrElse IsDBNull(oValue) Then
         Return iDefaultValue
      Else
         Return DirectCast(oValue, Long)
      End If

   End Function
	Public Shared Function CIntC(ByVal dValue As System.Double) As Integer
		Dim dIntMax As Double = Integer.MaxValue
		Dim dIntMin As Double = Integer.MinValue
		If dValue <= dIntMax AndAlso dValue >= dIntMin Then
			Return CInt(dValue)
		ElseIf dValue > 0 Then
			Return Integer.MaxValue
		Else
			Return Integer.MinValue
		End If
   End Function
   Public Shared Function CDateN(ByVal oValue As System.Object) As Date
      If oValue Is Nothing OrElse IsDBNull(oValue) Then
         Return Date.MinValue
      Else
         Return DirectCast(oValue, Date)
      End If

   End Function
	Public Shared Function StringToSQL(ByVal sValue As String) As String
		Return Strings.Replace(Strings.Replace(sValue, "'", "''"), """", """")	'""""""
	End Function
 
End Class
