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
	PaintHatch = 11
End Enum
Public Class AcadLayerDef
   Private msName As String
   Private miLayerFunction As enLayerFunction
   Private moColor As Autodesk.AutoCAD.Colors.Color
	Private miLineWeight As Autodesk.AutoCAD.DatabaseServices.LineWeight
	Private mbCorrect As Boolean = True
   Public Sub New(ByVal sLayer As String)
      msName = sLayer
      zzNew(True)
   End Sub
   Public Sub New(ByVal iLayerFunction As enLayerFunction)
      miLayerFunction = iLayerFunction
      zzNew(False)
   End Sub
   Public ReadOnly Property Name() As String
      Get
         Return msName
      End Get
   End Property
   Public ReadOnly Property Color() As Autodesk.AutoCAD.Colors.Color
      Get
         Return moColor
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
   Private Sub zzNew(ByVal bByName As Boolean)
      Const sNameFldName As String = "LayerName"
      Const sFunctionFldName As String = "Function"

      Dim sSelectFldName As String
      Dim sWhereFldName As String
      Dim sCondition As String
      If bByName Then
         sSelectFldName = sFunctionFldName
         sWhereFldName = sNameFldName
         sCondition = "'" & msName & "'"

      Else 'ByFunction
         sSelectFldName = sNameFldName
         sWhereFldName = sFunctionFldName
         sCondition = CStr(CInt(miLayerFunction))
      End If
		Dim sComText As String = "SELECT " & sSelectFldName & ",Argb,AcadColor,LineWeight FROM LayerDefs WHERE (AppID=" & CStr(Common.AppID) & ") AND (" & sWhereFldName & "=" & sCondition & ")"

      Dim oDataReader As IDataReader = TPlServerDB.ServerDB.GetDataIReader(sComText)

      If oDataReader IsNot Nothing Then
         If oDataReader.Read Then
            If bByName Then
               If [Enum].IsDefined(GetType(TopoManager.enLayerFunction), oDataReader.GetInt32(0)) Then
                  miLayerFunction = CType(oDataReader.GetInt32(0), enLayerFunction)
               End If
            Else
               msName = oDataReader.GetString(0)
            End If

            If Not oDataReader.IsDBNull(1) Then
               moColor = Autodesk.AutoCAD.Colors.Color.FromColor(System.Drawing.Color.FromArgb(oDataReader.GetInt32(1)))
            ElseIf Not oDataReader.IsDBNull(2) Then
               moColor = Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.None, oDataReader.GetInt16(2))
            End If
            If Not oDataReader.IsDBNull(3) Then
               If [Enum].IsDefined(GetType(Autodesk.AutoCAD.DatabaseServices.LineWeight), oDataReader.GetInt32(3)) Then
                  miLineWeight = CType(oDataReader.GetInt32(3), Autodesk.AutoCAD.DatabaseServices.LineWeight)
               End If
            End If
            oDataReader.Close()
         Else
				System.Windows.Forms.MessageBox.Show("Layer " & sCondition & " was not found", "AcadLayerDef - New")
				mbCorrect = False
         End If
      Else
			System.Windows.Forms.MessageBox.Show("Layer Error", "AcadLayerDef - New_01")
			mbCorrect = False
      End If
   End Sub
End Class
