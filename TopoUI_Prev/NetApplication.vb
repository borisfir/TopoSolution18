Option Explicit On
Option Strict On

Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.EditorInput
Imports AcDbSymbolUtilities
Imports Autodesk.Gis.Map

Imports Autodesk.Gis.Map.Utilities
Imports System.Runtime.InteropServices
Imports TopoManager
Public NotInheritable Class NetApplication
   Private mfTopoView As frmTopoView
   Private WithEvents mfTplnView As TPlanGraph.frmTplnView
	Private WithEvents mfTopoActions As frmTopoActions
	Private WithEvents mfTopoActionsB As frmTopoActionsB

   Private WithEvents moAcadDocument As Autodesk.AutoCAD.ApplicationServices.Document

	<Autodesk.AutoCAD.Runtime.CommandMethodAttribute("CmdList")> _
		  Public Shared Sub CmdList()
		Common.GetEditor.WriteMessage(vbCrLf & " TownPlanner Commands : " & vbCrLf)
		Common.GetEditor.WriteMessage("** Cmd : TplnCalc " & vbCrLf)

		Common.GetEditor.WriteMessage("** Cmd : RepContent  - 301" & vbCrLf)
		Common.GetEditor.WriteMessage("** Cmd : RepPlanParcels  - 302 " & vbCrLf)
		Common.GetEditor.WriteMessage("** Cmd : RepLotsK  - 303 " & vbCrLf)
		Common.GetEditor.WriteMessage("** Cmd : RepLotsM  - 304 " & vbCrLf)

		Common.GetEditor.WriteMessage("** Cmd : RepParcelsLuseK  - 305 " & vbCrLf)
		Common.GetEditor.WriteMessage("** Cmd : RepParcelsLuseM  - 306 " & vbCrLf)

		Common.GetEditor.WriteMessage("** Cmd : RepSumLuse  - 307 " & vbCrLf)
		Common.GetEditor.WriteMessage("** Cmd : RepParcels  - 311 " & vbCrLf)


		Common.GetEditor.WriteMessage("** Cmd : DispTpln " & vbCrLf)
		Common.GetEditor.WriteMessage("** Cmd : TplnClose " & vbCrLf)

	End Sub
   <CommandMethod("TplnOpenA")> _
 Public Sub TplnOpenA()
		TopoManager.TPlanGraph.TplnProject.InitializeDB()
      If mfTopoActions Is Nothing Then
         mfTopoActions = New frmTopoActions
      End If
		TopoManager.TPlanGraph.TplnProject.InitializeList()
      If mfTopoActions.ShowDialog = DialogResult.Yes Then
         mfTopoActions.Dispose()
         mfTopoActions = Nothing
			TopoManager.TPlanGraph.TplnProject.Close()
      End If

	End Sub
	<CommandMethod("TmstOpen")> _
  Public Sub TmstOpen()
		Common.AppID = enApplications.TopoMaster
		TopoManager.TPlanGraph.TplnProject.InitAppName()
		TopoManager.TPlanGraph.TplnProject.InitializeDB()
		If TopoManager.TPlanGraph.TplnProject.InitializedDB Then
			TopoManager.TPlanGraph.TplnProject.InitializeList()
			If mfTopoActionsB Is Nothing OrElse mfTopoActions.IsDisposed Then
				mfTopoActionsB = New frmTopoActionsB
			End If
			TopoManager.TPlanGraph.TplnProject.InitializeView()
			Dim oAcadWin As AcadWin = New AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
			Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfTopoActionsB)
		End If
	End Sub

	<CommandMethod("TplnOpen")> _
  Public Sub TplnOpen()
		Common.AppID = enApplications.Taba
		TopoManager.TPlanGraph.TplnProject.InitAppName()
		TopoManager.TPlanGraph.TplnProject.InitializeDB()
		If TopoManager.TPlanGraph.TplnProject.InitializedDB Then
			TopoManager.TPlanGraph.TplnProject.InitializeList()
			If mfTopoActions Is Nothing OrElse mfTopoActions.IsDisposed Then
				mfTopoActions = New frmTopoActions
			End If
			TopoManager.TPlanGraph.TplnProject.InitializeView()
			Dim oAcadWin As AcadWin = New AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
			Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfTopoActions)
		End If
	End Sub
   <CommandMethod("TplnCleanup")> _
  Public Sub TplnCleanup()
      If mfTopoActions IsNot Nothing Then
         mfTopoActions.Mark()
      End If
   End Sub
   <CommandMethod("TestLayer")> _
   Public Shared Sub TestLayer()
		TopoManager.AcadTransaction.Start()
      AcadTransaction.TestSetLayer("1602", "TestLayer")
      AcadTransaction.Terminate()
   End Sub
   <CommandMethod("TplnBuild")> _
  Public Sub TplnBuild()
      If mfTopoActions IsNot Nothing Then
         mfTopoActions.CreateTopo()
      End If
   End Sub
   <CommandMethod("TplnKill")> _
 Public Sub TplnKill()
      If mfTopoActions IsNot Nothing Then
         mfTopoActions.DeleteTopo()
      End If
   End Sub
   <CommandMethod("ExecUnion")> _
