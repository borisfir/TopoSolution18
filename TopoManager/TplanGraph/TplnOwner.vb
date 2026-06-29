Option Explicit On
Option Strict On
Imports System.Data
Namespace TPlanGraph
	Public Enum enDisplayType
		Undefined
		HebLetters
		Numbers
	End Enum
	Public Class TplnOwner
		Public Const OwnerPgonMax As Integer = 21
		Public Shared DisplayType As enDisplayType
		'	Private Shared mbaAreaExists(OwnerPgonMax) As Boolean
		Private Shared miOwnerPgonUB As Integer
		'	Private Shared mdic As SortedDictionary(Of Integer, TplnOwner)
		Private Shared mdicOwners As IDictionary(Of Integer, TplnOwner) = New SortedDictionary(Of Integer, TplnOwner)
		Private miOwnerID As Integer
		Private msOwnerName As String
		Private mbIsPublic As Boolean
		'	Private mdaArea(OwnerPgonMax) As Double
		Private moaOwnerPgon(OwnerPgonMax) As TplnOwnerPgon

		Private mdAreaSum As Double
		Private mdPublicSum As Double
		Public Shared Sub Init()
			mdicOwners = New SortedDictionary(Of Integer, TplnOwner)
		End Sub
		Public Shared Sub AddOwnerPgon(oOwnerPgon As TplnOwnerPgon)
			Dim tData As OwnerPgonData = oOwnerPgon.Data
			Dim iOwnerID As Integer = tData.OwnerID
			Dim bIsPublic As Boolean = tData.IsPublic

			If iOwnerID > 0 AndAlso (tData.IsPublic OrElse tData.OwnerPgonNo >= 0) Then
				Dim sOwnerName As String = tData.OwnerName
				Dim oOwner As TplnOwner = Nothing
				If Not mdicOwners.TryGetValue(iOwnerID, oOwner) Then
					oOwner = New TplnOwner(iOwnerID, sOwnerName, bIsPublic)
					mdicOwners.Add(iOwnerID, oOwner)
				ElseIf oOwner.OwnerName Is Nothing Then
					oOwner.OwnerName = sOwnerName
				End If

				If oOwner.IsPublic <> bIsPublic Then
					Dim sMsgText As String = "מס' משק " & CStr(iOwnerID)
					sMsgText &= vbCrLf & tData.OwnerPgonLetter
					MessageBox.Show(sMsgText, "Error")
					DMAcadExt.AppMessages.AddMessage(True, oOwnerPgon.CenterPosition, oOwnerPgon.BoundingBox, "", sMsgText, False)
				End If
				oOwner.AddPgon(oOwnerPgon)
			End If
		End Sub
		Public Sub New(iID As Integer, sName As String, bIsPublic As Boolean)
			miOwnerID = iID
			msOwnerName = sName
			mbIsPublic = bIsPublic
		End Sub

		Public Shared ReadOnly Property OwnerPgonUB As Integer
			Get
				Return miOwnerPgonUB
			End Get
		End Property
		Public Property OwnerName As String
			Get
				Return msOwnerName
			End Get
			Set(sValue As String)
				msOwnerName = sValue
			End Set
		End Property
		Public ReadOnly Property IsPublic As Boolean
			Get
				Return mbIsPublic
			End Get
		End Property
		Public ReadOnly Property OwnerID As Integer
			Get
				Return miOwnerID
			End Get
		End Property
		Public ReadOnly Property PublicSum As Double
			Get
				Return mdPublicSum
			End Get
		End Property
		Public ReadOnly Property AreaSum As Double
			Get
				Return mdAreaSum + mdPublicSum
			End Get
		End Property
		Public Shared Function GetView() As DataView
			Const sOwnerNameFieldName As String = "OwnerName"
			Const sOwnerIDFieldName As String = "OwnerID"
			Const sOwnerAreaFieldName As String = "OwnerArea"

			Dim oOwnerTable As DataTable = New DataTable("Owners")
			Dim oColumn As DataColumn
			Dim oNewRow As System.Data.DataRow
			Dim dArea As Double
			With oOwnerTable.Columns
				.Add(sOwnerNameFieldName, GetType(System.String))
				.Add(sOwnerIDFieldName, GetType(System.String))
				.Add(sOwnerAreaFieldName, GetType(System.String))

				For iIndex As Integer = 0 To miOwnerPgonUB

					oColumn = .Add("F" & Convert.ToString(iIndex), GetType(System.Double))
					oColumn.Caption = "א" & Convert.ToString(iIndex)
               TplnProject.WriteMessageBox("oColumn.Caption: " & oColumn.Caption.ToString(), "AWEZ")
				Next
			End With
			'	MessageBox.Show(CStr(mdicOwners.Count), "01_530c")
			For Each oOwner As TplnOwner In mdicOwners.Values
				oNewRow = oOwnerTable.NewRow()
				With oNewRow
					.Item(sOwnerNameFieldName) = oOwner.OwnerName
					.Item(sOwnerIDFieldName) = oOwner.OwnerID
					.Item(sOwnerAreaFieldName) = oOwner.AreaSum
					If True OrElse Not oOwner.IsPublic Then
						For iIndex As Integer = 0 To miOwnerPgonUB
							dArea = oOwner.PgonArea(iIndex)
							'If mbaAreaExists(iIndex) Then
							If dArea <> 0.0 Then
								.Item("F" & Convert.ToString(iIndex)) = dArea
							End If
							'	End If
						Next
					End If
				End With
				oOwnerTable.Rows.Add(oNewRow)
			Next

			Return New DataView(oOwnerTable)
		End Function

		Public Shared Function GetCaptions(bWin As Boolean) As String()
			Dim saCaptions(miOwnerPgonUB) As String
			Dim sRes As String

			If DisplayType = enDisplayType.Undefined Then
				DisplayType = enDisplayType.HebLetters
			End If
			If DisplayType = enDisplayType.HebLetters Then


				For iIndex As Integer = 0 To miOwnerPgonUB
					If bWin Then
						sRes = ChrW(iIndex + AscW("א"))

					Else
						sRes = Chr(128 + iIndex)
					End If
					saCaptions(miOwnerPgonUB - iIndex) = sRes
				Next
				Return saCaptions
			ElseIf DisplayType = enDisplayType.Numbers Then
				For iIndex As Integer = 0 To miOwnerPgonUB
					sRes = CStr(iIndex + 1)
					saCaptions(miOwnerPgonUB - iIndex) = sRes
				Next
				Return saCaptions
			Else
				Return Nothing
			End If

		End Function
		Public ReadOnly Property PgonArea(iIndex As Integer) As Double
			Get
				If iIndex <= OwnerPgonMax Then
					'Return mdaArea(iIndex)
					If moaOwnerPgon(iIndex) IsNot Nothing Then
						Return moaOwnerPgon(iIndex).RoundedArea
					Else
						Return 0.0
					End If

				Else
					Return -1.0
				End If

			End Get
		End Property
		Public Sub AddPgon(oOwnerPgon As TplnOwnerPgon)
			Dim tData As OwnerPgonData = oOwnerPgon.Data
			If tData.IsPublic Then
				mdPublicSum += oOwnerPgon.RoundedArea
			Else
				Dim iPgonNo As Integer = tData.OwnerPgonNo
				If iPgonNo <= OwnerPgonMax Then
					'	If False AndAlso mdaArea(iPgonNo) = 0.0 Then
					'mdaArea(iPgonNo) = oOwnerPgon.RoundedArea
					'mbaAreaExists(iPgonNo) = True
					'If miOwnerPgonUB < iPgonNo Then
					'miOwnerPgonUB = iPgonNo
					'End If
					'	mdAreaSum += oOwnerPgon.RoundedArea

					'	End If
               If iPgonNo > 1 Then
                  '  MessageBox.Show(iPgonNo.ToString(), "05_341z")
               End If
					If moaOwnerPgon(iPgonNo) Is Nothing Then
						moaOwnerPgon(iPgonNo) = oOwnerPgon
						If miOwnerPgonUB < iPgonNo Then
							miOwnerPgonUB = iPgonNo
						End If
						mdAreaSum += oOwnerPgon.RoundedArea
					Else
						Dim sMsgText As String = "הפוליגון כבר קיים" & vbCrLf & "מס' משק " & CStr(miOwnerID) & vbCrLf & "שם " & CStr(oOwnerPgon.Data.OwnerPgonLetter)
						MessageBox.Show(sMsgText, "TplnOwner - AddPgon")
						DMAcadExt.AppMessages.AddMessage(True, moaOwnerPgon(iPgonNo).CenterPosition, moaOwnerPgon(iPgonNo).BoundingBox, "", sMsgText, False)
						DMAcadExt.AppMessages.AddMessage(True, oOwnerPgon.CenterPosition, oOwnerPgon.BoundingBox, "", sMsgText, False)

					End If
				Else
					DMAcadExt.AcadDocument.WriteMessage("05_037: " & CStr(iPgonNo))
				End If
			End If
			'	MessageBox.Show(CStr(miOwnerPgonUB), "07_200")

		End Sub
	End Class
End Namespace
