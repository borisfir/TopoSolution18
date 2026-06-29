Option Explicit On
Option Strict On
 
Imports Autodesk.AutoCAD.DatabaseServices
Imports TopoManager
Public Class AcadTransactionB
	Private Shared moTransaction As Transaction = Nothing
	Friend Shared Sub Start()
		Try
			moTransaction = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction()
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - Start")
		End Try
	End Sub
	Friend Shared ReadOnly Property TransactionExists() As Boolean
		Get
			Return moTransaction IsNot Nothing AndAlso (Not moTransaction.IsDisposed)
		End Get
	End Property
	Friend Shared Sub Terminate()
		If Not moTransaction Is Nothing Then
			Try
				moTransaction.Commit()
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadTransaction - Terminate_1")
				Try
					moTransaction.Abort()
				Catch oExA As Exception
					System.Windows.Forms.MessageBox.Show(oExA.Message, "AcadTransaction - Terminate_2")
				End Try
			Finally
				moTransaction = Nothing
			End Try
		End If
	End Sub

	Public Shared Function AddPolyline(ByVal daVertices() As Double) As Polyline2d
		Dim oCurrentDatabase As Autodesk.AutoCAD.DatabaseServices.Database = HostApplicationServices.WorkingDatabase
		Dim oBlockTable As BlockTable
		Dim oModelBlockTableRecord As BlockTableRecord
		Dim oPolyline2d As Polyline2d = New Polyline2d()
		Try
			oBlockTable = DirectCast(moTransaction.GetObject(oCurrentDatabase.BlockTableId, Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTable)
			oModelBlockTableRecord = DirectCast(moTransaction.GetObject(oBlockTable.Item(BlockTableRecord.ModelSpace), Autodesk.AutoCAD.DatabaseServices.OpenMode.ForRead), BlockTableRecord)
			oModelBlockTableRecord.AppendEntity(oPolyline2d)
			moTransaction.AddNewlyCreatedDBObject(oPolyline2d, True)
			Return oPolyline2d
		Catch oEx As Exception
			AcadDocument.WriteMessage(oEx.Message)
		End Try
		Return Nothing

	End Function
End Class