Public Sub ExecUnion()
      TopoOverlay.TopoSourceName = "TopoA"
      TopoOverlay.TopoOverlayName = "TopoB"
      TopoOverlay.Exec()
   End Sub
   <CommandMethod("UnionTplnM")> _
Public Sub UnionTplnM()
      TopoOverlay.TopoSourceName = TPlanGraph.TplnLot.TopoName(TPlanGraph.enTopoPurpose.Proposed)
      TopoOverlay.TopoOverlayName = TPlanGraph.TplnParcel.TopoName
      TopoOverlay.TopoResultName = TPlanGraph.TplnLot.UnionTopoNameB(TPlanGraph.enTopoPurpose.Proposed)
      TopoOverlay.ExecTplan()
   End Sub
   <CommandMethod("UnionTplnK")> _
Public Sub UnionTplnK()
      TopoOverlay.TopoSourceName = TPlanGraph.TplnLot.TopoName(TPlanGraph.enTopoPurpose.Approved)
      TopoOverlay.TopoOverlayName = TPlanGraph.TplnParcel.TopoName
      TopoOverlay.TopoResultName = TPlanGraph.TplnLot.UnionTopoNameB(TPlanGraph.enTopoPurpose.Approved)
      TopoOverlay.ExecTplan()
   End Sub
   <CommandMethod("TopoList")> _
