Imports Autodesk.AutoCAD.Interop.Common

Public Class Toolbar
   Public Shared Sub AddToolbar()

      Dim oAcadMenuGroup As AutoCAD.IAcadMenuGrou
      Dim oAcadToolbars As AutoCAD.IAcadToolbars
      Dim oAcadToolbar As AutoCAD.IAcadToolbar
      Dim oScaleAcadToolbar As AutoCAD.IAcadToolbar

      '  Dim sMacros() As String = {"LoadApp", "OpenApp", "DisplayEntity", "DisplayEntityCard", "DisplayTable", "PlotCardSelected", "EditParameters", "CloseApp", "UnloadApp"}


      oAcadMenuGroup = moAutocadApp.MenuGroups.Item(msMenuGroupName)
      oAcadToolbars = oAcadMenuGroup.Toolbars


      Try
         oScaleAcadToolbar = oAcadToolbars.Item(msFloatScalesToolbarName)
         Try
            oScaleAcadToolbar.Delete()
         Catch ex As Exception
            Exit Sub
         End Try
      Catch ex As Exception
         Exit Sub
      End Try
      Try
         oScaleAcadToolbar = oAcadToolbars.Add(msFloatScalesToolbarName)
      Catch ex As Exception
      End Try

      Try
         zzAddToolbarButton(oScaleAcadToolbar, "PlotCardScale100", enCommandType.Run, 2)
         zzAddToolbarButton(oScaleAcadToolbar, "PlotCardScale250", enCommandType.Run, 2)
         zzAddToolbarButton(oScaleAcadToolbar, "PlotCardScale500", enCommandType.Run, 2)
      Catch ex As Exception
      End Try
      oScaleAcadToolbar.Visible = False

      Try
         oAcadToolbar = oAcadToolbars.Item(msMainToolbarName)
         Try
            oAcadToolbar.Delete()
         Catch ex As Exception
            Exit Sub
         End Try
      Catch ex As Exception
      End Try
      Try
         oAcadToolbar = oAcadToolbars.Add(msMainToolbarName)
      Catch ex As Exception
         '''''''' Stop
         Exit Sub
      End Try

      Try
         zzAddToolbarButton(oAcadToolbar, "LoadApp", enCommandType.Load, 1)
         zzAddToolbarButton(oAcadToolbar, "OpenApp", enCommandType.Run, 1)
         zzAddToolbarButton(oAcadToolbar, "DisplayEntity", enCommandType.Run, 1)
         zzAddToolbarButton(oAcadToolbar, "DisplayEntitySet", enCommandType.Run, 1)
         zzAddToolbarButton(oAcadToolbar, "DisplayEntityCard", enCommandType.Run, 1)
         zzAddToolbarButton(oAcadToolbar, "DisplayTable", enCommandType.Run, 1)
         zzAddToolbarButton(oAcadToolbar, "PlotCardSelected", enCommandType.Run, 1, False)
         zzAddToolbarButton(oAcadToolbar, "FlayoutA", enCommandType.Run, 1, True)
         zzAddToolbarButton(oAcadToolbar, "DisplayValves", enCommandType.Run, 1, False)
         zzAddToolbarButton(oAcadToolbar, "ClearDispEntities", enCommandType.Run, 1, False)
         zzAddToolbarButton(oAcadToolbar, "LayerManager", enCommandType.Run, 1, False)
         zzAddToolbarButton(oAcadToolbar, "EditParameters", enCommandType.Run, 1)
         zzAddToolbarButton(oAcadToolbar, "CloseApp", enCommandType.Run, 1)
         zzAddToolbarButton(oAcadToolbar, "UnloadApp", enCommandType.Unload, 1)
      Catch ex As Exception
         '''''''''   Stop
      End Try

      oAcadToolbar.Visible = True
      oAcadMenuGroup.Save(AcMenuFileType.acMenuFileCompiled)
   End Sub
   Private Shared Sub zzAddToolbarButton(ByVal oAcadToolbar As AutoCAD.IAcadToolbar, ByVal sName As String, ByVal iCommandType As enCommandType, ByVal iHelpTheme As Integer, Optional ByVal bFlyout As Boolean = False)
      Dim oAcadToolbarItem As AutoCAD.IAcadToolbarItem
      Dim iMenuItemIndex As Integer

      iMenuItemIndex = oAcadToolbar.Count
      oAcadToolbarItem = oAcadToolbar.AddToolbarButton(iMenuItemIndex, sName, "", Chr(3) & Chr(3) & zzGetAcadCommand(iCommandType, sName), bFlyout)


      '   oAcadToolbarItem.AttachToolbarToFlyout("ACMAP", "SCALES")
      If bFlyout Then
         oAcadToolbarItem.AttachToolbarToFlyout(msMenuGroupName, msFloatScalesToolbarName)
      Else
         oAcadToolbarItem.SetBitmaps(zzGetBitmapPath(sName), zzGetBitmapPath(sName))
      End If
      oAcadToolbarItem.HelpString = IstrResource.GetText(iMenuItemIndex, miFormTheme, iHelpTheme)
      Try
         '     oAcadToolbarItem.TagString = oAcadToolbarItem.HelpString
      Catch ex As Exception
         ''''''''''    Stop
      End Try




   End Sub
End Class
