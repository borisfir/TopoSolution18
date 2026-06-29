Option Explicit On
Option Strict On
Imports System.Data
Public Class UD_ExpImpData
   Inherits TPlServerDB.ExpImpData




   Const msJournalSPName As String = "GetJournalData"
   Const msFragmentsSPName As String = "GetFragments"
   Const msParcelFragmentsSPName As String = "GetParcelFragments"
   Const msProjectDataSPName As String = "GetProjectData"

   Private Shared msaTableName() As String = {"Journal", "Fragments", "ParcelFragments", "ProjectData"}
   Private Shared msaSPName() As String = {msJournalSPName, msFragmentsSPName, msParcelFragmentsSPName, msProjectDataSPName}

   Public Sub New(iProjectCode As Integer, iDetailNo As Integer)

      MyBase.New(TPlServerDB.ServerDB.CurrentProjectDB, iProjectCode, iDetailNo, msaTableName, msaSPName)
   End Sub

   Protected Overrides Function GetLoadParameters(sTableName As String) As System.Data.Common.DbParameter()
      Dim bBlockParameter As Boolean
      Select Case sTableName
         Case "ProjectData"
            bBlockParameter = False
         Case Else
            bBlockParameter = True
      End Select
      Return zzGetParameters(bBlockParameter)
   End Function
   Protected Overrides Function ClearWarning(sBlockList As String) As Boolean
      Const sMsg0 As String = " אתה עומד למחוק אותם"
      Const sMsg1 As String = "? האם הינך בטוח שברצונך להמשיך"
      '   Dim sBlockList As String = zzGetBlockList()
      Dim sMsgText As String
      Dim bRes As Boolean

      Dim sProjectLabel As String = TopoManager.TPlanGraph.TplnBlock.GetBlockName(ProjectCode, DetailNo)
      '  DMCommon.Debug.MsgBox("12_890a", miProjectCode, miDetailNo, sBlockList)
      If sBlockList IsNot Nothing Then
         sMsgText = "הגושים הקיימים בפרויקט " & sProjectLabel & vbCrLf
         sMsgText &= sBlockList & vbCrLf
         sMsgText &= sMsg0 & vbCrLf
         sMsgText &= sMsg1
         If System.Windows.Forms.MessageBox.Show(sMsgText, "Datamap", Windows.Forms.MessageBoxButtons.OKCancel, Windows.Forms.MessageBoxIcon.Warning, Windows.Forms.MessageBoxDefaultButton.Button2, Windows.Forms.MessageBoxOptions.RightAlign) = Windows.Forms.DialogResult.OK Then  'And Windows.Forms.MessageBoxOptions.RtlReading
            bRes = True
         Else
            bRes = False
         End If
      Else
         bRes = True
      End If
      Return bRes


   End Function
   Private Function zzGetBlockList() As String
      Const sBlockListSPName As String = "GetBlockList"
      Dim sBlockName As String
      Dim iBlockNo, iBlockAdd As Integer
      Dim oDataReader As Common.DbDataReader = doConnection.GetDataReader(sBlockListSPName, CommandType.StoredProcedure, zzGetParameters(False))
      Dim sBlockList As String = Nothing


      If oDataReader IsNot Nothing Then
         If oDataReader.HasRows Then
            '  DMCommon.Debug.MsgBox("12_890d")
            While oDataReader.Read
               iBlockNo = oDataReader.GetInt32(0)
               iBlockAdd = oDataReader.GetInt32(1)
               sBlockName = TopoManager.TPlanGraph.TplnBlock.GetBlockName(iBlockNo, iBlockAdd)
               If sBlockList Is Nothing Then
                  sBlockList = sBlockName
               Else
                  sBlockList &= sBlockName & ", "
               End If

            End While
         End If
         oDataReader.Close()
      End If

      Return sBlockList
   End Function
   Protected Overrides Sub ClearTables()
      Const sSPName As String = "ClearBlockData"
      Dim iRes As Integer = doConnection.RunCommand(sSPName, CommandType.StoredProcedure, zzGetParameters())
   End Sub
   Private Function zzGetParameters(Optional bBlockParameter As Boolean = True) As System.Data.Common.DbParameter()
      Dim iParamUB As Integer
      If bBlockParameter Then
         iParamUB = 3
      Else
         iParamUB = 1
      End If
      Dim oaParams() As System.Data.Common.DbParameter = MyBase.GetParameters(iParamUB)
      '	Dim oErrOut As System.Data.Common.DbException = Nothing


      If bBlockParameter Then
         oaParams(2) = doConnection.GetParameter("@prOriginalBlockNo", DbType.Int32, 0)
         oaParams(3) = doConnection.GetParameter("@prOriginalBlockAddNo", DbType.Int32, 0)
         '  DMCommon.Debug.MsgBox("11_572N", doConnection.Database, oaParams(0).Value, oaParams(1).Value, oaParams(2).Value, oaParams(3).Value)
      End If
      Return oaParams
   End Function

   Protected Overrides Function DataExists(ByRef sList As String) As Boolean
      sList = zzGetBlockList()

      '   DMCommon.Debug.MsgBox("12_900L", sList, ProjectCode, DetailNo)
      Return Not String.IsNullOrEmpty(sList)
   End Function

   Protected Overrides Sub UpdateProjectDetail()

   End Sub
End Class