Public Sub TopoList()

      Dim saAllTopoNames() As String = Nothing
      Dim saUnionTopoNames() As String = Nothing


      ODEditor.GetTopoNames(saAllTopoNames, saUnionTopoNames)
      Dim sAllOut As String = Join(saAllTopoNames, vbCrLf)
      Dim sUnionOut As String = Join(saAllTopoNames, vbCrLf)

      System.Windows.Forms.MessageBox.Show(sAllOut & vbCrLf & sUnionOut)
   End Sub
   <CommandMethod("DispTopo")> _
    Public Sub DispTopo()
      mfTopoView = New frmTopoView
      mfTopoView.ShowDialog()
      mfTopoView.Dispose()
   End Sub
   <CommandMethod("DispTpln")> _
 Public Sub DispTpln()
      ' mfTplnView = New TPlanGraph.frmTplnView
      '  mfTplnView.ShowDialog()
      '  mfTplnView.Dispose()
      TPlanGraph.TplnProject.InitializeDB()
      If TPlanGraph.TplnProject.InitializedDB Then
         If mfTplnView Is Nothing OrElse mfTplnView.IsDisposed Then
            mfTplnView = New TPlanGraph.frmTplnView
         End If

         Dim oAcadWin As AcadWin = New AcadWin(Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Handle)
         Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessDialog(oAcadWin, mfTplnView)
      End If
 
   End Sub
   <CommandMethod("Tpln")> _
  Public Sub Tpln()
      '   System.Windows.Forms.MessageBox.Show(TPlServerDB.TextResource.GetText(7, 2, 1))

      TPlanGraph.TplnProject.InitializeDB()
      TPlanGraph.TplnProject.InitializeList()

      TPlanGraph.TplnLot.Initialize()
      TPlanGraph.TplnProject.LoadLots(TPlanGraph.enTopoPurpose.Proposed)
      TPlanGraph.TplnProject.LoadLots(TPlanGraph.enTopoPurpose.Approved)

      TPlanGraph.TplnParcel.Initialize()
      TPlanGraph.TplnProject.LoadParcels()
      TPlanGraph.TplnProject.LoadMerge(TPlanGraph.enTopoPurpose.Proposed)
      TPlanGraph.TplnProject.LoadMerge(TPlanGraph.enTopoPurpose.Approved)
      TPlanGraph.TplnProject.UpdateParcelTable(True, False, True, True)
      TPlanGraph.TplnProject.UpdateLotTable(TPlanGraph.enTopoPurpose.Proposed)
      TPlanGraph.TplnProject.UpdateLotTable(TPlanGraph.enTopoPurpose.Approved)
      TPlanGraph.TplnParcel.CalculateBlocks()
      TPlanGraph.TplnProject.Terminate()



   End Sub
   <CommandMethod("TestRep")> _
 Public Sub TestRep()
      Dim oRepApp As AcadReport.Application
      oRepApp = New AcadReport.Application(TPlServerDB.enResourceTheme.AcRepContent)
      oRepApp.Insert()
      oRepApp = Nothing
   End Sub
   <CommandMethod("TplnA")> _
  Public Sub TplnA()
      '   System.Windows.Forms.MessageBox.Show(TPlServerDB.TextResource.GetText(7, 2, 1))
      TPlanGraph.TplnProject.InitializeDB()
      TPlanGraph.TplnProject.InitializeList()

      TPlanGraph.TplnLot.Initialize()
      TPlanGraph.TplnProject.LoadLots(TPlanGraph.enTopoPurpose.Proposed)
      TPlanGraph.TplnProject.LoadLots(TPlanGraph.enTopoPurpose.Approved)
      TPlanGraph.TplnParcel.Initialize()

      TPlanGraph.TplnProject.LoadParcels()


      TPlanGraph.TplnProject.LoadMerge(TPlanGraph.enTopoPurpose.Proposed)

      TPlanGraph.TplnProject.LoadMerge(TPlanGraph.enTopoPurpose.Approved)
      '     System.Windows.Forms.MessageBox.Show("BEFORE UpdateParcelTabled")
      TPlanGraph.TplnProject.UpdateParcelTable(True, False, True, True)
      '  System.Windows.Forms.MessageBox.Show("BEFORE UpdateLotTable")
      TPlanGraph.TplnProject.UpdateLotTable(TPlanGraph.enTopoPurpose.Proposed)
      TPlanGraph.TplnProject.UpdateLotTable(TPlanGraph.enTopoPurpose.Approved)
      TPlanGraph.TplnParcel.CalculateBlocks()
      TPlanGraph.TplnProject.Terminate()

   End Sub
  
   <CommandMethod("TestGetPoint")> _
