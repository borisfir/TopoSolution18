Option Explicit On
Option Strict On
 
Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.DatabaseServices
Public Class frmEditBlockAttr

	Private moAcadBlockRef As BlockReference
	Private mvAttribRefs As AttributeCollection
	Public Property BlockRef() As BlockReference
		Get
			Return moAcadBlockRef
		End Get
		Set(ByVal oValue As BlockReference)
			moAcadBlockRef = oValue
			zzFillAttribData()
		End Set
	End Property



	 

	'   zzFillExtData
	'  zzGetStatus
	'  msSheetCalc = CalcSheet(oNewValue)
	'  zzCheckPoint


	Private Sub zzFillAttribData()
		Dim oAttributeRefObjID As ObjectId
		Dim oAttributeRef As AttributeReference = Nothing
		Dim sValue As String
		'moAcadBlockRef

		mvAttribRefs = DirectCast(moAcadBlockRef.AttributeCollection, AttributeCollection)
		For iIndex As Integer = 0 To mvAttribRefs.Count - 1
			oAttributeRefObjID = mvAttribRefs.Item(iIndex)
			sValue = oAttributeRef.TextString
			If IsNumeric(sValue) Then
				'''''! 	Me("TextBox" & CStr(lIndex + 1)).Text = sValue
			Else
				'''''!	Me("TextBox" & CStr(lIndex + 1)).Text = InvertWX(sValue, False)
			End If
		Next
		oAttributeRef = Nothing
	End Sub

End Class