Option Explicit On
Option Strict On
Public Enum enApplications
	Undefined = 0
	Taba = 1
	TopoMaster = 2
	Unidiv = 3
	Parcels = 4
	Ownership = 5
	BN = 6
End Enum
Public Class DMApp
	Public Shared AppID As enApplications = enApplications.Undefined
	Public Shared Function GetDefaultRepScale() As Double
		Select Case AppID
			Case enApplications.Taba
				Return 1000.0
			Case enApplications.TopoMaster
				Return 1000.0
			Case Else
				Return 1000.0
		End Select
	End Function
	Public Shared Function GetDefaultTableStyleName() As String
		Select Case AppID
			Case enApplications.Taba
				Return "Tplanner"
			Case enApplications.TopoMaster
				Return "Bamash"
			Case Else
				Return String.Empty
		End Select

	End Function
	Public Shared Function GetDataGridLineHidden() As Boolean
		Select Case AppID
			Case enApplications.Taba
				Return False
			Case enApplications.TopoMaster
				Return True
			Case Else
				Return False
		End Select
   End Function
	Public Shared Sub MsgBox(ByVal sCaption As String, ByVal ParamArray oParams() As System.Object)
		If DMCommon.Debug.Debug Then
			Dim fMsg As frmMsgBox = New frmMsgBox(DMCommon.Debug.GetMsg(True, oParams))
			fMsg.Text = sCaption
			Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
			Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(oAcadWin, fMsg)
		End If
	End Sub
	Public Shared Sub UseMsgBox(ByVal sCaption As String, ByVal ParamArray oParams() As System.Object)

		Dim fMsg As frmMsgBox = New frmMsgBox(DMCommon.Debug.GetMsg(True, oParams))
		fMsg.Text = sCaption
		Dim oAcadWin As DMAcadExt.AcadWin = New DMAcadExt.AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
		Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(oAcadWin, fMsg)

	End Sub
	Public Shared Function GetList(col As ICollection(Of Autodesk.AutoCAD.DatabaseServices.ObjectId)) As String
      Dim sRes As String = Nothing
      For Each tItem As Autodesk.AutoCAD.DatabaseServices.ObjectId In col
         If sRes Is Nothing Then
            sRes = tItem.ToString()
         Else
            sRes &= "," & tItem.ToString()
         End If
      Next
      Return sRes
   End Function
   Public Shared Function GetListArray(col As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection) As System.Object()
      Dim oaRes(col.Count - 1) As System.Object
      Dim iIndex As Integer = 0
      For Each tItem As Autodesk.AutoCAD.DatabaseServices.ObjectId In col

         oaRes(iIndex) = tItem
         iIndex += 1
      Next
      Return oaRes
   End Function
End Class
