Option Explicit On
Option Strict On
Imports System.Data
Public Class frmEditDetails
	Private miProjectCode As Integer
	Private moDataAdapter As Common.DbDataAdapter
	Private WithEvents moDetails As DataTable
   Private miDetailSource As Integer
   Private miDetailNo As Integer
   Private mbEventsEnabled As Boolean = False
   Private Shared mtID_Control As DMCommon.ID_Control
   Public Shared Property DetailID_Control As DMCommon.ID_Control
      Get
         Return mtID_Control
      End Get
      Set(tValue As DMCommon.ID_Control)
         mtID_Control = tValue
         'MessageBox.Show(CStr(mtID_Control.miMinID) & ":" & CStr(mtID_Control.miMaxID))
      End Set
   End Property



   Private Sub zzSetDetails()
      Dim sComText As String = "SELECT ProjectCode, Detail, DetailName, CreatorName, DateCreated FROM PrjDetails WHERE ProjectCode=" & CStr(miProjectCode)
      Dim iRowsCount As Integer

      moDataAdapter = TPlServerDB.ServerDB.CurrentServerDB.GetDataAdapter(sComText, CommandType.Text, True, , True, True)
      moDetails = New DataTable("Details")
      moDataAdapter.Fill(moDetails)
      ' DMCommon.Debug.MsgBox("13_012", TPlServerDB.ServerDB.CurrentServerDB.ConnectionString, moDetails.Rows.Count)
      iRowsCount = moDetails.Rows.Count
      If iRowsCount = 0 Then
         miDetailSource = 0
      Else
         miDetailSource = DirectCast(moDetails.Rows.Item(iRowsCount - 1).Item(1), Integer)

      End If

      If mtID_Control.BelongRange(miDetailSource) Then
         mtID_Control.SetID(miDetailSource)
      Else
         mtID_Control.Reset()
         For iIndex As Integer = 0 To iRowsCount - 1
            mtID_Control.AddID(DirectCast(moDetails.Rows.Item(iIndex).Item(1), Integer))
         Next
      End If
      miDetailNo = mtID_Control.GetNextID()
      Me.dgvMain.AutoGenerateColumns = False
      'MessageBox.Show(CStr(miDeatailSource) & ":" & CStr(miDetailNo) & vbCrLf & CStr(mtID_Control.miCurrentMaxID), "03_490")
      Me.dgvMain.DataSource = moDetails
   End Sub
   Private Sub zzAddPrjMapTheme()
      Dim oRow As DataRow
      Dim oDataGridRow As DataGridViewRow
      Dim oCelVall As System.Object
      Dim sCelVall As String
      Dim iCellVal As Integer = -1

      Dim iNewDetail As Integer
      Dim iResRows As Integer
      Dim bAddOK As Boolean = True

      For iIndex As Integer = 0 To moDetails.Rows.Count - 1
         oRow = moDetails.Rows.Item(iIndex)

         If oRow.RowState = DataRowState.Added Then
            iNewDetail = DirectCast(oRow.Item(1), Integer)
            oDataGridRow = dgvMain.Rows.Item(iIndex)
				oCelVall = oDataGridRow.Cells.Item(2).Value
				If oCelVall Is Nothing Then
               iCellVal = -1
            Else
               sCelVall = DirectCast(oCelVall, String)
               If sCelVall.Length = 0 Then
                  iCellVal = -1
               Else
                  If Not Integer.TryParse(sCelVall, iCellVal) Then
                     iCellVal = -1
                     MessageBox.Show(sCelVall, "Err #1201")
                  End If
               End If
            End If

            iResRows = zzAddPrjMapTheme(iNewDetail, iCellVal)
            If iResRows = 0 Then
               bAddOK = False
            End If
         End If
      Next
      If bAddOK Then

         moDataAdapter.Update(moDetails)
      End If

   End Sub
   Private Function zzAddPrjMapTheme(iNewDetailNo As Integer, Optional iDetailSource As Integer = -1) As Integer
      Const sSPName As String = "AddDetail"
      Dim oaParams(2) As Common.DbParameter
      Dim iResRows As Integer
      Dim oErrOut As System.Data.Common.DbException = Nothing
      If iDetailSource = -1 Then
         iDetailSource = miDetailSource
      End If
      oaParams(0) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prProjectCode", DbType.Int32, miProjectCode)
      oaParams(1) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prDetailSource", DbType.Int32, iDetailSource)
      oaParams(2) = TPlServerDB.ServerDB.CurrentProjectDB.GetParameter("@prNewDetail", DbType.Int32, iNewDetailNo)

      iResRows = TPlServerDB.ServerDB.CurrentServerDB.RunCommand(sSPName, CommandType.StoredProcedure, oaParams, oErrOut)


      '	MessageBox.Show(CStr(iResRows) & vbCrLf & CStr(miProjectCode) & ":" & CStr(miDeatailSource) & ":" & CStr(iNewDeatailNo), "01_437")
      Return iResRows
   End Function

	Private Sub zzSetPrjName()
		Dim sComText As String = "SELECT PrjName FROM [ProjectListLocal] WHERE PrjCode=" & CStr(miProjectCode)
		Dim oPrjName As System.Object = TPlServerDB.ServerDB.CurrentServerDB.GetDataScalar(sComText, CommandType.Text)
		If oPrjName Is Nothing Then
			Me.txtProjectName.Text = String.Empty
		Else
			Me.txtProjectName.Text = DirectCast(oPrjName, String)
		End If
	End Sub
	Public Sub New(iProjectCode As Integer)

		' This call is required by the designer.
		InitializeComponent()
		miProjectCode = iProjectCode
		' Add any initialization after the InitializeComponent() call.
		zzSetPrjName()
		zzSetDetails()

	End Sub

	Private Sub frmEditDetails_Load(oSender As System.Object, e As System.EventArgs) Handles MyBase.Load
		If miProjectCode <> 0 Then
			Me.txtProjectCode.Text = Convert.ToString(miProjectCode)
		End If
	End Sub

	Private Sub txtProjectCode_Leave(oSender As System.Object, e As System.EventArgs) Handles txtProjectCode.Leave
		If IsNumeric(Me.txtProjectCode.Text) Then
			miProjectCode = Convert.ToInt32(Me.txtProjectCode.Text)
		End If
	End Sub
	Private Sub cmdOK_Click(oSender As System.Object, e As System.EventArgs) Handles cmdOK.Click
		zzAddPrjMapTheme()
		Me.DialogResult = Windows.Forms.DialogResult.OK
		'zzUpdateDetailsTable()
	End Sub

	Private Sub moDetails_RowChanged(oSender As System.Object, e As System.Data.DataRowChangeEventArgs) Handles moDetails.RowChanged
		If e.Action = DataRowAction.Add Then
			'	miDetailNo += 1
			miDetailNo = mtID_Control.GetNextID()
		End If
		'	MessageBox.Show(e.Action.ToString(), "01_295Table ed")
	End Sub
	Private Sub moDetails_TableNewRow(oSender As System.Object, e As System.Data.DataTableNewRowEventArgs) Handles moDetails.TableNewRow
		'	MessageBox.Show(miDetailNo.ToString(), "01_299o")
		Dim oRow As DataRow = e.Row
		oRow.BeginEdit()
		oRow.Item(0) = miProjectCode
		oRow.Item(1) = miDetailNo
		oRow.Item(3) = System.Environment.UserName
		oRow.Item(4) = Date.Now
		oRow.EndEdit()

	End Sub



	Private Sub frmEditDetails_Shown(sender As System.Object, e As System.EventArgs) Handles Me.Shown
		If Me.Location.X <= 0 OrElse Me.Location.Y <= 0 OrElse Me.Location.X > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Right - 100 OrElse Me.Location.Y > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Top - 100 Then
			Me.Location = New System.Drawing.Point(240, 240)
			Me.Size = New System.Drawing.Size(540, 348)
			Me.WindowState = FormWindowState.Normal
		End If
		mbEventsEnabled = True
	End Sub

	Private Sub cmdCancel_Click(sender As System.Object, e As System.EventArgs) Handles cmdCancel.Click
		Me.DialogResult = Windows.Forms.DialogResult.Cancel
	End Sub


End Class