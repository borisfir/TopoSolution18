Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Public Enum enAcadBlocks
   Block = 1
   Parcel = 2
   Plan = 3
   Lot = 4

End Enum
Public Enum enAcadAttributes
   NameStr = 1
   NameNum = 2
   LegalArea = 3
   AcadArea = 4
   CalcArea = 5

   ParentNameStr = 11
   ParentNameNum = 12
   BlockStatus = 101
   LanduseID = 401
   PlanInOut = 402
End Enum
Public Class AcadBlockDef

   Private miBlockID As enAcadBlocks
   Private msBlockName As String
   Private msLayer As String
   Private mdicAttributes As BlockAttributes
   Private Shared miFormat As Integer = 1
   Private Class BlockAttributes
      Inherits Dictionary(Of enAcadAttributes, AcadAttribute)
      Private mdicTags As Dictionary(Of String, enAcadAttributes)
      Public Sub New()
         mdicTags = New Dictionary(Of String, enAcadAttributes)
      End Sub

      Public Sub Terminate()
         If mdicTags IsNot Nothing Then
            mdicTags.Clear()
            mdicTags = Nothing
         End If
      End Sub
      Public Sub AddAttributeDef(ByVal iID As Integer, ByVal sTag As String, ByVal sLayer As String)
         Dim oAttribute As AcadAttribute = New AcadAttribute()
         If [Enum].IsDefined(GetType(enAcadAttributes), iID) Then
            oAttribute.AttributeID = CType(iID, enAcadAttributes)
         End If
         oAttribute.AttributeTag = sTag
         oAttribute.AttributeLayer = sLayer
         MyBase.Add(oAttribute.AttributeID, oAttribute)
         mdicTags.Add(sTag, oAttribute.AttributeID)
      End Sub
      Public Sub SetIndex(ByVal sTag As String, ByVal iIndex As Integer)
         Dim iAttributeID As enAcadAttributes
         Dim oAttribute As AcadAttribute

         If mdicTags.ContainsKey(sTag) Then
            iAttributeID = mdicTags.Item(sTag)
            oAttribute = MyBase.Item(iAttributeID)
            oAttribute.AttributeIndex = iIndex
            MyBase.Remove(iAttributeID)
            MyBase.Add(oAttribute.AttributeID, oAttribute)

         Else
            System.Windows.Forms.MessageBox.Show("Tag " & sTag & " was not found", "BlockAttributes-SetIndex")
         End If
      End Sub
      Public Function GetIndex(ByVal enAcadAttributes As enAcadAttributes) As Integer
         Dim oAttribute As AcadAttribute

         If MyBase.ContainsKey(enAcadAttributes) Then
            oAttribute = MyBase.Item(enAcadAttributes)
            Return oAttribute.AttributeIndex
         End If
      End Function

      Protected Overrides Sub Finalize()
         MyBase.Finalize()
      End Sub
   End Class
   Private Structure AcadAttribute
      Dim AttributeID As enAcadAttributes
      Dim AttributeTag As String
      Dim AttributeIndex As Integer
      Dim AttributeLayer As String
   End Structure
   Public Sub New(ByVal iBlockID As enAcadBlocks)
      Dim sBlockID As String = CStr(CInt(iBlockID))
      miBlockID = iBlockID
      Dim sComText As String = "SELECT BlockName,Layer FROM AcadBlocks WHERE (FormatID=" & CStr(miFormat) & ") AND (ID=" & sBlockID & ")"
      Dim oDataReader As IDataReader = TPlServerDB.ServerDB.GetDataIReader(sComText)
      Dim iAttributeID As Integer
      Dim sAttributeTag As String
      Dim sAttributeLayer As String

      If oDataReader IsNot Nothing Then
         If oDataReader.Read Then
            If Not oDataReader.IsDBNull(0) Then
               msBlockName = oDataReader.GetString(0)
            End If
            If Not oDataReader.IsDBNull(1) Then
               msLayer = oDataReader.GetString(1)
            End If

         End If
         oDataReader.Close()
         mdicAttributes = New BlockAttributes
         sComText = "SELECT AttributeID,AttributeTag,Layer FROM AcadBlockAttributes WHERE (FormatID=" & CStr(miFormat) & ") AND (BlockID=" & sBlockID & ")"

         oDataReader = TPlServerDB.ServerDB.GetDataIReader(sComText)
         If oDataReader IsNot Nothing Then
            Do While oDataReader.Read
               If oDataReader.IsDBNull(0) Then
                  iAttributeID = 0
               Else
                  iAttributeID = oDataReader.GetInt32(0)
               End If

               If oDataReader.IsDBNull(1) Then
                  sAttributeTag = String.Empty
               Else
                  sAttributeTag = oDataReader.GetString(1)
               End If

               If oDataReader.IsDBNull(2) Then
                  sAttributeLayer = String.Empty
               Else
                  sAttributeLayer = oDataReader.GetString(2)
               End If
               mdicAttributes.AddAttributeDef(iAttributeID, sAttributeTag, sAttributeLayer)
            Loop
            oDataReader.Close()
         End If
      Else
         System.Windows.Forms.MessageBox.Show("!!!!!NOTHING", "20_850")
      End If

   End Sub
   Public Shared Property Format() As Integer
      Get
         Return miFormat
      End Get
      Set(ByVal iValue As Integer)
         miFormat = iValue
      End Set
   End Property
   Public Sub Terminate()
      If mdicAttributes IsNot Nothing Then
         mdicAttributes.Terminate()
      End If
   End Sub
   Public Sub LoadDWG()


      If msBlockName IsNot Nothing AndAlso msBlockName.Length <> 0 Then
         Try
            Dim saBlockAttribTag() As String = AcadTransaction.GetAttribDef(msBlockName)
            If saBlockAttribTag IsNot Nothing Then
               For iAttribIndex As Integer = 0 To saBlockAttribTag.GetUpperBound(0)
                  mdicAttributes.SetIndex(saBlockAttribTag(iAttribIndex), iAttribIndex)
               Next
            Else
               System.Windows.Forms.MessageBox.Show(msBlockName & " Nothing", "20_487")

            End If

         Catch oEx As System.Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadBlockDef - LoadDWG")
         End Try
      End If

   End Sub
 

   Public Function GetBlockInfo(ByVal iAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId, ByVal iaAttributeIDs() As enAcadAttributes) As String()
      Dim sOut() As String
      Dim iUB As Integer = iaAttributeIDs.GetUpperBound(0)
      Dim iaAttribIndexes(iUB) As Integer
      For iIndex As Integer = 0 To iUB
         iaAttribIndexes(iIndex) = mdicAttributes.Item(iaAttributeIDs(iIndex)).AttributeIndex
      Next
		sOut = AcadTransaction.GetAttribText(iAcObjID, iaAttribIndexes)
      Return sOut

   End Function
   Public ReadOnly Property BlockName() As String
      Get
         Return msBlockName
      End Get
   End Property

End Class

