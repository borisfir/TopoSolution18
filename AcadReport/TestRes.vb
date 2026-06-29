Option Explicit On
Option Strict On
Public Class TestRes
   Private Const miResourceTheme As TPlServerDB.enResourceTheme = TPlServerDB.enResourceTheme.AcRepContent
   Private miReportID As TplnReportID
   Private msTitle As String
   Private msColHeaders() As String
   Private mdaColWidths() As Double
   ' Private moAcadTable As Autodesk.AutoCAD.DatabaseServices.Table = New Autodesk.AutoCAD.DatabaseServices.Table()

   Private moMainView As System.Data.DataView
   Public Sub New(ByVal iReportID As TplnReportID)
      iReportID = miReportID
   End Sub
   Public Property MainView() As System.Data.DataView
      Get
         Return moMainView
      End Get
      Set(ByVal oValue As System.Data.DataView)
         moMainView = oValue
      End Set
   End Property
  
   Public Sub LoadInfo()
      zzLayout()
   End Sub
   Private Sub zzLayout()
		Dim oResource As TPlServerDB.TPlResource = TPlServerDB.ServerDB.CurrentServerDB.GetResource(miResourceTheme)
      mdaColWidths = oResource.GetDblItem(0)
      Stop
      Dim oHeaderResource As TPlServerDB.TPlResource = oResource.GetChild(0)
      '  Dim oHeader As Section = New Section(oHeaderResource, Autodesk.AutoCAD.DatabaseServices.RowType.DataRow)

      '   mdaColWidths = oResource.ResItems

   End Sub
End Class
