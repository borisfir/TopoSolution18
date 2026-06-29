Option Explicit On
Option Strict On

'Imports Autodesk.AutoCAD.ApplicationServices
Imports Autodesk.AutoCAD.Runtime
Imports Autodesk.AutoCAD.DatabaseServices
Imports Autodesk.AutoCAD.Geometry
Imports Autodesk.AutoCAD.GraphicsInterface

Imports Autodesk.AutoCAD.EditorInput
Imports AcDbSymbolUtilities
Imports Autodesk.Gis.Map

'Imports Autodesk.AutoCAD.ApplicationServices.Application

Imports Autodesk.Gis.Map.Utilities
Imports System.Runtime.InteropServices
'Imports DOTNETARX
Public Class TestClip
   Private Const msBlockName As String = "ZebraUnit100"
   Private Shared moTransaction As Transaction = Nothing
   Private Shared WithEvents moBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference
   Private Shared WithEvents moClipBlockRef As testBlockRef

   Public Shared Sub InsertBlockAAAA()
      Const sBlockName As String = "ZebraUnit100A"
      Dim oTransaction As Transaction = Nothing
      Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
      Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
      Dim oBlockTable As BlockTable
      Dim oModelBlockTableRecord As BlockTableRecord
      Dim oNewBlockTableRecord As BlockTableRecord
      Dim oNewBlockObjID As ObjectId
      Dim oAcobjId As ObjectId
      Dim sTest As String = "a"
      Dim oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference
      Dim oInsertPoint As Point3d = New Point3d(4.0, 4.0, 0.0)
      Try
         oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
         oTransaction = oTransactionManager.StartTransaction()
         oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, OpenMode.ForRead, False, False), BlockTable)

         oNewBlockTableRecord = New BlockTableRecord()
         sTest = "ab"

         '   oNewBlockTableRecord.PathName = "C:\aWork____\Block"
         sTest = "b"
         oNewBlockTableRecord.Name = "ZebraUnit100"
         sTest = "c"
         oNewBlockTableRecord.Origin = oInsertPoint
         ' oNewBlockTableRecord.UpgradeOpen()
         sTest = "d"
         If oBlockTable.Has(sBlockName) Then
            oNewBlockObjID = oBlockTable.Item("ZebraUnit100")
            sTest = "da"
            System.Windows.Forms.MessageBox.Show(oNewBlockObjID.ToString(), "????oNewBlockObjID")
         Else
            sTest = "db"
            oBlockTable.UpgradeOpen()
            sTest = "dc"
            oNewBlockObjID = oBlockTable.Add(oNewBlockTableRecord)
         End If
         sTest = "de"

         sTest = "e"
         System.Windows.Forms.MessageBox.Show(oNewBlockTableRecord.Name, "BlockTableRecord.PathName")


      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message & vbCrLf & sTest, "InsertBlock-11")
         Exit Sub
      End Try
      '  oBlockTable.DecomposeForSave(DwgVersion.Current)

      Try
         oTransaction.Commit()
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "DrawTitle")


      Finally
         oTransaction.Dispose()
      End Try

      System.Windows.Forms.MessageBox.Show(oNewBlockTableRecord.PathName, ".PathName-END")

      If oBlockTable.Has("ZebraUnit100") Then
         System.Windows.Forms.MessageBox.Show("Has  ZebraUnit100", "!!!!!!!!!!!")
      End If


      Try
         oTransaction = oTransactionManager.StartTransaction()
         oBlockRef = New Autodesk.AutoCAD.DatabaseServices.BlockReference(oInsertPoint, oNewBlockObjID)
         oModelBlockTableRecord = DirectCast(oTransactionManager.GetObject(oBlockTable(BlockTableRecord.ModelSpace), OpenMode.ForWrite, False), BlockTableRecord)
         sTest = "c"
      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message & vbCrLf & sTest, "InsertBlock-11")
         oTransaction.Commit()
         Exit Sub
      End Try

      sTest = "fx"
      oAcobjId = oModelBlockTableRecord.AppendEntity(oBlockRef)
      sTest = "g"
      oTransactionManager.AddNewlyCreatedDBObject(oBlockRef, True)

      Try
         oTransaction.Commit()
      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message & vbCrLf & sTest, "DrawTitle")
      Finally
         oTransaction.Dispose()
      End Try

   End Sub
   Public Shared Sub InsertBlockA()

      '  Dim oTransaction As Transaction = Nothing
      Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
      Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
      Dim oBlockTable As BlockTable
      Dim oNewBlockTableRecord As BlockTableRecord
      Dim oNewBlockObjID As ObjectId
      Dim sTest As String = "a_"
      Dim oInsertPoint As Point3d = New Point3d(4.0, 4.0, 0.0)
      Dim oText As Autodesk.AutoCAD.DatabaseServices.DBText = New DBText()
      sTest = "xc"
      oText.TextString = "AWA"
      oText.HorizontalMode = TextHorizontalMode.TextMid : sTest = "b"
      oText.VerticalMode = TextVerticalMode.TextBase : sTest = "bb"

      oText.Position = oInsertPoint
      Try
         oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
         moTransaction = oTransactionManager.StartTransaction()
         sTest = "xca"
         oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, OpenMode.ForRead, False), BlockTable)
         sTest = "xcb"
         oNewBlockTableRecord = New BlockTableRecord()
         sTest = "b"
         oNewBlockTableRecord.Name = msBlockName
         sTest = "bb"
         oNewBlockTableRecord.Origin = oInsertPoint
         sTest = "bbc"

         If oBlockTable.Has(msBlockName) Then
            oNewBlockObjID = oBlockTable.Item(msBlockName)
            sTest = "da"
            System.Windows.Forms.MessageBox.Show(oNewBlockObjID.ToString(), "oNewBlockObjID")
         Else
            Common.GetEditor.WriteMessage("L120")
            sTest = "db"
            oBlockTable.UpgradeOpen()
            Common.GetEditor.WriteMessage("L121")
            sTest = "dc"
            oNewBlockObjID = oBlockTable.Add(oNewBlockTableRecord)
            Common.GetEditor.WriteMessage("L122")
            oTransactionManager.AddNewlyCreatedDBObject(oNewBlockTableRecord, True)
         End If
         oNewBlockTableRecord.AppendEntity(oText)
         Common.GetEditor.WriteMessage("L130")
         sTest = "dh"
         oTransactionManager.AddNewlyCreatedDBObject(oText, True)
         sTest = "dk"
         Try
            moTransaction.Commit()
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "DrawTitle")
         Finally
            moTransaction.Dispose()
         End Try

         Common.GetEditor.WriteMessage("L200")
         ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
         sTest = "ga"
         If oBlockTable IsNot Nothing AndAlso oBlockTable.Has(msBlockName) Then
            sTest = "gb"
            System.Windows.Forms.MessageBox.Show("Has " & msBlockName, "!!!!!!!!!!!")
            oNewBlockObjID = oBlockTable.Item(msBlockName)
            sTest = "daz"
            System.Windows.Forms.MessageBox.Show(oNewBlockObjID.ToString(), "oNewBlockObjID")


				Dim oBlockTableRecord As BlockTableRecord
            moTransaction = oTransactionManager.StartTransaction()
            sTest = "dda"
            oBlockTableRecord = DirectCast(oTransactionManager.GetObject(oNewBlockObjID, OpenMode.ForRead, False, False), BlockTableRecord)
            sTest = "ddb"
            Common.GetEditor.WriteMessage("L220")

            Try

               '   oTransaction.Commit()
               Common.GetEditor.WriteMessage("L225")
				Catch oEx As Autodesk.AutoCAD.Runtime.Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "DrawTitlexx")
            Finally
               '    oTransaction.Dispose()
               Common.GetEditor.WriteMessage("L226")
            End Try
         End If

