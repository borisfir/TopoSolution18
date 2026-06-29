Option Explicit On
Option Strict On
Imports System.Data
Public Class Form3
	Private Shared miLandusesFormatID As Integer = 1
	Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
		Debug.Print("QQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQQ")
		Dim sInput As String = "1-5,6,7,9-11,12"
      Dim oNumeration As TopoManager.NumerationPair.Numeration = New TopoManager.NumerationPair.Numeration(TopoManager.NumerationPair.enComplexType.Undefined, TopoManager.NumerationPair.enTextDirection.RightToLeft)
		oNumeration.Input(sInput)
		For iIndex As Integer = 0 To oNumeration.Count - 1
			txtDisp.Text &= (oNumeration.ItemPresentation(iIndex))
		Next
		txtDisp.Text &= vbCrLf
		txtDisp.Text &= oNumeration.GetPresentation()
	End Sub

	Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
		Dim oBalanceArea As TopoManager.BalanceArea
      '  Dim daSourceArea() As Double = {1.4, 1.51, 0.48, 1.52, 1.5}
      '  Dim daSourceArea() As Double = {1.51, 1.49, 1.52, 1.48}
      '   Dim daSourceArea() As Double = {1.555, 1.48, 1.49, 1.475}
      Dim daSourceArea() As Double = {1.655, 1.38, 1.49, 1.2, 1.275}
      Dim daConstArea() As Double = {0.0, 0.0, 0.0, 0.0, 0.0}
      Dim iaGroupNo() As Integer = {1, 0, 1, 1, 0}
      Dim iaPgonID() As Integer = {1, 0, 1, 1, 0}
      Dim taCells(daSourceArea.GetUpperBound(0)) As TopoManager.BalanceArea.Cell

      Dim dDest As Double = 7.0
      For iIndex As Integer = 0 To taCells.GetUpperBound(0)
         taCells(iIndex) = New TopoManager.BalanceArea.Cell(daSourceArea(iIndex), iaPgonID(iIndex), iaGroupNo(iIndex))
      Next
      '	Dim daSourceArea() As Double = {2.0, 2.0}
      'Dim daSourceArea() As Double = {339.854}
      oBalanceArea = New TopoManager.BalanceArea(taCells, TopoManager.BalanceArea.enBalanceLevel.AllByCells, 1.0, dDest, False, "Button2")

      '   oBalanceArea = New TopoManager.BalanceArea(daSourceArea, 1.0, dDest, False, "Button2")
      '   oBalanceArea = New TopoManager.BalanceArea(daSourceArea, iaGroupNo, 1.0, dDest, False, "Button2")

      Dim laOutput() As Long = oBalanceArea.Output
      taCells = oBalanceArea.Cells
      Stop
   End Sub
	Private Sub zzInitDB()
		Const sServerName As String = "Pluto"
		Const sDatabaseName As String = "ProjectData"
		'ALLA-DELL\PLUTO
		'		TPlServerDB.ServerDB.InitCurrentProject()
		TPlServerDB.ServerDB.InitCurrentServer()
		'		TPlServerDB.ServerDB.CurrentProjectDB.SetSQL(sServerName, sDatabaseName, True)
		TPlServerDB.ServerDB.CurrentServerDB.SetSQL(sServerName, sDatabaseName, True)



	End Sub

	Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
		zzInitDB()
		FillLanduses(Me.cmbFind, DMAcadExt.ColorScheme.NameDelim)
   End Sub
  
	Public Shared Sub FillLanduses(ByRef oCombo As ComboBox, ByVal sDelim As String, Optional ByVal iDefaultID As Integer = 0)
		Dim sComText As String = "SELECT ID,Name FROM Landuses_" & CStr(miLandusesFormatID) & "F ORDER BY ID"
		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
		Dim oItemData As DMCommon.ItemData
		Dim iLanduseID As Integer
		Dim iDefaultInfex As Integer = 0
		If oDataReader IsNot Nothing Then
			While oDataReader.Read
				iLanduseID = oDataReader.GetInt32(0)

				oItemData = New DMCommon.ItemData(iLanduseID, Convert.ToString(iLanduseID) & sDelim & oDataReader.GetString(1))

				oCombo.Items.Add(oItemData)
				If iLanduseID = iDefaultID Then
					iDefaultInfex = oCombo.Items.Count - 1
				End If
			End While
			oDataReader.Close()
			oCombo.SelectedIndex = iDefaultInfex
		End If

	End Sub



	Private Sub cmbFind_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cmbFind.SelectedIndexChanged

	End Sub
	Private Function zzGetCriteria() As String
		Const gsAnd As String = " AND "
		Dim sWhereStr As String = String.Empty
		Dim sConnectStr As String = String.Empty
		Dim lInputLen As Long
		Dim sFindText As String

		sFindText = Me.cmbFind.Text
		lInputLen = Len(sFindText)
		sFindText = Replace(sFindText, "'", "''", 1&, -1&, vbBinaryCompare)


		If Len(sFindText) <> 0& Then

			sWhereStr = "CHARINDEX( '" & sFindText & "',[Name]) <> 0"
			sConnectStr = gsAnd
		End If


		If sWhereStr.Length = 0 Then
			Return String.Empty
		Else
			Return " WHERE " & sWhereStr
		End If


	End Function
	Public Shared Function GetCustomersTable(ByVal sCriteria As String, Optional ByVal bShortName As Boolean = False, Optional ByVal iTop As Integer = -1) As DataTable
		Dim sComText As String = zzGetCustomerTableComText(sCriteria, iTop)

		Return TPlServerDB.ServerDB.CurrentServerDB.GetDataTable(sComText, System.Data.CommandType.Text, "List")
	End Function
	Private Shared Function zzGetCustomerTableComText(ByVal sCriteria As String, Optional ByVal iTop As Integer = -1) As String
		Dim sTopText As String
		Dim sName As String = "Name"
		If iTop = -1 Then
			sTopText = String.Empty
		Else
			sTopText = " TOP " & CStr(iTop)
		End If

		Return "SELECT " & sTopText & "ID," & sName & " FROM Landuses_1F " & sCriteria & " ORDER BY Name"
	End Function

	Private Sub cmbFind_TextChanged(ByVal oSender As System.Object, e As System.EventArgs) Handles cmbFind.TextChanged
		Dim sCriteria As String = zzGetCriteria()
		Me.lstFind.DataSource = GetCustomersTable(sCriteria)
	End Sub

	Private Sub lstFind_SelectedIndexChanged(oSender As System.Object, e As System.EventArgs) Handles lstFind.SelectedIndexChanged

	End Sub

	Private Sub TextBox1_TextChanged(sender As System.Object, e As System.EventArgs) Handles TextBox1.TextChanged

	End Sub

	Private Sub ToolStripTextBox1_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripTextBox1.Click

	End Sub

	Private Sub ToolStripTextBox1_Enter(ByVal oSender As System.Object, e As System.EventArgs) Handles ToolStripTextBox1.DragDrop

	End Sub

	Private Sub ToolStripTextBox1_KeyPress(ByVal oSender As System.Object, e As System.Windows.Forms.KeyPressEventArgs) Handles ToolStripTextBox1.KeyPress
		MessageBox.Show(e.KeyChar.ToString(), "")
	End Sub

	Private Sub ToolStripTextBox1_KeyDown(ByVal oSender As System.Object, e As System.Windows.Forms.KeyEventArgs) Handles ToolStripTextBox1.KeyUp
		MessageBox.Show(e.KeyCode.ToString() & vbCrLf & e.KeyData.ToString() & vbCrLf & e.KeyValue.ToString(), "")
	End Sub

   Private Sub Button4_Click(oSender As System.Object, e As EventArgs) Handles Button4.Click
      Dim oArea As TopoManager.TPlanGraph.ParcelArea = New TopoManager.TPlanGraph.ParcelArea
      oArea.CalculateArea(34.104, 33886, 3)

   End Sub

   Private Sub DataGridView1_CellContentClick(oSender As System.Object, e As DataGridViewCellEventArgs) Handles dgvArea.CellContentClick

   End Sub

   Private Sub Button5_Click(oSender As System.Object, e As EventArgs) Handles Button5.Click
      ' Dim daSourceArea() As Double = {1.655, 1.38, 1.49, 1.2, 1.275}
      '  Dim daSourceArea() As Double = {12.65579, 18.38477, 23.49081, 2.20003, 5.27529}
      Dim daSourceArea() As Double = {2000, 1000, 5000, 5000, 2000}

      '+@@ 7,4: 2000, 2000, 2000
      '+@@ 8,3: 1000, 1200, 1000
      '+@@ 8,0: 9000, 10800, 9000
      '+@@ 9,3: 5000, 5250, 5000
      '+@@ 9,0: 10000, 10500, 10000
      '+@@ 9,4: 5000, 5250, 5000
      '+@@ 10,4: 2000, 2000, 2000
      '+@@ 10,0: 16000, 16000, 16000

      '+@@ 7,4,2: 2000, 2000, 2000
      '+@@ 8,3,1: 1000, 1200, 1000
      '+@@ 8,0,0: 9000, 10800, 9000
      '+@@ 9,3,1: 5000, 5250, 5000
      '+@@ 9,0,0: 10000, 10500, 10000
      '+@@ 9,4,2: 5000, 5250, 5000
      '+@@ 10,4,2: 2000, 2000, 2000
      '+@@ 10,0,0: 16000, 16000, 16000


      '      ^451 1000
      '^451 5000
      '^451 2000
      '^451 5000
      '^451 2000


      Dim daConstArea() As Double = {0.0, 0.0, 0.0, 0.0, 0.0}
      Dim iaPgonID() As Integer = {4, 3, 3, 4, 4}
      '   Dim iaGroupNo() As Integer = {0, 1, 1, 1, 0}
      '  Dim iaGroupNo() As Integer = {1, 2, 2, 1, 1}
      Dim iaGroupNo() As Integer = {2, 2, 2, 2, 2}

      Dim oRow As DataGridViewRow
      Me.txtDest.Text = "15700"
      '  Me.txtDest.Text = "7"
      Me.cmbRound.Text = "1"
      ' Me.cmbRound.Text = "1000"


      Me.dgvArea.Rows.Add(daSourceArea.GetUpperBound(0) + 1)

      For iIndex As Integer = 0 To daSourceArea.GetUpperBound(0)
         oRow = Me.dgvArea.Rows.Item(iIndex)
         oRow.Cells.Item(0).Value = daSourceArea(iIndex)
         oRow.Cells.Item(1).Value = iaGroupNo(iIndex)
         oRow.Cells.Item(2).Value = iaPgonID(iIndex)


      Next
      Dim iRows As Integer = Me.dgvArea.Rows.Count
      oRow = Me.dgvArea.Rows.Item(Me.dgvArea.Rows.Count - 1)

   End Sub

   Private Sub Button6_Click(oSender As System.Object, e As EventArgs) Handles Button6.Click
      Dim oBalanceArea As TopoManager.BalanceArea
      Dim iUB As Integer = Me.dgvArea.Rows.Count - 2
      Dim iGroupUB As Integer = Me.dgvGroups.Rows.Count - 2
      Dim iPgonUB As Integer = Me.dgvPgons.Rows.Count - 2


      Dim oaCells(iUB) As TopoManager.BalanceArea.Cell
      Dim taConstGroups(iGroupUB) As TopoManager.BalanceArea.ConstArea
      Dim taConstPgons(iPgonUB) As TopoManager.BalanceArea.ConstArea

      Dim oRow As DataGridViewRow
      Dim dDest As Double = Convert.ToDouble(Me.txtDest.Text)
      Dim dSource As Double
      Dim dConst As Double
      Dim iGroup As Integer
      Dim iPgonID As Integer

      Dim iRound As Integer = Convert.ToInt32(Me.cmbRound.Text)
      For iIndex As Integer = 0 To oaCells.GetUpperBound(0)
         oRow = Me.dgvArea.Rows.Item(iIndex)
         dSource = Convert.ToDouble(oRow.Cells.Item(0).Value)
         iGroup = Convert.ToInt32(oRow.Cells.Item(1).Value)
         iPgonID = Convert.ToInt32(oRow.Cells.Item(2).Value)
         oaCells(iIndex) = New TopoManager.BalanceArea.Cell(dSource, iPgonID, iGroup)
      Next
      For iIndex As Integer = 0 To iGroupUB
         oRow = Me.dgvGroups.Rows.Item(iIndex)
         iGroup = Convert.ToInt32(oRow.Cells.Item(0).Value)
         dConst = Convert.ToDouble(oRow.Cells.Item(4).Value)
         taConstGroups(iIndex) = New TopoManager.BalanceArea.ConstArea(iGroup, dConst)
      Next
      For iIndex As Integer = 0 To iPgonUB
         oRow = Me.dgvPgons.Rows.Item(iIndex)
         iPgonID = Convert.ToInt32(oRow.Cells.Item(0).Value)
         dConst = Convert.ToDouble(oRow.Cells.Item(4).Value)
         taConstPgons(iIndex) = New TopoManager.BalanceArea.ConstArea(iPgonID, dConst)
      Next
      If iGroupUB >= -1 Then
         oBalanceArea = New TopoManager.BalanceArea(oaCells, taConstPgons, taConstGroups, TopoManager.BalanceArea.enBalanceLevel.AllByCells, dDest, iRound, False, "Button2")
      Else
         oBalanceArea = New TopoManager.BalanceArea(oaCells, TopoManager.BalanceArea.enBalanceLevel.AllByCells, dDest, iRound, False, "Button2")
      End If


      '   oBalanceArea = New TopoManager.BalanceArea(daSourceArea, 1.0, dDest, False, "Button2")
      '   oBalanceArea = New TopoManager.BalanceArea(daSourceArea, iaGroupNo, 1.0, dDest, False, "Button2")

      '    Dim laOutput() As Long = oBalanceArea.Output
      oaCells = oBalanceArea.Cells

      For iIndex As Integer = 0 To oaCells.GetUpperBound(0)
         oRow = Me.dgvArea.Rows.Item(iIndex)
         oRow.Cells.Item(3).Value = oaCells(iIndex).OutputAreaFix
         oRow.Cells.Item(4).Value = oaCells(iIndex).OutputAreaFloat

      Next
      Dim oaGroups() As TopoManager.BalanceArea.Cell = oBalanceArea.GroupsAsCells
      Me.dgvGroups.Rows.Clear()
      Me.dgvGroups.Rows.Add(oaGroups.GetUpperBound(0) + 1)
      For iIndex As Integer = 0 To oaGroups.GetUpperBound(0)
         oRow = Me.dgvGroups.Rows.Item(iIndex)
         oRow.Cells.Item(0).Value = oaGroups(iIndex).GroupNo
         oRow.Cells.Item(1).Value = oaGroups(iIndex).SourceArea
         oRow.Cells.Item(2).Value = oaGroups(iIndex).OutputAreaFix
         oRow.Cells.Item(3).Value = oaGroups(iIndex).OutputAreaFloat
         oRow.Cells.Item(4).Value = oaGroups(iIndex).ConstArea
      Next

      Dim oaBasicPgons() As TopoManager.BalanceArea.Cell = oBalanceArea.BasicPgonsAsCells
      Me.dgvPgons.Rows.Clear()
      Me.dgvPgons.Rows.Add(oaBasicPgons.GetUpperBound(0) + 1)
      For iIndex As Integer = 0 To oaBasicPgons.GetUpperBound(0)
         oRow = Me.dgvPgons.Rows.Item(iIndex)
         oRow.Cells.Item(0).Value = oaBasicPgons(iIndex).BasicPgonID
         oRow.Cells.Item(1).Value = oaBasicPgons(iIndex).SourceArea
         oRow.Cells.Item(2).Value = oaBasicPgons(iIndex).OutputAreaFix
         oRow.Cells.Item(3).Value = oaBasicPgons(iIndex).OutputAreaFloat
         oRow.Cells.Item(4).Value = oaBasicPgons(iIndex).ConstArea
      Next
      '050 889 43 99
   End Sub

   Private Sub Button7_Click(oSender As System.Object, e As EventArgs) Handles Button7.Click
      Dim iSrc As Integer = Integer.MaxValue
      Dim iOvl As Integer = Integer.MaxValue
      Dim lKey As ULong = ToLongKey(iSrc, iOvl)
       
      Stop
   End Sub
   Private Function ToLongKey(iSourceID As Integer, iOverlayID As Integer) As ULong
    
      Return (CType(iSourceID, ULong) << 32) + CType(iOverlayID, ULong)
   End Function
End Class

'34.104	33.886	0.218
