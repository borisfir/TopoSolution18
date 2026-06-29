Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.Interop
Public Class AcadToolbar
	Private Shared moAutocadApp As AcadApplication
   Private Enum enCommandType
      VBALoad
      NetLoad
      Run
      AcadCommand
      Unload
   End Enum
   Private Const msAutocadClassName As String = "AutoCAD.Application.16"
   Private Const msAutocadMapClassName As String = "AutoCADMap.Application.3"
   Private Const msAcadCommandRun As String = "-VBARun"
   Private Const msAcadCommandLoad As String = "-VBALoad"
   Private Const msCommandNetLoad As String = "-NetLoad"

   Private Const msAcadCommandUnload As String = "VBAUnload"
   Private Const msMenuGroupName As String = "ACMAP"
   Private Const msMainToolbarName As String = "TPlanner"
   ' Private Const msFloatScalesToolbarName As String = "Scales"

   Public Shared Sub AddToolbar()

		Dim oAcadMenuGroup As IAcadMenuGroup
		Dim oAcadToolbars As IAcadToolbars
		Dim oAcadToolbar As IAcadToolbar
      '  Dim oScaleAcadToolbar As AutoCAD.IAcadToolbar
      zzOpenAcadMap()
      Try
         oAcadMenuGroup = moAutocadApp.MenuGroups.Item(msMenuGroupName)
         oAcadToolbars = oAcadMenuGroup.Toolbars
      Catch oEx As System.Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadToolbar-zzAddToolbarButton-0")
         Exit Sub
      End Try
      



      Try
         oAcadToolbar = oAcadToolbars.Item(msMainToolbarName)
         Try
            oAcadToolbar.Delete()
         Catch oEx As System.Exception
            Exit Sub
         End Try
      Catch oEx As System.Exception
      End Try
      Try
         oAcadToolbar = oAcadToolbars.Add(msMainToolbarName)
      Catch oEx As System.Exception
         '''''''' Stop
         Exit Sub
      End Try

      Try
         zzAddToolbarButton(oAcadToolbar, "LoadApp", enCommandType.NetLoad, 1)
         zzAddToolbarButton(oAcadToolbar, "OpenTopoForm", enCommandType.AcadCommand, 1)
         zzAddToolbarButton(oAcadToolbar, "Cleanup", enCommandType.AcadCommand, 1)
         zzAddToolbarButton(oAcadToolbar, "BuildTopo", enCommandType.AcadCommand, 1)
         zzAddToolbarButton(oAcadToolbar, "KillTopo", enCommandType.AcadCommand, 1)
         zzAddToolbarButton(oAcadToolbar, "CloseApp", enCommandType.AcadCommand, 1)

      Catch oEx As System.Exception
         '''''''''   Stop
         Exit Sub
      End Try

      oAcadToolbar.Visible = True
		oAcadMenuGroup.Save(Autodesk.AutoCAD.Interop.Common.AcMenuFileType.acMenuFileCompiled)
   End Sub
   Public Shared Sub AddTest()
      Dim oMenuItemCollection As Autodesk.AutoCAD.Windows.MenuItemCollection
      Dim oMenuItem As MenuItem
      Dim oMenuBar As Object
      Try
         oMenuBar = Autodesk.AutoCAD.ApplicationServices.Application.MenuBar()
         System.Windows.Forms.MessageBox.Show(oMenuBar.ToString(), "MenuBar")
         System.Windows.Forms.MessageBox.Show((oMenuBar.GetType().ToString()), "MenuBar-Type")
      Catch oEx As System.Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "!!!oMenuBar-1")
         Exit Sub
      End Try
      Try
         oMenuItem = CType(oMenuBar, MenuItem)
         System.Windows.Forms.MessageBox.Show(oMenuItem.ToString(), "MenuItem")
      Catch oEx As System.Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "!!!oMenuItem-12")
      End Try
      Try
         Dim oMenuGroups As Object = Autodesk.AutoCAD.ApplicationServices.Application.MenuGroups()
         System.Windows.Forms.MessageBox.Show(oMenuGroups.ToString(), "MenuGroups")
         System.Windows.Forms.MessageBox.Show(oMenuGroups.GetType().ToString(), "MenuGroups-Type")
         oMenuItemCollection = CType(oMenuGroups, Autodesk.AutoCAD.Windows.MenuItemCollection)
         System.Windows.Forms.MessageBox.Show(CStr(oMenuItemCollection.Count), "MenuItemCollection.Count")
      Catch oEx As System.Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "!!!!!!!oMenuGroups-2")
      End Try

      ' Dim oMenuCol As Autodesk.AutoCAD.Windows.MenuItemCollection
      '   oMenuCol = New Autodesk.AutoCAD.Windows.MenuItemCollection(

      Try
         Dim oMenuItem1 As Autodesk.AutoCAD.Windows.MenuItem = New Autodesk.AutoCAD.Windows.MenuItem("XXX")
         Dim oMenuItem2 As Autodesk.AutoCAD.Windows.MenuItem = New Autodesk.AutoCAD.Windows.MenuItem("YYY")
         Dim oMenuItem3 As Autodesk.AutoCAD.Windows.MenuItem = New Autodesk.AutoCAD.Windows.MenuItem("ZZZ")


         Dim oMenu As Autodesk.AutoCAD.Windows.Menu
         oMenu = New Autodesk.AutoCAD.Windows.MenuItem("AAA")
         oMenu.MenuItems.Add(oMenuItem1)
         oMenu.MenuItems.Add(oMenuItem2)
         oMenu.MenuItems.Add(oMenuItem3)

      Catch oEx As System.Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "!!!oMenuGroups-3")
      End Try

   End Sub
	Private Shared Sub zzAddToolbarButton(ByVal oAcadToolbar As IAcadToolbar, ByVal sName As String, ByVal iCommandType As enCommandType, ByVal iHelpTheme As Integer, Optional ByVal bFlyout As Boolean = False)
		Dim oAcadToolbarItem As IAcadToolbarItem
		Dim iMenuItemIndex As Integer

		iMenuItemIndex = oAcadToolbar.Count
		oAcadToolbarItem = oAcadToolbar.AddToolbarButton(iMenuItemIndex, sName, "", Chr(3) & Chr(3) & zzGetAcadCommand(iCommandType, sName), bFlyout)


		'   oAcadToolbarItem.AttachToolbarToFlyout("ACMAP", "SCALES")

		oAcadToolbarItem.SetBitmaps(zzGetBitmapPath(sName), zzGetBitmapPath(sName))
		oAcadToolbarItem.HelpString = zzGetText(iMenuItemIndex, iHelpTheme)
		Try
			'     oAcadToolbarItem.TagString = oAcadToolbarItem.HelpString
		Catch oEx As System.Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadToolbar-zzAddToolbarButton")
		End Try




	End Sub
   Private Shared Function zzGetAcadCommand(ByVal iCommandType As enCommandType, ByVal sName As String) As String
      Dim sCommand As String
      Dim sDelimSp As String = Microsoft.VisualBasic.Space(1)
      Dim sDelimQ As String = """"

      Select Case iCommandType
         Case enCommandType.VBALoad
            sCommand = msAcadCommandLoad & sDelimSp & sDelimQ & Common.GetNetAppPath & sDelimQ & sDelimSp

         Case enCommandType.NetLoad
            sCommand = msCommandNetLoad & sDelimSp & sDelimQ & Common.GetNetAppPath & sDelimQ & sDelimSp
         Case enCommandType.Run
            sCommand = msAcadCommandRun & sDelimSp & sName & sDelimSp
         Case enCommandType.AcadCommand
            sCommand = sName

         Case enCommandType.Unload
            sCommand = msAcadCommandUnload & sDelimSp & sDelimQ & Common.GetNetAppPath & sDelimQ & sDelimSp
         Case Else
            sCommand = String.Empty
      End Select
      Return sCommand
   End Function
   Private Shared Function zzGetBitmapPath(ByVal sName As String) As String
      Return Common.ToolbarIconFolder & "\" & sName & ".bmp"
   End Function
   Private Shared Function zzGetText(ByVal iItemID As Integer, ByVal iSectionID As Integer) As String
      Return TPlServerDB.TextResource.GetText(iItemID, TPlServerDB.enResourceTheme.AcApplication, iSectionID)

   End Function
   Private Shared Sub zzOpenAcadMap()

      '  Dim poAutocadApp As IAcadApplication

      Dim iErrAcad As Integer
      On Error Resume Next
		moAutocadApp = CType(GetObject(, msAutocadClassName), AcadApplication)
      iErrAcad = Err.Number
      Err.Clear()
      If iErrAcad = 429 Then
			moAutocadApp = CType(GetObject("", msAutocadClassName), AcadApplication)
         iErrAcad = Err.Number
      End If

      System.Windows.Forms.MessageBox.Show(CStr(moAutocadApp Is Nothing), "AcadToolbar-zzOpenAcadMap")
   End Sub

End Class
