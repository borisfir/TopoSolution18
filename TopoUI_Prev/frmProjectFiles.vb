Imports Autodesk.AutoCAD.DatabaseServices
Public Class frmProjectFiles
   Protected dsDictionaryName As String = "ProjectData"
   Private mdicMain As DBDictionary
   Private mbOpened As Boolean = False
   Private miMode As OpenMode
   Private moPgonSet As FDO.TplnPolygonSet
   Private mcolCentroids As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection
   Private moFDO_Manager As FDO.FDO_Manager
   Protected ptMapThemeData As DMAcadExt.MapThemeData
   Protected ptDissolveMapThemeData As DMAcadExt.MapThemeData
   Private moWorkAreaBound As Polyline

   Private Sub Button1_Click(oSender As System.Object, e As EventArgs) Handles Button1.Click

   End Sub

   Private Sub cmdOK_Click(oSender As System.Object, e As EventArgs) Handles cmdOK.Click
      DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
      DMAcadExt.AcadTransaction.Start()
      OpenDictionary(True, False)

      DMAcadExt.AcadTransaction.Terminate()
      DMAcadExt.AcadDocument.Unlock()
   End Sub
   Private Sub OpenDictionary(ByVal bCreate As Boolean, ByVal bReadOnly As Boolean)
      Dim tPrjDataDicObjID As ObjectId
      Dim dicNamed As Autodesk.AutoCAD.DatabaseServices.DBDictionary
      Dim iNamedMode As OpenMode
      Dim iMode As OpenMode

      If bCreate Then
         iNamedMode = OpenMode.ForWrite
      Else
         iNamedMode = OpenMode.ForRead
      End If
      If bReadOnly Then '
         iMode = OpenMode.ForRead
      Else
         iMode = OpenMode.ForWrite
      End If

      dicNamed = DMAcadExt.AcadTransaction.GetNamedDictionary(iNamedMode)
      'MessageBox.Show(CStr(dicNamed.Count) & ":" & dsDictionaryName, "01_469aa")
      If dicNamed IsNot Nothing Then
         If dicNamed.Contains(dsDictionaryName) Then
            tPrjDataDicObjID = DirectCast(dicNamed.Item(dsDictionaryName), ObjectId)
            mdicMain = DirectCast(DMAcadExt.AcadTransaction.GetDBObject(tPrjDataDicObjID, iMode), Autodesk.AutoCAD.DatabaseServices.DBDictionary)
            mbOpened = True
            miMode = iMode
         ElseIf bCreate Then
            mdicMain = New Autodesk.AutoCAD.DatabaseServices.DBDictionary()
            dicNamed.SetAt(dsDictionaryName, mdicMain)
            mbOpened = True
            DMAcadExt.AcadTransaction.AppendDBObject(mdicMain)
            dicNamed.Contains(dsDictionaryName)
         Else
            DMAcadExt.AcadDocument.WriteMessage(dsDictionaryName & " was not found")
         End If
      Else
         MessageBox.Show("NamedDictionary is nothing " & iNamedMode.ToString())
      End If

   End Sub

   Private Sub cmdPgonset_Click(oSender As System.Object, e As EventArgs) Handles cmdPgonset.Click
      zzCreatePgonSet()
   End Sub
   Private Sub zzCreatePgonSet()

      If moPgonSet Is Nothing Then
         '  MessageBox.Show(Me.Name & vbCrLf & "zzCreatePgonSet", "05_370")
         Dim bCurrentLayerOK As Boolean = False

         '   System.Windows.Forms.MessageBox.Show(ptMapThemeData.MapThemeID.ToString & vbCrLf & ptMapThemeData.TopoPurpose.ToString & vbCrLf & ptMapThemeData.TopoName & vbCrLf & ptMapThemeData.ClosedPgonsLayers, "07_001s")
         moPgonSet = New FDO.TplnPolygonSet(DMAcadExt.enMapTheme.UD_Parcels, DMAcadExt.enTopoPurpose.Parcel, "Parcels")
         '	Dim oEditor As Autodesk.AutoCAD.EditorInput.Editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor
         Dim oEnt As DBObject = Nothing

         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)
         bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer("zzIntersPoints", DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)
         System.Windows.Forms.MessageBox.Show(DMAcadExt.AcadDocument.IsLocked & vbCrLf & "", "07_001")
         TopoManager.TPlanGraph.TplnBlock.Initialize(ptMapThemeData)
         TopoManager.TPlanGraph.TplnParcel.Initialize(ptMapThemeData)
         TopoManager.TPlanGraph.TplnLot.Initialize(ptMapThemeData)
         UnidivNet.UD_Parcel.Initialize(ptMapThemeData)

         Dim colPolygonIDs As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetLinks("C1602_*")
         '
         Dim dicCentroids As System.Collections.Generic.IDictionary(Of ObjectId, BlockReference) = DMAcadExt.AcadTransaction.GetBlockRefsDic(ptMapThemeData.CentroidBlock, OpenMode.ForWrite)   '"pclp004"
         '
         mcolCentroids = DMAcadExt.AcadTransaction.GetBlockRefsNew("C1603", "C1603_*")
         DMAcadExt.AcadDocument.WriteMessage("!!Pgon Count: " & CStr(colPolygonIDs.Count))
         moPgonSet.AddPolylineIDs(colPolygonIDs)
         '?	bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer("TplnParcelMPgon", DMAcadExt.DMApp.AppID, DMAcadExt.enLayerFunction.Default, True, True, False)
         '
         'oPgonSet.AddBlocks(dicCentroids)
         moPgonSet.AddCentroids(mcolCentroids)
         '
         ptMapThemeData.CentroidBlocks = "C1603"

         UnidivNet.UD_Parcel.Initialize(ptMapThemeData)
         TopoManager.TPlanGraph.TplnParcel.Initialize(ptMapThemeData)
         TopoManager.TPlanGraph.TplnLot.Initialize(ptMapThemeData)
         '
         '''''''''''''''''''''''''moPgonSet()
         Dim colPolylines As Autodesk.AutoCAD.DatabaseServices.ObjectIdCollection = DMAcadExt.AcadTransaction.GetEntitiesByLayer(DMAcadExt.MapThemeData.WorkAreaBoundaryLayer)
         If colPolylines IsNot Nothing AndAlso colPolylines.Count > 0 Then
            moWorkAreaBound = DMAcadExt.AcadTransaction.GetPolyline(colPolylines.Item(0), OpenMode.ForRead)

         End If
         moPgonSet.WorkAreaBound = moWorkAreaBound
         
         bCurrentLayerOK = DMAcadExt.AcadTransaction.SetCurrentLayer("zzIntersPoints", DMAcadExt.DMApp.AppID, True, True)

         If bCurrentLayerOK Then
            moPgonSet.CalculateNewF(True)
         End If
         '''''''''''''	


         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.CloseMessage()
         DMAcadExt.AcadDocument.Unlock()

      End If
   End Sub
   Private Sub zzGroupDissolve()
      If moPgonSet IsNot Nothing Then
         ' MessageBox.Show(Me.Name & vbCrLf & "zzGroupDissolve")
         DMAcadExt.AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.Write, True)
         DMAcadExt.AcadTransaction.Start()
         DMAcadExt.AcadTransaction.OpenModelSpace(OpenMode.ForWrite)

         moFDO_Manager = New FDO.FDO_Manager()
         Dim saShapeFileNames() As FDO.FDO_Manager.ShapeFOData = moPgonSet.ExportByGroup
         'moPgonSet.PrintLinkTable()	'

         moFDO_Manager.FOData = saShapeFileNames
         moFDO_Manager.CreateBoundingBoxesMapLayer()
         moFDO_Manager.ConnectAddToMap()
         '	CreateDissolveMapLayer(mcolDisPlines, "WorkArea", "shpWorkArea")
         DMAcadExt.AcadTransaction.CloseModelSpace()
         DMAcadExt.AcadTransaction.Terminate()
         DMAcadExt.AcadDocument.CloseMessage()
         DMAcadExt.AcadDocument.Unlock()
      End If

   End Sub

   Private Sub Button2_Click(oSender As System.Object, e As EventArgs) Handles Button2.Click
      zzGroupDissolve()
   End Sub
   Private Sub zzDissolve()
      '	moFDO_Manager = New FDO.FDO_Manager(msCurrentRootName, msaParcelFolders)
      '  MessageBox.Show(ptDissolveMapThemeData.CentroidBlock & vbCrLf & ptDissolveMapThemeData.CentroidLayer & vbCrLf & ptDissolveMapThemeData.LinkLayer & vbCrLf & ptDissolveMapThemeData.ClosedPgonsLayer, "05_341a")
      If moFDO_Manager IsNot Nothing Then
         ptDissolveMapThemeData.CentroidBlocks = "1601"
         ptDissolveMapThemeData.CentroidLayers = "1601"

         moFDO_Manager.CreateBlocksUnion(ptDissolveMapThemeData.CentroidBlock, ptDissolveMapThemeData.CentroidLayer)
      Else
         MessageBox.Show("FDO_Manager was not found", "01_277")
      End If

   End Sub
   Private Sub zzCreateWorkArea()
      '	moFDO_Manager = New FDO.FDO_Manager(msCurrentRootName, msaParcelFolders)
      'MessageBox.Show(ptDissolveMapThemeData.CentroidBlock & vbCrLf & ptDissolveMapThemeData.CentroidLayer, "05_341")
      If moFDO_Manager IsNot Nothing Then
         moFDO_Manager.CreateWorkArea()
      End If

   End Sub

   Private Sub Button3_Click(oSender As System.Object, e As EventArgs) Handles Button3.Click
      zzDissolve()
   End Sub

   Private Sub Button4_Click(oSender As System.Object, e As EventArgs) Handles Button4.Click
      zzCreateWorkArea()
   End Sub

   Public Sub New()

      ' This call is required by the designer.
      InitializeComponent()

      ' Add any initialization after the InitializeComponent() call.

   End Sub
End Class