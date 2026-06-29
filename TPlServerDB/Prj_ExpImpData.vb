Option Explicit On
Option Strict On
Imports System.Data
Public Class Prj_ExpImpData
   Inherits TPlServerDB.ExpImpData





   Const msPrjMapThemesSPName As String = "GetPrjMapThemes"
   Const msPrjDetailsSPName As String = "GetPrjDetails"
   Const msPrjParamsSPName As String = "GetPrjParams"
   '  Const msProjectDataSPName As String = "GetProjectData"

   Private Shared msaTableName() As String = {"PrjMapThemes", "PrjDetails", "PrjParams"}
   Private Shared msaSPName() As String = {msPrjMapThemesSPName, msPrjDetailsSPName, msPrjParamsSPName}
   Public Sub New(iProjectCode As Integer, iDetailNo As Integer)

      MyBase.New(TPlServerDB.ServerDB.CurrentServerDB, iProjectCode, iDetailNo, msaTableName, msaSPName)
   End Sub
   Public Sub New()

      MyBase.New(TPlServerDB.ServerDB.CurrentServerDB, msaTableName, msaSPName)
   End Sub
   Protected Overrides Sub ClearTables()
      Const sSPName As String = "ClearProjectData"
      Dim iRes As Integer = doConnection.RunCommand(sSPName, CommandType.StoredProcedure, GetParameters())
   End Sub

   Protected Overrides Function ClearWarning(sList As String) As Boolean
      Const sMsg0 As String = " אתה עומד למחוק גירסה"
      Const sMsg1 As String = "? האם הינך בטוח שברצונך להמשיך"
      '   Dim sBlockList As String = zzGetBlockList()
      Dim sMsgText As String
      Dim bRes As Boolean = False

      Dim sProjectLabel As String = DMCommon.Functions.GetComplexName(ProjectCode, DetailNo)
      '  DMCommon.Debug.MsgBox("12_890a", miProjectCode, miDetailNo, sBlockList)


      sMsgText = "הגושים הקיימים בפרויקט " & sProjectLabel & vbCrLf

      sMsgText = sMsg0 & sProjectLabel & vbCrLf
      sMsgText &= sMsg1
      If System.Windows.Forms.MessageBox.Show(sMsgText, "Datamap", Windows.Forms.MessageBoxButtons.OKCancel, Windows.Forms.MessageBoxIcon.Warning, Windows.Forms.MessageBoxDefaultButton.Button2, Windows.Forms.MessageBoxOptions.RightAlign) = Windows.Forms.DialogResult.OK Then  'And Windows.Forms.MessageBoxOptions.RtlReading
         bRes = True

      End If



      Return bRes
   End Function

   Protected Overrides Function GetLoadParameters(sTableName As String) As Data.Common.DbParameter()
      Return MyBase.GetParameters(1)
   End Function

   Protected Overrides Function DataExists(ByRef sList As String) As Boolean
      Dim oDataReader As Common.DbDataReader = doConnection.GetDataReader(msPrjMapThemesSPName, CommandType.StoredProcedure, GetParameters())
      Dim bDataExists As Boolean = oDataReader IsNot Nothing AndAlso oDataReader.HasRows
      oDataReader.Close()
      '   DMCommon.Debug.MsgBox("12_900K", bDataExists, ProjectCode, DetailNo)
      Return bDataExists
   End Function

   Protected Overrides Sub UpdateProjectDetail()
      If doDataSet.Tables(0).Rows.Count > 0 Then
         ProjectCode = DMCommon.Functions.CIntN(doDataSet.Tables(0).Rows.Item(0).Item("ProjectCode"))
         DetailNo = DMCommon.Functions.CIntN(doDataSet.Tables(0).Rows.Item(0).Item("Detail"))
      End If
      '   DMCommon.Debug.MsgBox("12_900W", ProjectCode, DetailNo)
   End Sub
End Class
