Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices

Public Enum enLayerFunction
	[Default] = 0
	Topology = 1
	CleanupErrMarks = 2
	Report = 3
	TopoLink = 4
	OverlayCentroid = 5
	OverlayFDO_LotKParcel = 6
   OverlayFDO_LotMParcel = 7
   OverlayFDO_MerhLotKParcel = 8
   [ErrorMarks] = 10
	PaintHatchAppr = 11
	PaintHatchProp = 12
	PaintHatchDefault = 11
	PaintHatchK = 13
	PaintHatchM = 14
   PaintHatchParcel = 15
	PaintHatchPgons = 16
   ParcelCP_Erased = 17
   LanduseK = 18
   LanduseM = 19
   PaintPLineAppr = 21
	PaintPLineProp = 22

	PaintParagraph19 = 24
	PaintLeasing = 25
	PaintParagraph126 = 26
	PaintSharedHouse = 27
	BamashPaint = 31
	TitleBlockEarly = 41
	TitleBlockTemp = 42
	TitleBlockFinal = 43
	Legend = 44



End Enum
Public Structure AcadLayerDef
   Const NameDelim As String = "_"
   Private msName As String
   Private msTemplate As String
   Private miNameLen As Integer
   Private miAppID As Integer
   Private miLayerFunction As enLayerFunction
   Private moAcadColor As Autodesk.AutoCAD.Colors.Color
   Private miLineWeight As Autodesk.AutoCAD.DatabaseServices.LineWeight
   Private mbOn As Boolean
   Private mbCorrect As Boolean
   Private mbExists As Boolean
   Public Sub New(ByVal iAppID As Integer, ByVal sLayer As String, Optional ByVal sTemplate As String = Nothing)
      miAppID = iAppID
      msName = sLayer
      msTemplate = sTemplate
      zzNew(True)
   End Sub
   Public Sub New(ByVal iAppID As Integer, ByVal iLayerFunction As enLayerFunction)
      miAppID = iAppID
      miLayerFunction = iLayerFunction
      zzNew(False)
   End Sub
   Public ReadOnly Property AppID() As Integer
      Get
         Return miAppID
      End Get
   End Property
   Public ReadOnly Property Name() As String
      Get
         Return msName
      End Get
   End Property
   Public ReadOnly Property NamePlusExtension(iNumber As Integer) As String
      Get
         If iNumber <> 0 Then
            Return msName & NameDelim & Convert.ToString(iNumber)
         Else
            Return msName
         End If

      End Get
   End Property
   Public ReadOnly Property AcadColor() As Autodesk.AutoCAD.Colors.Color
      Get
         Return moAcadColor
      End Get
   End Property
   Public ReadOnly Property LineWeight() As Autodesk.AutoCAD.DatabaseServices.LineWeight
      Get
         Return miLineWeight
      End Get
   End Property
   Public ReadOnly Property Correct() As Boolean
      Get
         Return mbCorrect
      End Get
   End Property
   Public ReadOnly Property [On]() As Boolean
      Get
         Return mbOn
      End Get
   End Property
   Public ReadOnly Property Exists() As Boolean
      Get
         Return mbCorrect
      End Get
   End Property
   Public Function Contains(sLayerName As String) As Boolean
      If miNameLen = 0 Then
         miNameLen = msName.Length
      End If
      If sLayerName.StartsWith(msName) Then
         If sLayerName.Length > miNameLen Then
            If sLayerName.Substring(miNameLen, 1) = NameDelim Then
               Return True
            Else
               Return False
            End If
         Else
            Return True
         End If
      Else
         Return False
      End If


      

   End Function
   Private Sub zzNew(ByVal bByName As Boolean)
      Const sSQLWhereNameFldName As String = "UPPER(LayerName)"
      Const sJetWhereNameFldName As String = "UCase(LayerName)"
      Const sNameFldName As String = "LayerName"
      Const sFunctionFldName As String = "[Function]"

      Dim sWhereNameFldName As String
      Select Case TPlServerDB.ServerDB.CurrentServerDB.Provider
         Case TPlServerDB.TPlProvider.ProviderJet
            sWhereNameFldName = sJetWhereNameFldName
         Case TPlServerDB.TPlProvider.ProviderSQLServer
            sWhereNameFldName = sSQLWhereNameFldName
         Case Else
            sWhereNameFldName = Nothing
      End Select

      Dim sSelectFldName As String
      Dim sWhereFldName As String
      Dim sCondition As String
      mbCorrect = True
      If bByName Then
         sSelectFldName = sFunctionFldName
         sWhereFldName = sWhereNameFldName
         If msTemplate Is Nothing Then
            sCondition = "'" & msName.ToUpper() & "'"
         Else
            sCondition = "'" & msTemplate.ToUpper() & "'"
         End If


      Else 'ByFunction
         sSelectFldName = sNameFldName
         sWhereFldName = sFunctionFldName
         sCondition = CStr(CInt(miLayerFunction))
      End If
		Dim sComText As String = "SELECT " & sSelectFldName & ",Argb,AcadColor,LineWeight,[On] FROM LayerDefs WHERE ((AppID=0) OR (AppID=" & CStr(miAppID) & ")) AND (" & sWhereFldName & "=" & sCondition & ")"
		Dim oDataReader As System.Data.Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)

      If oDataReader IsNot Nothing Then
         Try
            If oDataReader.Read Then
               If bByName Then
                  If [Enum].IsDefined(GetType(enLayerFunction), oDataReader.GetInt32(0)) Then
                     miLayerFunction = CType(oDataReader.GetInt32(0), enLayerFunction)
                  End If
               Else
                  msName = oDataReader.GetString(0)
               End If

               If Not oDataReader.IsDBNull(1) Then
						moAcadColor = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.FromArgb(oDataReader.GetInt32(1)))

					ElseIf Not oDataReader.IsDBNull(2) Then
                  moAcadColor = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.None, oDataReader.GetInt16(2))
               End If
               If Not oDataReader.IsDBNull(3) Then
                  If [Enum].IsDefined(GetType(Autodesk.AutoCAD.DatabaseServices.LineWeight), oDataReader.GetInt32(3)) Then
                     miLineWeight = CType(oDataReader.GetInt32(3), Autodesk.AutoCAD.DatabaseServices.LineWeight)
                  End If
               End If
               mbOn = oDataReader.GetBoolean(4)

            Else
               System.Windows.Forms.MessageBox.Show("Layer " & "(AppID=" & CStr(miAppID) & ") AND (" & sWhereFldName & "=" & sCondition & ")" & " was not found", "AcadLayerDef - zzNew!!!")

               mbCorrect = False

            End If
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "AcadLayerDef - zzNew")
         Finally
            oDataReader.Close()
         End Try
      Else
         System.Windows.Forms.MessageBox.Show("Layer Error", "AcadLayerDef - New_01")
         mbCorrect = False
      End If
   End Sub
End Structure

