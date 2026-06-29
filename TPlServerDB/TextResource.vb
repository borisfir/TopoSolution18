Option Explicit On 
Option Strict On


Public Class TextResource
	Private Shared moReadTextResource As ReadTextResource
	Private Shared miLCID As Integer
	Private Enum enResourceStatus
		Active = 0
		Source = -1
	End Enum
   Public Shared Sub Open(Optional ByVal iLCID As Integer = 1037)

      miLCID = iLCID
      moReadTextResource = New ReadTextResource(iLCID)
   End Sub
   Public Shared Sub Close()
      moReadTextResource = Nothing
   End Sub
	Public Shared Function GetText(ByVal iElementID As Integer, ByVal iThemeID As Integer, Optional ByVal iSubThemeID As Integer = 0, Optional ByVal bReturnEmpty As Boolean = False, Optional ByVal iMsgDbg As Integer = 0) As String
		Dim iKey As Integer
		iKey = iElementID + 10000 * iThemeID + 100 * iSubThemeID
		If iMsgDbg <> 0 Then
			System.Windows.Forms.MessageBox.Show(CStr(iKey) & vbCrLf & moReadTextResource.GetText(iKey, bReturnEmpty), CStr(iMsgDbg))
		End If
		'	DMCommon.Debug.ExcelLog.SetNextValue(0, "", iKey, moReadTextResource.GetText(iKey, bReturnEmpty))
		'DMCommon.Debug.MsgBox("Key, moReadText", iKey, moReadTextResource.GetText(iKey, bReturnEmpty))
		Return moReadTextResource.GetText(iKey, bReturnEmpty)
	End Function
	Public Shared Function GetTextArray(ByVal iStartElementID As Integer, ByVal iCount As Integer, ByVal iThemeID As Integer, Optional ByVal iSubThemeID As Integer = 0, Optional ByVal bReturnEmpty As Boolean = False, Optional ByVal iMsgDbg As Integer = 0) As String()
		Dim iKey As Integer
		Dim saResult(iCount - 1) As String
		For iIndex As Integer = 0 To iCount - 1
			iKey = iStartElementID + iIndex + 10000 * iThemeID + 100 * iSubThemeID
			saResult(iIndex) = moReadTextResource.GetText(iKey, bReturnEmpty)
		Next

		If iMsgDbg <> 0 Then
			System.Windows.Forms.MessageBox.Show(CStr(iKey) & vbCrLf & moReadTextResource.GetText(iKey, bReturnEmpty), CStr(iMsgDbg))
		End If
		Return saResult
	End Function
	Public Shared Function GetMsgText(ByVal iMsgTheme As Integer, ByVal iMsgThemeNo As Integer, Optional ByVal saInsertText() As String = Nothing) As String
		Dim iKey As Integer
		Dim sText As String
		iKey = iMsgThemeNo + 100 * iMsgTheme + enResourceTheme.Message
		sText = moReadTextResource.GetText(iKey)
		If Not saInsertText Is Nothing Then
			For iIndex As Integer = 0 To saInsertText.GetUpperBound(0)
				sText = Strings.Replace(sText, "|", saInsertText(iIndex), , 1)
			Next
		End If
		Return sText
	End Function
	Public Shared Function GetMsgText(ByVal iMsgTheme As Integer, ByVal iMsgThemeNo As Integer, ByVal sInsertText As String) As String
		Dim iKey As Integer
		Dim sText As String
		iKey = iMsgThemeNo + 100 * iMsgTheme + 201		'ResourceTheme.Message
		sText = moReadTextResource.GetText(iKey)
		sText = Strings.Replace(sText, "|", sInsertText)
		Return sText
	End Function
	Private Class ReadTextResource
		Inherits System.Collections.DictionaryBase
		Public Sub New(ByVal iLCID As Integer)
			MyBase.New()
			Dim oDataReader As System.Data.Common.DbDataReader
			Dim iKey As Integer
			Dim sText As String

			oDataReader = ServerDB.CurrentServerDB.GetDataReader("SELECT ResKey,Text FROM LocaleResource WHERE (LCID=" & CStr(iLCID) & ") AND (Status=" & CStr(enResourceStatus.Active) & ")")
			While oDataReader.Read()
            iKey = oDataReader.GetInt32(0)
            If oDataReader.IsDBNull(1) Then
               sText = String.Empty
            Else
               sText = oDataReader.GetString(1)
            End If

            MyBase.Dictionary.Add(iKey, sText)
         End While
			oDataReader.Close()

		End Sub
      Public Function GetText(ByVal iKey As Integer, Optional ByVal bReturnEmpty As Boolean = False) As String
         If MyBase.Dictionary.Contains(iKey) Then
				Dim oValue As System.Object = MyBase.Dictionary.Item(iKey)
				If oValue Is Nothing OrElse IsDBNull(oValue) Then
					Return String.Empty
				Else
					Return CStr(oValue)
				End If
         ElseIf bReturnEmpty Then
            Return String.Empty
         Else
				Return CStr(iKey)
			End If
      End Function
	End Class
	Private Class UpdateTextResource
		Inherits System.Collections.DictionaryBase

		Public Sub New()
			MyBase.New()

		End Sub
		Public Sub UpdateText(ByVal iKey As Integer, ByVal sNewText As String)
			With MyBase.Dictionary
				If .Contains(iKey) Then .Remove(iKey)
				.Add(iKey, sNewText)
			End With
		End Sub
	End Class
End Class
