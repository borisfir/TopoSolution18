Option Explicit On
Option Strict On
Public Class UI_Settings
	Private miResourceTheme As enResourceTheme
	Private miObjectNo As Integer
	Private mdicItemSettingsByIndex As Dictionary(Of Integer, ItemSetting)
	Private mdicItemSettingsByName As Dictionary(Of String, ItemSetting)

	Public Sub New(iResourceTheme As enResourceTheme, iObjectNo As Integer, bByIndex As Boolean, bByName As Boolean)
		miResourceTheme = iResourceTheme
		miObjectNo = iObjectNo
		If bByIndex Then
			mdicItemSettingsByIndex = New Dictionary(Of Integer, ItemSetting)()
		End If

		If bByName Then
			mdicItemSettingsByName = New Dictionary(Of String, ItemSetting)()
		End If

		zzSetDicItemSettings(bByIndex, bByName)
	End Sub

	Public Function GetItem(iIndex As Integer) As ItemSetting
		If mdicItemSettingsByIndex IsNot Nothing Then
			Try
				Return mdicItemSettingsByIndex.Item(iIndex)
			Catch oEx As Exception
				Return New ItemSetting()
			End Try
		Else
			Return New ItemSetting()
		End If
	End Function


	Public Function GetItem(sName As String) As ItemSetting
		If mdicItemSettingsByIndex IsNot Nothing Then
			Try
				Return mdicItemSettingsByName.Item(sName)
			Catch oEx As Exception
				Return New ItemSetting()
			End Try
		Else
			Return New ItemSetting()
		End If
	End Function

	Private Sub zzSetDicItemSettings(bByIndex As Boolean, bByName As Boolean)
		Dim sComText As String = Nothing
		Select Case TPlServerDB.ServerDB.CurrentProjectDB.Provider
			Case TPlServerDB.TPlProvider.ProviderSQLServer
				sComText = "SELECT [ItemNo],[ItemName],[Text],[TextA],[Size] FROM [ProjectData].[dbo].[UI_Info] WHERE ([ResourceTheme]=" & CStr(miResourceTheme) & ") AND ([ObjectNo]=" & CStr(miObjectNo) & ")"
			Case TPlServerDB.TPlProvider.ProviderJet
				sComText = "SELECT [ItemNo],[ItemName],[Text],[TextA],[Size] FROM [UI_Info] WHERE ([ResourceTheme]=" & CStr(miResourceTheme) & ") AND ([ObjectNo]=" & CStr(miObjectNo) & ")"
		End Select

		If sComText IsNot Nothing Then
			Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentProjectDB.GetDataReader(sComText)
			Dim tItemSetting As ItemSetting
			If oDataReader IsNot Nothing Then
				If oDataReader.HasRows Then
					While oDataReader.Read()
						tItemSetting = New ItemSetting(oDataReader)
						If bByIndex Then
							mdicItemSettingsByIndex.Add(tItemSetting.ItemNo, tItemSetting)
						End If
						If bByName Then
							mdicItemSettingsByName.Add(tItemSetting.ItemName, tItemSetting)
						End If

					End While
				End If
				oDataReader.Close()
			End If
		End If
	End Sub
End Class