99999:   ''''''''''''''''''''''''''''''' '''''''''''''''''END'''''''''''''''''''''''''''''
         sTest = "de"
         '      oBlockTable.Close()
         sTest = "e"
         System.Windows.Forms.MessageBox.Show(oNewBlockTableRecord.Name, "BlockTableRecord.PathName")
         sTest = "d"
         Common.GetEditor.WriteMessage("L131")
      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message & vbCrLf & sTest, "InsertBlock-11")
         Exit Sub
      End Try
      '  oBlockTable.DecomposeForSave(DwgVersion.Current)

      Try
         sTest = "xa"
         System.Windows.Forms.MessageBox.Show("610")
         '   oNewBlockTableRecord.Close()
         sTest = "xb"
         ' System.Windows.Forms.MessageBox.Show("620")
         '    oBlockTable.Close()
         '  System.Windows.Forms.MessageBox.Show("630")
         sTest = "xc"
         moTransaction.Commit()
         System.Windows.Forms.MessageBox.Show("630")
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "DrawTitle")


      Finally
         moTransaction.Dispose()
      End Try

      System.Windows.Forms.MessageBox.Show(oNewBlockTableRecord.PathName, ".PathName-END")

      If oBlockTable.Has(msBlockName) Then
         System.Windows.Forms.MessageBox.Show("Has  ZebraUnit100", "!!!!!!!!!!!")
      End If

      ' Exit Sub

      Common.GetEditor.WriteMessage("L143")
      ' oTransactionManager.Dispose()
   End Sub
	Public Shared Sub InsertBlockB_AAA()

		'  Dim oTransaction As Transaction = Nothing
		Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oBlockTable As BlockTable
		Dim oNewBlockTableRecord As BlockTableRecord
		Dim oNewBlockObjID As ObjectId
		Dim sTest As String = "a_"
		Dim oInsertPoint As Point3d = New Point3d(4.0, 4.0, 0.0)

		sTest = "xc"

		Try
			oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
			moTransaction = oTransactionManager.StartTransaction()
			sTest = "xca"
			oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, OpenMode.ForRead, False), BlockTable)
			sTest = "xcb"
			oNewBlockTableRecord = New BlockTableRecord()
			sTest = "b"
			oNewBlockTableRecord.Name = msBlockName
			sTest = "bb"
			oNewBlockTableRecord.Origin = oInsertPoint
			sTest = "bbc"

			If oBlockTable.Has(msBlockName) Then
				oNewBlockObjID = oBlockTable.Item(msBlockName)
				sTest = "da"
				System.Windows.Forms.MessageBox.Show(oNewBlockObjID.ToString(), "oNewBlockObjID")
			Else
				Common.GetEditor.WriteMessage("L120")
				sTest = "db"
				oBlockTable.UpgradeOpen()
				Common.GetEditor.WriteMessage("L121")
				sTest = "dc"
				oNewBlockObjID = oBlockTable.Add(oNewBlockTableRecord)
				Common.GetEditor.WriteMessage("L122")
				oTransactionManager.AddNewlyCreatedDBObject(oNewBlockTableRecord, True)
			End If
			oNewBlockTableRecord.PathName = "C:\aWork\Block\ZebraUnit100.dwg"
			Common.GetEditor.WriteMessage("L130")
			sTest = "dh"
			'  oTransactionManager.AddNewlyCreatedDBObject(oText, True)
			sTest = "dk"
			Try
				moTransaction.Commit()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "DrawTitle")
			Finally
				moTransaction.Dispose()
			End Try

			Common.GetEditor.WriteMessage("L200")
			''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
			sTest = "ga"
			If oBlockTable IsNot Nothing AndAlso oBlockTable.Has(msBlockName) Then
				sTest = "gb"
				System.Windows.Forms.MessageBox.Show("Has " & msBlockName, "!!!!!!!!!!!")
				oNewBlockObjID = oBlockTable.Item(msBlockName)
				sTest = "daz"
				System.Windows.Forms.MessageBox.Show(oNewBlockObjID.ToString(), "oNewBlockObjID")


				Dim oBlockTableRecord As BlockTableRecord
				moTransaction = oTransactionManager.StartTransaction()
				sTest = "dda"
				oBlockTableRecord = DirectCast(oTransactionManager.GetObject(oNewBlockObjID, OpenMode.ForRead, False, False), BlockTableRecord)
				sTest = "ddb"
				Common.GetEditor.WriteMessage("L220")

				Try

					'   oTransaction.Commit()
					Common.GetEditor.WriteMessage("L225")
				Catch oEx As Exception

					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "DrawTitlexx")
				Finally
					'    oTransaction.Dispose()
					Common.GetEditor.WriteMessage("L226")
				End Try
			End If

			''''''''''''''''''''''''''''''''''''''''''''''''END'''''''''''''''''''''''''''''
			sTest = "de"
			'      oBlockTable.Close()
			sTest = "e"
			System.Windows.Forms.MessageBox.Show(oNewBlockTableRecord.Name, "BlockTableRecord.PathName")
			sTest = "d"
			Common.GetEditor.WriteMessage("L131")
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "InsertBlock-11")
			Exit Sub
		End Try
		'  oBlockTable.DecomposeForSave(DwgVersion.Current)

		Try
			sTest = "xa"
			System.Windows.Forms.MessageBox.Show("610")
			'   oNewBlockTableRecord.Close()
			sTest = "xb"
			' System.Windows.Forms.MessageBox.Show("620")
			'    oBlockTable.Close()
			'  System.Windows.Forms.MessageBox.Show("630")
			sTest = "xc"
			moTransaction.Commit()
			System.Windows.Forms.MessageBox.Show("630")
		Catch oEx As System.Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "DrawTitle")

		Finally
			moTransaction.Dispose()
		End Try

		System.Windows.Forms.MessageBox.Show(oNewBlockTableRecord.PathName, ".PathName-END")

		If oBlockTable.Has(msBlockName) Then
			System.Windows.Forms.MessageBox.Show("Has  ZebraUnit100", "!!!!!!!!!!!")
		End If

		' Exit Sub

		Common.GetEditor.WriteMessage("L143")
		' oTransactionManager.Dispose()
	End Sub
   Public Shared Sub InsertBlockBAAA()
      Const sBlockName As String = "ZZZZ"
      Dim oTransaction As Transaction = Nothing
      Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
      Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
      Dim oBlockTable As BlockTable
      Dim oNewBlockTableRecord As BlockTableRecord
      Dim oNewBlockObjID As ObjectId
      Dim sTest As String = "a"
      Dim oInsertPoint As Point3d = New Point3d(4.0, 4.0, 0.0)

      Try
         oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
         oTransaction = oTransactionManager.StartTransaction()
         oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, OpenMode.ForRead, True, True), BlockTable)

         oNewBlockTableRecord = New BlockTableRecord()
         sTest = "ab"

         '   oNewBlockTableRecord.PathName = "C:\aWork____\Block"
         sTest = "b"
         oNewBlockTableRecord.Name = sBlockName
         sTest = "c"
         oNewBlockTableRecord.Origin = oInsertPoint
         ' oNewBlockTableRecord.UpgradeOpen()


         If oBlockTable.Has(sBlockName) Then
            oNewBlockObjID = oBlockTable.Item(sBlockName)
            sTest = "da"
            System.Windows.Forms.MessageBox.Show(oNewBlockObjID.ToString(), "oNewBlockObjID")
         Else
            Common.GetEditor.WriteMessage("L120")
            sTest = "db"
            oBlockTable.UpgradeOpen()
            Common.GetEditor.WriteMessage("L121")
            sTest = "dc"
            oNewBlockObjID = oBlockTable.Add(oNewBlockTableRecord)
            Common.GetEditor.WriteMessage("L122")
         End If
         Try
            oTransaction.Commit()
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "DrawTitle")
         Finally
            oTransaction.Dispose()
         End Try

         sTest = "de"
         '      oBlockTable.Close()
         sTest = "e"
         System.Windows.Forms.MessageBox.Show(oNewBlockTableRecord.Name, "BlockTableRecord.Name")

         '    oTransaction = oTransactionManager.StartTransaction()

         sTest = "d"

         Common.GetEditor.WriteMessage("L131")

      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message & vbCrLf & sTest, "InsertBlock-11")
         oBlockTable = Nothing
      End Try



      If oBlockTable IsNot Nothing AndAlso oBlockTable.Has(sBlockName) Then
         System.Windows.Forms.MessageBox.Show("Has " & sBlockName, "!!!!!!!!!!!")
         oNewBlockObjID = oBlockTable.Item(sBlockName)
         sTest = "da"
         System.Windows.Forms.MessageBox.Show(oNewBlockObjID.ToString(), "oNewBlockObjID")


         Dim oBlockTableRecord As BlockTableRecord
         oTransaction = oTransactionManager.StartTransaction()
         oBlockTableRecord = DirectCast(oTransactionManager.GetObject(oNewBlockObjID, OpenMode.ForRead, False, False), BlockTableRecord)
         sTest = "ab"

         '   oNewBlockTableRecord.PathName = "C:\aWork_\Block"
         sTest = "b"

			Try
				'    oTransaction.Commit()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "DrawTitle")


			Finally
				'   oTransaction.Dispose()
			End Try
      Else
         System.Windows.Forms.MessageBox.Show("?????", "Err348")
      End If

   End Sub
   Public Shared Sub OpenT()
      Dim sTest As String
      Dim oBlockTable As BlockTable
      Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = moTransaction.TransactionManager
      Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
      Dim oNewBlockObjID As ObjectId
      moTransaction = oTransactionManager.StartTransaction()
      oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, OpenMode.ForRead, True, True), BlockTable)
      If oBlockTable IsNot Nothing AndAlso oBlockTable.Has(msBlockName) Then
         sTest = "gb"
         System.Windows.Forms.MessageBox.Show("Has " & msBlockName, "!!!!!!!!!!!")
         oNewBlockObjID = oBlockTable.Item(msBlockName)
         sTest = "daz"
         System.Windows.Forms.MessageBox.Show(oNewBlockObjID.ToString(), "oNewBlockObjID")


         Dim oBlockTableRecord As BlockTableRecord

         sTest = "dda"
         oBlockTableRecord = DirectCast(oTransactionManager.GetObject(oNewBlockObjID, OpenMode.ForRead, False, False), BlockTableRecord)
         sTest = "ddb"
         Common.GetEditor.WriteMessage("L220")
      End If
   End Sub
   Public Shared Sub CloseT()
      Try
         moTransaction.Commit()
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf, "CloseT")
      Finally
         moTransaction.Dispose()
      End Try
   End Sub
   Public Shared Sub InsertBlockRef()
      Dim sTest As String = "A"
      Dim oBlockTable As BlockTable

      Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager
      oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
      moTransaction = oTransactionManager.StartTransaction()
      Dim oModelBlockTableRecord As BlockTableRecord

      Dim oInsertPoint As Point3d = New Point3d(8.0, 4.0, 0.0)
      Dim oBlockObjID As ObjectId
      Dim oAcObjId As ObjectId
      Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
      System.Windows.Forms.MessageBox.Show("100", "InsertBlockRef")
      Try
         oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, OpenMode.ForRead, True, True), BlockTable)
         If oBlockTable IsNot Nothing AndAlso oBlockTable.Has(msBlockName) Then
            sTest = "gb"
            System.Windows.Forms.MessageBox.Show("Has " & msBlockName, "!!!!!!!!!!!")
            oBlockObjID = oBlockTable.Item(msBlockName)
            sTest = "daz"
            System.Windows.Forms.MessageBox.Show(oBlockObjID.ToString(), "oNewBlockObjID")
         End If
         moBlockRef = New Autodesk.AutoCAD.DatabaseServices.BlockReference(oInsertPoint, oBlockObjID)
         oModelBlockTableRecord = DirectCast(oTransactionManager.GetObject(oBlockTable(BlockTableRecord.ModelSpace), OpenMode.ForWrite, False), BlockTableRecord)
         sTest = "c"
      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message & vbCrLf & sTest, "InsertBlock-11")
         Exit Sub
      End Try

      sTest = "fx"
		System.Windows.Forms.MessageBox.Show(moBlockRef.BlockId.ToString(), "before AppendEntity")
      oAcObjId = oModelBlockTableRecord.AppendEntity(moBlockRef)
		System.Windows.Forms.MessageBox.Show(moBlockRef.BlockId.ToString() & ":" & oAcObjId.ToString, "before AppendEntity")
      sTest = "g"
      oTransactionManager.AddNewlyCreatedDBObject(moBlockRef, True)
   End Sub
   Public Shared Sub InsertClipBlockRef()
      Dim sTest As String = "A"
      Dim oBlockTable As BlockTable

      Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager
      oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
      moTransaction = oTransactionManager.StartTransaction()
      Dim oModelBlockTableRecord As BlockTableRecord

      Dim oInsertPoint As Point3d = New Point3d(-116.0, -104.0, 0.0)
      Dim oBlockObjID As ObjectId
      Dim oAcObjId As ObjectId
      Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
      System.Windows.Forms.MessageBox.Show("202a", "InsertBlockRef")
      Try
         oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, OpenMode.ForRead, True, True), BlockTable)
         If oBlockTable IsNot Nothing AndAlso oBlockTable.Has(msBlockName) Then
            sTest = "gb"
            System.Windows.Forms.MessageBox.Show("Has " & msBlockName, "!!!!!!!!!!!")
            oBlockObjID = oBlockTable.Item(msBlockName)
            sTest = "daz"
            System.Windows.Forms.MessageBox.Show(oBlockObjID.ToString(), "oNewBlockObjID")
         End If

			System.Windows.Forms.MessageBox.Show(oBlockObjID.ToString(), "220-InsertBlockRef")
			If oBlockObjID.IsNull Then Exit Sub
         moClipBlockRef = New testBlockRef(oInsertPoint, oBlockObjID)
         System.Windows.Forms.MessageBox.Show("230", "InsertBlockRef")
         oModelBlockTableRecord = DirectCast(oTransactionManager.GetObject(oBlockTable(BlockTableRecord.ModelSpace), OpenMode.ForWrite, False), BlockTableRecord)
         sTest = "c"
         System.Windows.Forms.MessageBox.Show("240", "InsertBlockRef")
      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message & vbCrLf & sTest, "InsertBlock-11")
         Exit Sub
      End Try

      sTest = "fx"
		System.Windows.Forms.MessageBox.Show(moClipBlockRef.BlockId.ToString(), "before AppendEntity")
      oAcObjId = oModelBlockTableRecord.AppendEntity(moClipBlockRef)
		System.Windows.Forms.MessageBox.Show(moClipBlockRef.BlockId.ToString() & ":" & oAcObjId.ToString(), "After AppendEntity")
      sTest = "g"
      oTransactionManager.AddNewlyCreatedDBObject(moClipBlockRef, True)
      System.Windows.Forms.MessageBox.Show(CStr(moClipBlockRef.GetMyName()), "GetMyName")
      moClipBlockRef.Draw()
      System.Windows.Forms.MessageBox.Show("END")
   End Sub
   Public Shared Sub Test()

      '  Dim line As Lines = New Lines(New Point3d(0, 0, 0), New Point3d(50, 50, 0))
      '  Dim circle As Circles = New Circles(New Point3d(50, 50, 0), 25)

      Dim btr1 As BlockTableRecord = New BlockTableRecord() ''Creates a new block table record named block1
      btr1.Name = "ZebraUnit100"
      '  btr1.Origin = Circle.Center
      '    btr1.PathName = "C:\aWork\Block\ZebraUnit100.dwg"
      btr1.PathName = "C:\aWork\Block"

      Try
         btr1.UpgradeOpen()
      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message, "UpgradeOpen-137")
      End Try

      '  Dim oEntities() As Entity = {line, circle}
      '  Tools.AddBlockTableRecord(btr1, oEntities)
      '   Tools.AddBlockTableRecord(btr1)

   End Sub
   Public Shared Sub worldDrawA(ByVal pDraw As WorldDraw)

      Dim pGeom As WorldGeometry = pDraw.Geometry()
      Dim myTransform As Matrix3d
      Dim colPoint As Point2dCollection = New Point2dCollection()
      pGeom.PushModelTransform(myTransform)

      Dim cb As ClipBoundary = New ClipBoundary()
      cb.DrawBoundary = True
      cb.NormalVector = Vector3d.ZAxis
      cb.Point = Point3d.Origin
      '  // Two points treated as a rectangle, three creates a triangle
      colPoint.Add(New Point2d(0, 0))
      colPoint.Add(New Point2d(5, 5))

      cb.SetAptPoints(colPoint)
      ' // We are clipping in our own space

      ' cb.m_ToClipSpace.setToIdentity()
      Dim oM3d As Matrix3d = cb.TransformToClipSpace()
      '  oM3d.
      cb.TransformInverseBlockRefXForm = myTransform.Inverse()

      '  // No Z clipping
      cb.ClippingBack = False
      cb.ClippingFront = False
      cb.BackClipZ = 0.0
      cb.FrontClipZ = 0.0
      Dim bPopClipBoundary As Boolean = pGeom.PushClipBoundary(cb)

      ' // Draw something

      pGeom.Circle(New Point3d(3.0, 4.0, 0.0), New Point3d(4.0, 7.0, 0.0), New Point3d(5.0, 9.0, 0.0))

      pGeom.PopModelTransform()
      If bPopClipBoundary Then
         pGeom.PopClipBoundary()
      End If

      ' // world-only


   End Sub
   Dim oZebraLine As New LineSegment2d(New Point2d(1.0, 1.0), New Vector2d(1.0, 2.0))
   Public Shared Sub TestZebra()
      Const iPlineDim As Integer = 7
      Dim oaCurves(iPlineDim) As Curve2d
      Dim dLeft As Double = 2.0, dRight As Double = 12.0, dSplit1 As Double = 5.0, dSplit2 As Double = 9.0
      Dim dTop As Double = 8.0, dBootom As Double = 4.0
      Dim oComposite As CompositeCurve2d
      Dim sText As String
      Dim oIntersect As CurveCurveIntersector2d
      Dim oaPoints(iPlineDim + 1) As Point2d
      ''''''''''''''''''''''''''''''''''''' oaPoints(iPlineDim + 1)
      ''     oaCurves(0) = New LineSegment2d(New Point2d(dLeft, dTop), New Point2d(dRight, dTop))
      ''   oaCurves(1) = New LineSegment2d(New Point2d(dRight, dTop), New Point2d(dRight, dBootom))

      ''   oaCurves(2) = New LineSegment2d(New Point2d(dRight, dBootom), New Point2d(dLeft, dBootom))
      '    oaCurves(3) = New LineSegment2d(New Point2d(dLeft, dBootom), New Point2d(dLeft, dTop - 0.00001))
      ''   oaCurves(3) = New LineSegment2d(New Point2d(dLeft, dBootom), New Point2d(dLeft, dTop))
      oaCurves(0) = New LineSegment2d(New Point2d(dLeft, dTop), New Point2d(dSplit1, dTop))
      oaCurves(1) = New LineSegment2d(New Point2d(dSplit1, dTop), New Point2d(dSplit1, 0.25 * (dTop + 3 * dBootom)))
      oaCurves(2) = New LineSegment2d(New Point2d(dSplit1, 0.25 * (dTop + 3 * dBootom)), New Point2d(dSplit2, 0.25 * (dTop + 3 * dBootom)))
      oaCurves(3) = New LineSegment2d(New Point2d(dSplit2, 0.25 * (dTop + 3 * dBootom)), New Point2d(dSplit2, dTop))
      oaCurves(4) = New LineSegment2d(New Point2d(dSplit2, dTop), New Point2d(dRight, dTop))
      oaCurves(5) = New LineSegment2d(New Point2d(dRight, dTop), New Point2d(dRight, dBootom))


      oaCurves(6) = New LineSegment2d(New Point2d(dRight, dBootom), New Point2d(dLeft, dBootom))
      '    oaCurves(3) = New LineSegment2d(New Point2d(dLeft, dBootom), New Point2d(dLeft, dTop - 0.00001))
      oaCurves(7) = New LineSegment2d(New Point2d(dLeft, dBootom), New Point2d(dLeft, dTop))

      zzDispCurves(oaCurves, "Source")
      DrawCurves.Draw(oaCurves)
      oComposite = New CompositeCurve2d(oaCurves)

      '  Dim oZebraLine As New LineSegment2d(New Point2d(1.0, 1.0), New Vector2d(1.0, 2.0))
      '  Dim oZebraLine As New LineSegment2d(New Point2d(1.0, 1.0), New Point2d(7.0, 11.0))
      Dim oaInterval() As Interval
      Dim oZebraLineA As LineSegment2d = New LineSegment2d(New Point2d(6.0, 3.0), New Point2d(10.0, 10.0)) 'kasatel
      ' Dim oZebraLine As New LineSegment2d(New Point2d(3.0, 3.0), New Point2d(5.0, 8.0))
      Dim oZebraLineB As LineSegment2d = New LineSegment2d(New Point2d(7.5, 3.0), New Point2d(11.5, 10.0))
      Dim oPointOnCurve As PointOnCurve2d
      Dim oClipBoundary As ClipBoundary2d = New ClipBoundary2d(New Point2d(1.0, 1.0), New Point2d(7.0, 11.0))
      DrawCurves.DrawSingle(oZebraLineA)
      DrawCurves.DrawSingle(oZebraLineB)

      Try
         Dim oTolerance As Tolerance = New Tolerance(0.01, 0.01)
         '''''''''''''''''''''''''''       oClipBoundary.ClipPolygon(
         oIntersect = New CurveCurveIntersector2d(oComposite, oZebraLineA)
         sText = "NumberOfIntersectionPoints: " & CStr(oIntersect.NumberOfIntersectionPoints) & vbCrLf
         Dim daValue() As Double
         Dim oPoint As Point2d
         Dim dVal As Double
         sText &= "OverlapCount: " & CStr(oIntersect.OverlapCount) & vbCrLf
         For iIndex As Integer = 0 To oIntersect.NumberOfIntersectionPoints - 1
            daValue = oIntersect.GetIntersectionParameters(iIndex)
            zzDispArray(daValue, "IntersectPar #" & iIndex.ToString())
            oPoint = oIntersect.GetIntersectionPoint(iIndex)
            zzDispPoint(oPoint, "IntersectPoint #" & iIndex.ToString())
            AcadReport.AcadUtil.GetEditor().WriteMessage(CStr(oIntersect.GetIntersectionPointTolerance(iIndex)) & vbCrLf)
            oPointOnCurve = oIntersect.GetPointOnCurve1(iIndex)
            dVal = oPointOnCurve.Parameter
            AcadReport.AcadUtil.GetEditor().WriteMessage("oPointOnCurve.Parameter=" & CStr(dVal) & vbCrLf)
            oPoint = oPointOnCurve.GetPointAtParameter(0)
            zzDispPoint(oPoint, "GetPointAtParameter #" & iIndex.ToString())

         Next

         Dim oInterval As Interval = New Interval(1)

         oaInterval = oIntersect.GetIntersectionRanges()
         sText &= "IntersectionRanges: " & CStr(oaInterval.GetUpperBound(0)) & vbCrLf
 
         For iIndex As Integer = 0 To 1
            Try
               sText &= "Interval(" & iIndex & ").Element: " & CStr(oaInterval(iIndex).Element) & vbCrLf
               daValue = oaInterval(iIndex).GetBounds()
               zzDispArray(daValue, "Interval(" & iIndex & ").GetBounds")
               dVal = oaInterval(iIndex).LowerBound()
               sText &= "Interval(" & iIndex & ").LowerBound: " & CStr(dVal) & vbCrLf
               dVal = oaInterval(iIndex).UpperBound()
               sText &= "Interval(" & iIndex & ").UpperBound: " & CStr(dVal) & vbCrLf
               dVal = oaInterval(iIndex).Length
               sText &= "Interval(" & iIndex & ").Length: " & CStr(dVal) & vbCrLf
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "Interval " & CStr(iIndex))
            End Try

         Next

         AcadReport.AcadUtil.GetEditor().WriteMessage(sText)
      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message, "1 Extend")
      End Try




   End Sub
   Public Shared Sub TestZebraA()
      Const iPlineDim As Integer = 7
      Dim oaCurves(iPlineDim) As Curve2d
      Dim dLeft As Double = 2.0, dRight As Double = 12.0, dSplit1 As Double = 5.0, dSplit2 As Double = 9.0
      Dim dTop As Double = 8.0, dBootom As Double = 4.0
      Dim oComposite As CompositeCurve2d
      Dim sText As String
      Dim oIntersect As CurveCurveIntersector2d
      ''     oaCurves(0) = New LineSegment2d(New Point2d(dLeft, dTop), New Point2d(dRight, dTop))
      ''   oaCurves(1) = New LineSegment2d(New Point2d(dRight, dTop), New Point2d(dRight, dBootom))

      ''   oaCurves(2) = New LineSegment2d(New Point2d(dRight, dBootom), New Point2d(dLeft, dBootom))
      '    oaCurves(3) = New LineSegment2d(New Point2d(dLeft, dBootom), New Point2d(dLeft, dTop - 0.00001))
      ''   oaCurves(3) = New LineSegment2d(New Point2d(dLeft, dBootom), New Point2d(dLeft, dTop))
      oaCurves(0) = New LineSegment2d(New Point2d(dLeft, dTop), New Point2d(dSplit1, dTop))
      oaCurves(1) = New LineSegment2d(New Point2d(dSplit1, dTop), New Point2d(dSplit1, 0.25 * (dTop + 3 * dBootom)))
      oaCurves(2) = New LineSegment2d(New Point2d(dSplit1, 0.25 * (dTop + 3 * dBootom)), New Point2d(dSplit2, 0.25 * (dTop + 3 * dBootom)))
      oaCurves(3) = New LineSegment2d(New Point2d(dSplit2, 0.25 * (dTop + 3 * dBootom)), New Point2d(dSplit2, dTop))
      oaCurves(4) = New LineSegment2d(New Point2d(dSplit2, dTop), New Point2d(dRight, dTop))
      oaCurves(5) = New LineSegment2d(New Point2d(dRight, dTop), New Point2d(dRight, dBootom))


      oaCurves(6) = New LineSegment2d(New Point2d(dRight, dBootom), New Point2d(dLeft, dBootom))
      '    oaCurves(3) = New LineSegment2d(New Point2d(dLeft, dBootom), New Point2d(dLeft, dTop - 0.00001))
      oaCurves(7) = New LineSegment2d(New Point2d(dLeft, dBootom), New Point2d(dLeft, dTop))

      zzDispCurves(oaCurves, "Source")
      DrawCurves.Draw(oaCurves)
      oComposite = New CompositeCurve2d(oaCurves)

      '  Dim oZebraLine As New LineSegment2d(New Point2d(1.0, 1.0), New Vector2d(1.0, 2.0))
      '  Dim oZebraLine As New LineSegment2d(New Point2d(1.0, 1.0), New Point2d(7.0, 11.0))
      Dim oaInterval() As Interval
      Dim oZebraLineA As LineSegment2d = New LineSegment2d(New Point2d(6.0, 3.0), New Point2d(10.0, 10.0)) 'kasatel
      ' Dim oZebraLine As New LineSegment2d(New Point2d(3.0, 3.0), New Point2d(5.0, 8.0))
      Dim oZebraLineB As LineSegment2d = New LineSegment2d(New Point2d(7.5, 3.0), New Point2d(11.5, 10.0))
      Dim oPointOnCurve As PointOnCurve2d
      DrawCurves.DrawSingle(oZebraLineA)
      DrawCurves.DrawSingle(oZebraLineB)

      Try
         Dim oTolerance As Tolerance = New Tolerance(0.01, 0.01)
         oIntersect = New CurveCurveIntersector2d(oComposite, oZebraLineA)
         sText = "NumberOfIntersectionPoints: " & CStr(oIntersect.NumberOfIntersectionPoints) & vbCrLf
         Dim daValue() As Double
         Dim oPoint As Point2d
         Dim dVal As Double
         sText &= "OverlapCount: " & CStr(oIntersect.OverlapCount) & vbCrLf
         For iIndex As Integer = 0 To oIntersect.NumberOfIntersectionPoints - 1
            daValue = oIntersect.GetIntersectionParameters(iIndex)
            zzDispArray(daValue, "IntersectPar #" & iIndex.ToString())
            oPoint = oIntersect.GetIntersectionPoint(iIndex)
            zzDispPoint(oPoint, "IntersectPoint #" & iIndex.ToString())
            AcadReport.AcadUtil.GetEditor().WriteMessage(CStr(oIntersect.GetIntersectionPointTolerance(iIndex)) & vbCrLf)
            oPointOnCurve = oIntersect.GetPointOnCurve1(iIndex)
            dVal = oPointOnCurve.Parameter
            AcadReport.AcadUtil.GetEditor().WriteMessage("oPointOnCurve.Parameter=" & CStr(dVal) & vbCrLf)
            oPoint = oPointOnCurve.GetPointAtParameter(0)
            zzDispPoint(oPoint, "GetPointAtParameter #" & iIndex.ToString())

         Next

         Dim oInterval As Interval = New Interval(1)

         oaInterval = oIntersect.GetIntersectionRanges()
         sText &= "IntersectionRanges: " & CStr(oaInterval.GetUpperBound(0)) & vbCrLf

         For iIndex As Integer = 0 To 1
            Try
               sText &= "Interval(" & iIndex & ").Element: " & CStr(oaInterval(iIndex).Element) & vbCrLf
               daValue = oaInterval(iIndex).GetBounds()
               zzDispArray(daValue, "Interval(" & iIndex & ").GetBounds")
               dVal = oaInterval(iIndex).LowerBound()
               sText &= "Interval(" & iIndex & ").LowerBound: " & CStr(dVal) & vbCrLf
               dVal = oaInterval(iIndex).UpperBound()
               sText &= "Interval(" & iIndex & ").UpperBound: " & CStr(dVal) & vbCrLf
               dVal = oaInterval(iIndex).Length
               sText &= "Interval(" & iIndex & ").Length: " & CStr(dVal) & vbCrLf
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message, "Interval " & CStr(iIndex))
            End Try

         Next

         AcadReport.AcadUtil.GetEditor().WriteMessage(sText)
      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message, "1 Extend")
      End Try




   End Sub
   Public Shared Sub TestOffset()
      Const iPlineDim As Integer = 3
      Dim oaCurves(iPlineDim) As Curve2d
      Dim dLeft As Double = 1.0, dRight As Double = 7.0
      Dim dTop As Double = 10.0, dBootom As Double = 4.0
      Dim oComposite As CompositeCurve2d
      Dim oOffsetCurves() As Curve2d
      oaCurves(0) = New LineSegment2d(New Point2d(dLeft, dTop), New Point2d(dRight, dTop))
      oaCurves(1) = New LineSegment2d(New Point2d(dRight, dTop), New Point2d(dRight, dBootom))

      oaCurves(2) = New LineSegment2d(New Point2d(dRight, dBootom), New Point2d(dLeft, dBootom))
      oaCurves(3) = New LineSegment2d(New Point2d(dLeft, dBootom), New Point2d(dLeft, dTop - 0.00001))
      zzDispCurves(oaCurves, "Source")
      DrawCurves.Draw(oaCurves)
      oComposite = New CompositeCurve2d(oaCurves)

      Try
         Dim oTolerance As Tolerance = New Tolerance(0.01, 0.01)
         oOffsetCurves = oComposite.GetTrimmedOffset(1, OffsetCurveExtensionType.Extend)
         zzDispCurves(oOffsetCurves, "Offset 1 Extend")
      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message, "1 Extend")
      End Try
      Try
         oOffsetCurves = oComposite.GetTrimmedOffset(1, OffsetCurveExtensionType.Fillet)
         zzDispCurves(oOffsetCurves, "Offset 1 Fillet")
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message, "1 Fillet")
      End Try
      Try
         oOffsetCurves = oComposite.GetTrimmedOffset(1, OffsetCurveExtensionType.Chamfer)
         zzDispCurves(oOffsetCurves, "Offset 1 Chamfer")

      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message, "1 Chamfer")
      End Try
      Try
         oOffsetCurves = oComposite.GetTrimmedOffset(-1, OffsetCurveExtensionType.Extend)
         zzDispCurves(oOffsetCurves, "Offset -1 Extend")
         DrawCurves.Draw(oOffsetCurves)
      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message, "-1  Extend")
      End Try
      Try
         oOffsetCurves = oComposite.GetTrimmedOffset(-1, OffsetCurveExtensionType.Chamfer)
         zzDispCurves(oOffsetCurves, "Offset -1  Chamfer")
      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message, "-1  Chamfer")
      End Try
      Try
         oOffsetCurves = oComposite.GetTrimmedOffset(-1, OffsetCurveExtensionType.Fillet)
         zzDispCurves(oOffsetCurves, "Offset -1  Fillet")
      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message, "-1  Fillet")
      End Try


   End Sub
   Public Shared Sub TestOffsetA()

      Dim oTransaction As Transaction = Nothing
      Dim oTransactionManager As TransactionManager = Nothing
      Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
      Dim oBlockTable As BlockTable
      Dim oBlockTableRecord As BlockTableRecord

      Dim oAcobjId As ObjectId
      Dim sTest As String = "a"
      Try
         oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
         oTransaction = oTransactionManager.StartTransaction()
         oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, OpenMode.ForWrite, False, False), BlockTable)
         oBlockTableRecord = DirectCast(oTransactionManager.GetObject(oBlockTable(BlockTableRecord.ModelSpace), OpenMode.ForWrite, False), BlockTableRecord)



         Dim oText As Autodesk.AutoCAD.DatabaseServices.DBText = New DBText()
         sTest = "c"

         oText.HorizontalMode = TextHorizontalMode.TextMid : sTest = "b"
         oText.VerticalMode = TextVerticalMode.TextBase : sTest = "bb"
         Try


            Try
               sTest = "bx"

               'New Point3d(oInsertPoint.X + 5, oInsertPoint.Y, 0.0)
            Catch ex As Exception
               System.Windows.Forms.MessageBox.Show(ex.Message & vbCrLf & sTest, "DrawTitle AlignmentPoint")
            End Try

            sTest = "by"
            Dim oTestPointA As Point3d = oText.AlignmentPoint
            ''   System.Windows.Forms.MessageBox.Show(CStr(oTestPointA.X) & ":" & CStr(oTestPointA.Y), "AlignmentPoint X:Y")
            Dim oTestPointP As Point3d = oText.Position
            ''   System.Windows.Forms.MessageBox.Show(CStr(oTestPointP.X) & ":" & CStr(oTestPointP.Y), "Positiont X:Y")

         Catch ex As Exception
            System.Windows.Forms.MessageBox.Show(ex.Message & vbCrLf & sTest, "DrawTitle TextPoint")
         End Try


         sTest = "d"
         oText.Visible = True
         sTest = "e"





         Try

         Catch ex As Exception
            System.Windows.Forms.MessageBox.Show(ex.Message & vbCrLf & sTest, "DrawTitle TextStyle-2")
         End Try



         ' oText.AdjustAlignment(oCurrentDatabase)
         sTest = "fx"
         oAcobjId = oBlockTableRecord.AppendEntity(oText)
         sTest = "g"
         oTransactionManager.AddNewlyCreatedDBObject(oText, True)
         Try
            sTest = "fz"
            oText.UpgradeOpen()

         Catch ex As Exception
            System.Windows.Forms.MessageBox.Show(ex.Message & vbCrLf & sTest, "DrawTitle=11")
         End Try
         Try
            sTest = "k"
            oText.Draw()
         Catch ex As Exception
            System.Windows.Forms.MessageBox.Show(ex.Message & vbCrLf & sTest, "DrawTitle=DRAW")
         End Try

         sTest = "L"
         oTransaction.Commit()
      Catch ex As Exception
         System.Windows.Forms.MessageBox.Show(ex.Message & vbCrLf & sTest, "DrawTitle")
      Finally
         oTransaction.Dispose()
      End Try
   End Sub
   Public Shared Sub TestDraw()
      Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager
      oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
      moTransaction = oTransactionManager.StartTransaction()




      System.Windows.Forms.MessageBox.Show("", "oWorldDraw Is Nothing")
   End Sub
   Public Shared Sub GetClipBlockRef()
      Dim sTest As String = "A"
      Dim oBlockTable As BlockTable
      Dim oModelBlockTableRecord As BlockTableRecord
      Dim oEntity As Entity
      Dim oInsertPoint As Point3d = New Point3d(-116.0, -104.0, 0.0)
      Dim oBlockObjID As ObjectId
      Dim oAcObjId As ObjectId
      Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
      Dim sRXClassName As String
      Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager
      oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
      moTransaction = oTransactionManager.StartTransaction()
      oModelBlockTableRecord = DirectCast(oTransactionManager.GetObject(oBlockTable(BlockTableRecord.ModelSpace), OpenMode.ForWrite, False), BlockTableRecord)
      For Each objId As ObjectId In oModelBlockTableRecord


         oEntity = DirectCast(moTransaction.GetObject(objId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
         sRXClassName = oEntity.GetRXClass().Name
         Select Case sRXClassName
            Case Common.AcadPolylineName, Common.AcadLWPolylineName
            Case Common.AcadBlockRefName

         End Select
      Next

   End Sub
   Public Shared Sub InsertUnderlay()
      Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
      Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
      Dim oBlockTable As BlockTable
      Dim oNewBlockTableRecord As BlockTableRecord
      Dim oUnderlayDef As UnderlayDefinition
      Dim oNewBlockObjID As ObjectId
      Dim sTest As String = "a_"
      Dim oInsertPoint As Point3d = New Point3d(4.0, 4.0, 0.0)
      Dim oUnderlayFile As UnderlayFile
      Dim oDgnDef As DgnDefinition = New DgnDefinition()
      Dim oDwfDef As DwfDefinition = New DwfDefinition()
      oDwfDef.SourceFileName = "C:\aWork\Block\ZebraUnit100.dwf"
      oDwfDef.Load("")


      Dim oText As Autodesk.AutoCAD.DatabaseServices.DBText = New DBText()
      sTest = "xc"
      oText.TextString = "AWA"
      oText.HorizontalMode = TextHorizontalMode.TextMid : sTest = "b"
      oText.VerticalMode = TextVerticalMode.TextBase : sTest = "bb"
      oText.Position = oInsertPoint
      '  ISM_RASTER_IMAGE_DICT()


      Try

      Catch ex As Exception

      End Try
      oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
      moTransaction = oTransactionManager.StartTransaction()
      sTest = "xca"
      oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, OpenMode.ForRead, False), BlockTable)
      sTest = "xcb"
      oNewBlockTableRecord = New BlockTableRecord()
      sTest = "b"
      oNewBlockTableRecord.Name = msBlockName
      sTest = "bb"
      oNewBlockTableRecord.Origin = oInsertPoint
      sTest = "bbc"

      If oBlockTable.Has(msBlockName) Then
         oNewBlockObjID = oBlockTable.Item(msBlockName)
         sTest = "da"
         System.Windows.Forms.MessageBox.Show(oNewBlockObjID.ToString(), "oNewBlockObjID")
      Else
         Common.GetEditor.WriteMessage("L120")
         sTest = "db"
         oBlockTable.UpgradeOpen()
         Common.GetEditor.WriteMessage("L121")
         sTest = "dc"
         oNewBlockObjID = oBlockTable.Add(oNewBlockTableRecord)
         Common.GetEditor.WriteMessage("L122")
         oTransactionManager.AddNewlyCreatedDBObject(oNewBlockTableRecord, True)
      End If
      oNewBlockTableRecord.AppendEntity(oText)
      Common.GetEditor.WriteMessage("L130")
      sTest = "dh"
      oTransactionManager.AddNewlyCreatedDBObject(oText, True)
      sTest = "dk"
      Try
         moTransaction.Commit()
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "DrawTitle")
      Finally
         moTransaction.Dispose()
      End Try

   End Sub
   Public Shared Sub WhatIsIt()
      Dim oModelBlockTableRecord As BlockTableRecord
      Dim oEntity As Entity
      Dim sRXClassName As String
      Dim oBlockTable As BlockTable
      Dim oBlockRef As BlockReference
      Dim oPoint3dCollection As Point3dCollection
      Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager
      Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
      oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
      moTransaction = oTransactionManager.StartTransaction()
      oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, OpenMode.ForRead, False, False), BlockTable)
      oModelBlockTableRecord = DirectCast(oTransactionManager.GetObject(oBlockTable(BlockTableRecord.ModelSpace), OpenMode.ForWrite, False), BlockTableRecord)
      For Each objId As ObjectId In oModelBlockTableRecord
         oEntity = DirectCast(moTransaction.GetObject(objId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), Entity)
         sRXClassName = oEntity.GetRXClass().Name
         Select Case sRXClassName
            Case Common.AcadPolylineName, Common.AcadLWPolylineName
            Case Common.AcadBlockRefName
               oBlockRef = DirectCast(oEntity, BlockReference)
               System.Windows.Forms.MessageBox.Show(oEntity.GetType().ToString())

               '  oBlockRef.SetStretchPoints()
         End Select
      Next
      CloseT()
   End Sub
   Public Shared Sub CreateHatch()

      Dim oHatch As Autodesk.AutoCAD.DatabaseServices.Hatch = New Autodesk.AutoCAD.DatabaseServices.Hatch()
      Dim oTransaction As Autodesk.AutoCAD.DatabaseServices.Transaction = Nothing
      Dim oTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
      Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = Autodesk.AutoCAD.DatabaseServices.HostApplicationServices.WorkingDatabase
      Dim oBlockTable As Autodesk.AutoCAD.DatabaseServices.BlockTable
      Dim oBlockTableRecord As Autodesk.AutoCAD.DatabaseServices.BlockTableRecord
      Dim oAcobjId As Autodesk.AutoCAD.DatabaseServices.ObjectId
      Dim sTest As String = "a"
      Dim oHatchLoop As Autodesk.AutoCAD.DatabaseServices.HatchLoop
		Dim oDocLock As Autodesk.AutoCAD.ApplicationServices.DocumentLock
		oHatchLoop = New Autodesk.AutoCAD.DatabaseServices.HatchLoop(HatchLoopTypes.Polyline Or HatchLoopTypes.External)
		Dim oaBulgeVertices(4) As Autodesk.AutoCAD.DatabaseServices.BulgeVertex
      Dim vertexPts As Point2dCollection = New Point2dCollection
      Dim vertexBulges As DoubleCollection = New DoubleCollection
      vertexPts.Add(New Point2d(2.0, 2.0))
      vertexPts.Add(New Point2d(8.0, 2.0))
      vertexPts.Add(New Point2d(8.0, 8.0))
      vertexPts.Add(New Point2d(2.0, 8.0))
		vertexPts.Add(New Point2d(2.0, 2.0))
		vertexBulges.Add(1.0)
		vertexBulges.Add(-0.5)
		vertexBulges.Add(1.0)
		vertexBulges.Add(1.0)
		vertexBulges.Add(2.0)
      For i As Integer = 0 To 4
			oaBulgeVertices(i) = New BulgeVertex(vertexPts.Item(i), vertexBulges(i))
      Next
      For i As Integer = 0 To 4
			oHatchLoop.Polyline.Add(oaBulgeVertices(i))
		Next
		System.Windows.Forms.MessageBox.Show(CStr(oHatchLoop.Polyline.Count), "350")

		Dim oInnerHatchLoop As Autodesk.AutoCAD.DatabaseServices.HatchLoop
		Dim oInnerHatchLoopA As Autodesk.AutoCAD.DatabaseServices.HatchLoop

		oInnerHatchLoop = New Autodesk.AutoCAD.DatabaseServices.HatchLoop(HatchLoopTypes.Default Or HatchLoopTypes.Outermost)
		oInnerHatchLoopA = New Autodesk.AutoCAD.DatabaseServices.HatchLoop(HatchLoopTypes.Polyline)

		oInnerHatchLoopA.Polyline.Add(New BulgeVertex(New Point2d(4.0, 4.0), 0.3))
		oInnerHatchLoopA.Polyline.Add(New BulgeVertex(New Point2d(6.0, 4.0), 0.3))
		oInnerHatchLoopA.Polyline.Add(New BulgeVertex(New Point2d(6.0, 6.0), 0.3))
		oInnerHatchLoopA.Polyline.Add(New BulgeVertex(New Point2d(4.0, 6.0), 0.3))
		oInnerHatchLoopA.Polyline.Add(New BulgeVertex(New Point2d(4.0, 4.0), 0.3))
		System.Windows.Forms.MessageBox.Show("", "359")
      Dim cenPt As Point2d = New Point2d(5.0, 5.0)

		Dim dTwoPi As Double = 2.0 * 3.1415926535897931

		Dim cirArc As CircularArc2d = New CircularArc2d(cenPt, 1.0)
		Dim oCurve2d As Curve2d = cirArc
		'oInnerHatchLoop.Curves.Add(cirArc)

		oInnerHatchLoop.Curves.Add(New Line2d(vertexPts.Item(0), vertexPts.Item(1)))
		oInnerHatchLoop.Curves.Add(New Line2d(vertexPts.Item(1), vertexPts.Item(2)))
		oInnerHatchLoop.Curves.Add(New Line2d(vertexPts.Item(2), vertexPts.Item(3)))
		oInnerHatchLoop.Curves.Add(New Line2d(vertexPts.Item(3), vertexPts.Item(0)))


		If oInnerHatchLoop.Curves IsNot Nothing Then
			System.Windows.Forms.MessageBox.Show(CStr(oInnerHatchLoop.Curves.Count), "oInnerHatchLoop.Curves Count!!!222")

		End If
		Try
			System.Windows.Forms.MessageBox.Show("", "501")
			Dim oNormal As Vector3d = New Vector3d(0.0, 0.0, 1.0)


			oHatch.Normal = oNormal
			oHatch.Elevation = 0.0
			oHatch.Associative = False
			oHatch.ColorIndex = 5

			oHatch.SetHatchPattern(Autodesk.AutoCAD.DatabaseServices.HatchPatternType.PreDefined, "SOLID")
			oHatch.HatchStyle = Autodesk.AutoCAD.DatabaseServices.HatchStyle.Normal

			oTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
			oTransaction = oTransactionManager.StartTransaction()
			System.Windows.Forms.MessageBox.Show("", "510")
			AcadDocument.DocLock(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, True)

			Try
				oDocLock = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.LockDocument(Autodesk.AutoCAD.ApplicationServices.DocumentLockMode.ExclusiveWrite, "CommandLine", "", True)
			Catch oAcadEx As Autodesk.AutoCAD.Runtime.Exception
				AcadErrCode.ShowAcadError(oAcadEx.ErrorStatus, False, "TestClip-GetAttribDef-")
				Exit Sub
			End Try
			oBlockTable = DirectCast(oTransactionManager.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False, False), Autodesk.AutoCAD.DatabaseServices.BlockTable)
			oBlockTableRecord = DirectCast(oTransactionManager.GetObject(oBlockTable(Autodesk.AutoCAD.DatabaseServices.BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForWrite, False), Autodesk.AutoCAD.DatabaseServices.BlockTableRecord)


			Try
				sTest = "ga"
				sTest = "gb"


				If oInnerHatchLoop.Curves IsNot Nothing Then
					System.Windows.Forms.MessageBox.Show(oInnerHatchLoop.Curves.Count.ToString(), "530 Curves.Count")
				End If

				System.Windows.Forms.MessageBox.Show(oInnerHatchLoop.IsPolyline.ToString(), "531")
				System.Windows.Forms.MessageBox.Show(oInnerHatchLoop.LoopType.ToString(), "532")
				If oInnerHatchLoop.Polyline IsNot Nothing Then
					sTest = "gba"
					System.Windows.Forms.MessageBox.Show(oInnerHatchLoop.Polyline.Count.ToString(), "533 Polyline.Count")
				End If

			Catch oEx As System.Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=2")
			End Try
			sTest = "gbb"
			Try
				Try
					oHatch.AppendLoop(oInnerHatchLoopA)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=25a")
				End Try
				sTest = "gxx"
				System.Windows.Forms.MessageBox.Show(oHatch.Area.ToString(), "566")
				Try
					oHatch.AppendLoop(oInnerHatchLoop)
				Catch oEx As Exception
					System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=25b")
				End Try
				sTest = "gxy"
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=25x")
			End Try
			
			Try

				System.Windows.Forms.MessageBox.Show("After AppendLoop", "579a")
				sTest = "gcf"
				'	oHatch.AppendLoop(oHatchLoop)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=22s")
			End Try
			''	oHatch.AppendLoop(oHatchLoop)
			sTest = "gd"
			System.Windows.Forms.MessageBox.Show("After InnerAppendLoop", "579b")

			'''''' ''''''''   oHatch.AppendLoop(Autodesk.AutoCAD.DatabaseServices.HatchLoopTypes.Default, colInnerLinks)
			sTest = "gx"
			System.Windows.Forms.MessageBox.Show("", "579c")
			oHatch.EvaluateHatch(True)
			System.Windows.Forms.MessageBox.Show("", "590")


			oAcobjId = oBlockTableRecord.AppendEntity(oHatch)
			System.Windows.Forms.MessageBox.Show("", "540")
			sTest = "g"
			oTransactionManager.AddNewlyCreatedDBObject(oHatch, True)
			System.Windows.Forms.MessageBox.Show("", "550")
			Try
				oHatch.UpgradeOpen()

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=11")
			End Try
			System.Windows.Forms.MessageBox.Show("", "979")
			oHatch.Associative = True
			System.Windows.Forms.MessageBox.Show(CStr(oHatch.Associative), "980")
			oTransaction.Commit()
			System.Windows.Forms.MessageBox.Show("", "592")
			AcadDocument.Unlock()
			System.Windows.Forms.MessageBox.Show("", "592a")
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "zzAddHatch=1")
		Finally
			oTransaction.Dispose()
		End Try
   End Sub
   Private Shared Sub zzDispCurves(ByVal oaCurves() As Autodesk.AutoCAD.Geometry.Curve2d, ByVal sTitle As String)
      Dim sText As String
      Dim sType As String
      Dim oCompositeCurve As Autodesk.AutoCAD.Geometry.CompositeCurve2d
      AcadReport.AcadUtil.GetEditor().WriteMessage(sTitle & vbCrLf)
      sText = "Curves " & oaCurves.GetUpperBound(0).ToString() & vbCrLf
      AcadReport.AcadUtil.GetEditor().WriteMessage(sText)
      For iIndex As Integer = 0 To oaCurves.GetUpperBound(0)
         zzDispPoint(oaCurves(iIndex).StartPoint, "StartPoint " & iIndex.ToString())
         zzDispPoint(oaCurves(iIndex).EndPoint, "EndPoint " & iIndex.ToString())
         sType = oaCurves(iIndex).GetType().ToString()
         sText = sType & vbCrLf
         AcadReport.AcadUtil.GetEditor().WriteMessage(sText)
         If sType = "Autodesk.AutoCAD.Geometry.CompositeCurve2d" Then
            oCompositeCurve = DirectCast(oaCurves(iIndex), Autodesk.AutoCAD.Geometry.CompositeCurve2d)
            AcadReport.AcadUtil.GetEditor().WriteMessage("Composite No " & CStr(iIndex) & vbCrLf)
            zzDispCurves(oCompositeCurve.GetCurves(), sTitle & CStr(iIndex))

         End If
      Next
      AcadReport.AcadUtil.GetEditor().WriteMessage("-----------------" & vbCrLf)
   End Sub
   Private Shared Sub zzDispPoint(ByVal oPoint As Autodesk.AutoCAD.Geometry.Point2d, ByVal sName As String)
      Dim sText As String = sName & ": " & CStr(oPoint.X) & "," & CStr(oPoint.Y) & vbCrLf
      AcadReport.AcadUtil.GetEditor().WriteMessage(sText)
   End Sub
   Private Shared Sub zzDispArray(ByVal daValue() As Double, ByVal sName As String)
      Dim sText As String = sName & vbCrLf
      For iIndex As Integer = 0 To daValue.GetUpperBound(0)
         If iIndex <> 0 Then sText &= ";"
         sText &= daValue(iIndex).ToString()
      Next
      sText &= vbCrLf
      AcadReport.AcadUtil.GetEditor().WriteMessage(sText)
   End Sub
   Private Class DrawCurves
      Private Shared moTransaction As Transaction = Nothing
      Private Shared moTransactionManager As Autodesk.AutoCAD.DatabaseServices.TransactionManager = Nothing
      Private Shared moCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
      Private Shared oBlockTable As BlockTable
      Private Shared moModelBlockTableRecord As BlockTableRecord
      Private Shared moPolyline As Polyline
      Public Shared Sub DrawSingle(ByVal oCurve As Autodesk.AutoCAD.Geometry.Curve2d)



         Dim oNewBlockObjID As ObjectId
         Dim oAcobjId As ObjectId
         Dim sTest As String = "a"
         Dim oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference
         Dim oInsertPoint As Point3d = New Point3d(4.0, 4.0, 0.0)
         Try
            moTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
            moTransaction = moTransactionManager.StartTransaction()
            oBlockTable = DirectCast(moTransactionManager.GetObject(moCurrentDatabase.BlockTableId, OpenMode.ForRead, False, False), BlockTable)

            moModelBlockTableRecord = DirectCast(moTransactionManager.GetObject(oBlockTable(BlockTableRecord.ModelSpace), OpenMode.ForWrite, False), BlockTableRecord)


            moPolyline = New Polyline()

         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "InsertBlock-11")
            Exit Sub
         End Try

         zzDrawCurve(oCurve)

         oAcobjId = moModelBlockTableRecord.AppendEntity(moPolyline)
         sTest = "g"
         moTransactionManager.AddNewlyCreatedDBObject(moPolyline, True)
         Try
            sTest = "fz"
            moPolyline.UpgradeOpen()

         Catch ex As Exception
            System.Windows.Forms.MessageBox.Show(ex.Message & vbCrLf & sTest, "DrawTitle=11")
         End Try


         Try
            moTransaction.Commit()
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "DrawTitle")


         Finally
            moTransaction.Dispose()
         End Try

      End Sub
      Public Shared Sub Draw(ByVal oaCurves() As Autodesk.AutoCAD.Geometry.Curve2d)



         Dim oNewBlockObjID As ObjectId
         Dim oAcobjId As ObjectId
         Dim sTest As String = "a"
         Dim oBlockRef As Autodesk.AutoCAD.DatabaseServices.BlockReference
         Dim oInsertPoint As Point3d = New Point3d(4.0, 4.0, 0.0)
         Try
            moTransactionManager = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager
            moTransaction = moTransactionManager.StartTransaction()
            oBlockTable = DirectCast(moTransactionManager.GetObject(moCurrentDatabase.BlockTableId, OpenMode.ForRead, False, False), BlockTable)

            moModelBlockTableRecord = DirectCast(moTransactionManager.GetObject(oBlockTable(BlockTableRecord.ModelSpace), OpenMode.ForWrite, False), BlockTableRecord)


            moPolyline = New Polyline()

         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "InsertBlock-11")
            Exit Sub
         End Try

         zzDrawCurves(oaCurves)

         oAcobjId = moModelBlockTableRecord.AppendEntity(moPolyline)
         sTest = "g"
         moTransactionManager.AddNewlyCreatedDBObject(moPolyline, True)
         Try
            sTest = "fz"
            moPolyline.UpgradeOpen()

         Catch ex As Exception
            System.Windows.Forms.MessageBox.Show(ex.Message & vbCrLf & sTest, "DrawTitle=11")
         End Try


         Try
            moTransaction.Commit()
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTest, "DrawTitle")


         Finally
            moTransaction.Dispose()
         End Try

      End Sub

      Private Shared Sub zzDrawCurve(ByVal oCurve As Autodesk.AutoCAD.Geometry.Curve2d)
         Dim sType As String = oCurve.GetType().ToString()
         Select Case sType
            Case "Autodesk.AutoCAD.Geometry.CompositeCurve2d"
               Dim oCompositeCurve As Autodesk.AutoCAD.Geometry.CompositeCurve2d = _
DirectCast(oCurve, CompositeCurve2d)
               Dim oaCurves() As Autodesk.AutoCAD.Geometry.Curve2d = oCompositeCurve.GetCurves()
               zzDrawCurves(oaCurves)
            Case "Autodesk.AutoCAD.Geometry.LineSegment2d"
               If moPolyline.NumberOfVertices = 0 Then
                  moPolyline.AddVertexAt(moPolyline.NumberOfVertices, oCurve.StartPoint, 0.0, 0.0, 0.0)
               End If
               moPolyline.AddVertexAt(moPolyline.NumberOfVertices, oCurve.EndPoint, 0.0, 0.0, 0.0)

         End Select
      End Sub
      



      Private Shared Sub zzDrawCurves(ByVal oaCurves() As Autodesk.AutoCAD.Geometry.Curve2d)
         For iIndex As Integer = 0 To oaCurves.GetUpperBound(0)
            zzDrawCurve(oaCurves(iIndex))
         Next
      End Sub


   End Class
  
End Class