Public Sub TestGetPoint()
      Dim saPrompt(1) As String
      saPrompt(0) = "Enter start point of the line"

      saPrompt(1) = "Enter end point of the line"

      AcadReport.AcadUtil.GetTwoPoints(saPrompt)

   End Sub
   <CommandMethod("RepParcels")> Public Sub RepParcels() '311
      Dim oRepApp As AcadReport.Application = New AcadReport.Application(TPlServerDB.enResourceTheme.AcRepParcels)
      oRepApp.MainView = TPlanGraph.TplnParcel.MainView

      oRepApp.Insert()
      ' '''''''''TEMP TPlServerDB.ServerDB.Close()
   End Sub
   <CommandMethod("RepContent")> Public Sub RepContent() '301
      Dim oRepApp As AcadReport.Application = New AcadReport.Application(TPlServerDB.enResourceTheme.AcRepContent)
      oRepApp.MainView = TPlanGraph.TplnParcel.BlockView

      oRepApp.Insert()
      ''  TPlServerDB.ServerDB.Close()
   End Sub
 
   <CommandMethod("RepPlanParcels")> Public Sub RepPlanParcels() '302
      Dim oRepApp As AcadReport.Application = New AcadReport.Application(TPlServerDB.enResourceTheme.AcRepPlanParcels)
      Dim oDataView As DataView = Nothing
      Dim oaTotals() As System.Object = Nothing
      Dim iaColumns() As Integer = Nothing
      TPlanGraph.TplnParcel.GetInPlanData(oDataView, iaColumns, oaTotals)
      oRepApp.MainView = oDataView
      oRepApp.Totals = oaTotals
      oRepApp.Insert()
      ' '''''''''TEMP TPlServerDB.ServerDB.Close()
   End Sub
   <CommandMethod("RepLotsK")> Public Sub RepLotsK() '303
      Dim oRepApp As AcadReport.Application = New AcadReport.Application(TPlServerDB.enResourceTheme.AcRepLotsK)
      Dim oDataView As DataView = Nothing
      Dim iaDataColumns() As Integer = Nothing
      Dim oaTotals() As System.Object = Nothing

      TPlanGraph.TplnLot.GetMainData(TPlanGraph.enTopoPurpose.Approved, TPlanGraph.enDataOptions.Default, oDataView, iaDataColumns, oaTotals)
      oRepApp.MainView = oDataView
      oRepApp.Totals = oaTotals
      oRepApp.Insert()
      ' '''''''''TEMP TPlServerDB.ServerDB.Close()
   End Sub
   <CommandMethod("RepLotsM")> Public Sub RepLotsM() '304
      Dim oRepApp As AcadReport.Application = New AcadReport.Application(TPlServerDB.enResourceTheme.AcRepLotsM)
      Dim oDataView As DataView = Nothing
      Dim iaDataColumns() As Integer = Nothing
      Dim oaTotals() As System.Object = Nothing
      TPlanGraph.TplnLot.GetMainData(TPlanGraph.enTopoPurpose.Proposed, TPlanGraph.enDataOptions.Default, oDataView, iaDataColumns, oaTotals)
      oRepApp.MainView = oDataView
      oRepApp.Totals = oaTotals
      oRepApp.Insert()
      ' '''''''''TEMP TPlServerDB.ServerDB.Close()
   End Sub
   <CommandMethod("RepParcelsLuseK")> Public Sub RepParcelsLuseK() '305
      Dim oRepApp As AcadReport.Application = New AcadReport.Application(TPlServerDB.enResourceTheme.AcRepParcelLuseK)
      Dim oDataView As DataView = Nothing
      Dim iaDataColumns() As Integer = Nothing
      Dim oaTotals() As Object = Nothing
      TPlanGraph.TplnParcel.GetLanduseData(TPlanGraph.enTopoPurpose.Approved, TPlanGraph.enDataOptions.Default, oDataView, iaDataColumns, oaTotals)
      oRepApp.MainView = oDataView
      oRepApp.Totals = oaTotals
      oRepApp.Insert()
      ' '''''''''TEMP TPlServerDB.ServerDB.Close()
   End Sub
   <CommandMethod("RepParcelsLuseM")> Public Sub RepParcelsLuseM() '306
      Dim oRepApp As AcadReport.Application = New AcadReport.Application(TPlServerDB.enResourceTheme.AcRepParcelLuseM)
      Dim oDataView As DataView = Nothing
      Dim iaDataColumns() As Integer = Nothing
      Dim oaTotals() As System.Object = Nothing
      TPlanGraph.TplnParcel.GetLanduseData(TPlanGraph.enTopoPurpose.Proposed, TPlanGraph.enDataOptions.Default, oDataView, iaDataColumns, oaTotals)
      oRepApp.MainView = oDataView
      oRepApp.Totals = oaTotals
      oRepApp.Insert()
      ' '''''''''TEMP TPlServerDB.ServerDB.Close()
   End Sub
   <CommandMethod("RepSumLuse")> Public Sub RepSumLuse() '307
      Dim oRepApp As AcadReport.Application = New AcadReport.Application(TPlServerDB.enResourceTheme.AcRepSumLuse)
      Dim oDataView As DataView = Nothing
      Dim oaTotals() As System.Object = Nothing
      TPlanGraph.TplnLot.GetSumLanduseData(oDataView, TPlanGraph.enDataOptions.Default, oaTotals)
      oRepApp.MainView = oDataView
      oRepApp.Totals = oaTotals
      oRepApp.Insert()
      ' '''''''''TEMP TPlServerDB.ServerDB.Close()
   End Sub
   <CommandMethod("TplnCalc")> Public Sub TplnCalc()
      UnionTplnM()
      UnionTplnK()
      Tpln()

   End Sub
   <CommandMethod("aaa")> Public Sub aaa()
      UnionTplnM()
      UnionTplnK()
      Tpln()
      RepPlanParcels()
   End Sub
   <CommandMethod("zxx")> Public Sub zxx()
      TestClip.CreateHatch()

   End Sub
   <CommandMethod("zzz")> Public Sub zzz()
      TestClip.InsertBlockA()

   End Sub
   <CommandMethod("zzp")> Public Sub zzp()
      TestClip.InsertBlockB()

   End Sub
   <CommandMethod("zzc")> Public Sub zzc()
      TestClip.CloseT()

   End Sub
   <CommandMethod("zza")> Public Sub zza()
      TestClip.OpenT()

   End Sub
   <CommandMethod("zzW")> Public Sub zzW()
      TestClip.WhatIsIt()

   End Sub

   <CommandMethod("zzr")> Public Sub zzr()
      TestClip.InsertBlockRef()

   End Sub
   <CommandMethod("zzx")> Public Sub zzx()
      TestClip.Test()
   End Sub
   <CommandMethod("zzt")> Public Sub zzt()
      TestClip.InsertClipBlockRef()
   End Sub

   <CommandMethod("TplnClose")> Public Sub TplnClose()
      If mfTopoActions IsNot Nothing Then
         If Not mfTopoActions.IsDisposed Then
            mfTopoActions.Close()
            mfTopoActions.Dispose()
         End If
         mfTopoActions = Nothing
      End If
      If mfTplnView IsNot Nothing Then
         If Not mfTplnView.IsDisposed Then
            mfTplnView.Close()
            mfTplnView.Dispose()
         End If
         mfTplnView = Nothing
      End If

      TPlanGraph.TplnProject.Close()


   End Sub

   <CommandMethod("zzo")> _
    Public Sub zzo()
      Try
         TestClip.TestOffset()
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "zzo")
      End Try


   End Sub
   <CommandMethod("zz")> _
    Public Sub zz()
      Try
         TestClip.TestZebra()
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "zz")
      End Try


   End Sub
   <CommandMethod("xxx")> _
  Public Sub xxx()
      Try
         Dim sMsg As String = Common.GetMenu()
         System.Windows.Forms.MessageBox.Show(sMsg, "Common.GetMenu")
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "xxx")
      End Try
 
      AcadToolbar.AddTest()
   End Sub
   <CommandMethod("xxy")> _
  Public Sub xxy()
      Try

         Autodesk.AutoCAD.ApplicationServices.Application.SetSystemVariable("CTABLESTYLE", "Legend")
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "xxy")
      End Try


   End Sub
   Private Sub mfTopoActions_Calculate() Handles mfTopoActions.Calculate
      If mfTplnView IsNot Nothing Then
         mfTplnView.Reset()
      End If
   End Sub

   Private Sub mfTopoActions_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mfTopoActions.Deactivate
      If mfTopoActions.DialogResult = DialogResult.Yes Then
         mfTopoActions.DialogResult = DialogResult.No
         mfTopoActions.Dispose()
         mfTopoActions = Nothing
         TPlanGraph.TplnProject.Close()

      End If

   End Sub
   Private Class AcadWin
      Implements System.Windows.Forms.IWin32Window
      Private moHandle As System.IntPtr
      Public Sub New(ByVal oHandle As System.IntPtr)
         moHandle = oHandle
      End Sub
      Public ReadOnly Property Handle() As System.IntPtr Implements System.Windows.Forms.IWin32Window.Handle
         Get
            Return moHandle
         End Get
      End Property
   End Class

  
   Private Sub mfTplnView_Deactivate(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mfTplnView.Deactivate
      If mfTplnView.DialogResult = DialogResult.Yes Then
         mfTplnView.DialogResult = DialogResult.No
         mfTplnView.Dispose()
         mfTplnView = Nothing
      End If
   End Sub

   
   Private Sub moAcadDocument_CommandEnded(ByVal oSender As System.Object, ByVal e As Autodesk.AutoCAD.ApplicationServices.CommandEventArgs) Handles moAcadDocument.CommandEnded
      Select Case e.GlobalCommandName
         Case "LAYER"
            mfTopoActions.RefreshLayers()
         Case "MAPTOPOCREATE", "MAPTOPODEL", "MAPTOPOREN"
            mfTopoActions.RefreshTopo()
         Case Else
            ' System.Windows.Forms.MessageBox.Show(e.GlobalCommandName, "29_769")
      End Select

   End Sub

   Private Sub mfTopoActions_FormatChanged() Handles mfTopoActions.FormatChanged
      If mfTplnView IsNot Nothing Then
         mfTplnView.RefreshFormat()
      End If
   End Sub
End Class
