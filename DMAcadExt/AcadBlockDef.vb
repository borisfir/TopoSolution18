Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Imports System.Data
Public Enum enAcadBlocks
   Block = 1
   Parcel = 2
   Plan = 3
   Lot = 4
   DMLot = 5
   UD_Parcel = 21
   UD_Parcel_Hanit = 22

End Enum
Public Enum enAcadAttributes
   NameStr = 1
   NameNum = 2
   LegalArea = 3
   AcadArea = 4
   CalcArea = 5
   ForcedArea = 6

   ParentNameStr = 11
   ParentNameNum = 12
	BlockStatus = 101
	BlockSupplement = 102
   BlockAnalytic = 103
   Source = 111
   Cross = 201
   Comment = 202
   ParcelPrev = 203
   GushPrev = 204

   LanduseID = 400
   LanduseName = 401
   PlanInOut = 402
   Plan = 403
   Lot = 404
   NewParcelNo = 405

	Filler1 = 1001
	Filler2 = 1002
	Filler3 = 1003
	Filler4 = 1004
	Filler5 = 1005
	Filler6 = 1006
	Filler7 = 1007
	Filler8 = 1008
	Filler9 = 1009
  

End Enum


Public Class AcadBlockDef

   Private miBlockID As enAcadBlocks
   Private msBlockName As String
   Private msLayer As String
   Private mdicAttributes As BlockAttributes
	Private Shared miFormat As Integer = 1
	Private miaAttributesID() As DMAcadExt.enAcadAttributes
	Private miaAttributeIndices() As Integer

	Public Delegate Sub ParseAttribute(ByVal sAttributeValue As String)
   Private Class BlockAttributes
		Inherits Dictionary(Of enAcadAttributes, AcadAttribute)
      Private mdicTags As Dictionary(Of String, enAcadAttributes)
      Public Sub New()
         mdicTags = New Dictionary(Of String, enAcadAttributes)
      End Sub

      Public Sub Terminate()
         If mdicTags IsNot Nothing Then
				'mdicTags.Clear()
            mdicTags = Nothing
         End If
      End Sub
      Public Sub AddAttributeDef(ByVal iID As Integer, ByVal sTag As String, ByVal sLayer As String)
         Dim oAttribute As AcadAttribute = New AcadAttribute()
         If [Enum].IsDefined(GetType(enAcadAttributes), iID) Then
            oAttribute.AttributeID = CType(iID, enAcadAttributes)
         Else
            System.Windows.Forms.MessageBox.Show(iID.ToString & ":" & sTag, "09_671")
         End If
         oAttribute.AttributeTag = sTag
			oAttribute.AttributeLayer = sLayer
			Try
				MyBase.Add(oAttribute.AttributeID, oAttribute)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oAttribute.AttributeID.ToString() & vbCrLf & sTag, "AcadBlockDef - AddAttributeDef_1")
         End Try
         '  System.Windows.Forms.MessageBox.Show(sTag & ":" & oAttribute.AttributeID.ToString(), "09_788")
			Try
				mdicTags.Add(sTag, oAttribute.AttributeID)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & sTag & vbCrLf & oAttribute.AttributeID.ToString(), "AcadBlockDef - AddAttributeDef_2")
			End Try
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
            System.Windows.Forms.MessageBox.Show("Tag " & sTag & " was not found" & vbCrLf & "Name=" & "msBlockName" & vbCrLf & "mdicTags.Count = " & mdicTags.Count.ToString(), "BlockAttributes - SetIndex")
         End If
		End Sub

		Public Function GetIndex(ByVal iAcadAttributeID As enAcadAttributes) As Integer
			Dim oAttribute As AcadAttribute

			If MyBase.ContainsKey(iAcadAttributeID) Then
				oAttribute = MyBase.Item(iAcadAttributeID)
				Return oAttribute.AttributeIndex
			End If
		End Function

		Public Function GetIndices(ByVal iaAcadAttributeIDs() As enAcadAttributes) As Integer()
			Dim oAttribute As AcadAttribute
			Dim iUB As Integer = iaAcadAttributeIDs.GetUpperBound(0)
			Dim iaRes(iUB) As Integer
			For iIndex As Integer = 0 To iUB
				If MyBase.ContainsKey(iaAcadAttributeIDs(iIndex)) Then
					oAttribute = MyBase.Item(iaAcadAttributeIDs(iIndex))
					iaRes(iIndex) = oAttribute.AttributeIndex
            Else
               DMCommon.Debug.MsgBox("12_308", iIndex, iaAcadAttributeIDs(iIndex))
               iaRes(iIndex) = -1
				End If
			Next
			Return iaRes
		End Function
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
		Dim oDataReader As Common.DbDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
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

			oDataReader = TPlServerDB.ServerDB.CurrentServerDB.GetDataReader(sComText)
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
               '  DMCommon.ExcelLogG.SetNextValue(i, 0, iAttributeID, sAttributeTag, sAttributeLayer, mdicAttributes.Count)


               mdicAttributes.AddAttributeDef(iAttributeID, sAttributeTag, sAttributeLayer)
            Loop
				oDataReader.Close()
				If mdicAttributes.Count = 0 Then
               System.Windows.Forms.MessageBox.Show("Err #2105" & vbCrLf & "AcadBlockDef - New")
            Else
               ' System.Windows.Forms.MessageBox.Show("OK #2109" & vbCrLf & "AcadBlockDef - New")
            End If
			Else
				System.Windows.Forms.MessageBox.Show("Err #2106" & vbCrLf & "AcadBlockDef - New")
			End If
		Else
			System.Windows.Forms.MessageBox.Show(sComText & vbCrLf & "!!!!!NOTHING", "20_850")
		End If

	End Sub
	Public Property AttributesID() As DMAcadExt.enAcadAttributes()
		Get
			Return miaAttributesID
		End Get
		Set(ByVal iaValue As DMAcadExt.enAcadAttributes())
			miaAttributesID = iaValue

        
      End Set
	End Property
	Public ReadOnly Property AttributeIndices() As Integer()
		Get
			Return miaAttributeIndices
		End Get

	End Property
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
            Dim saBlockAttribTag() As String = AcadTransaction.GetAttribDef(msBlockName, True)
            'DMCommon.Debug.MsgBox("09_551_" & msBlockName, DMCommon.Debug.GetListArray(saBlockAttribTag))
            '   DMCommon.Functions.DispArray(saBlockAttribTag, "01_579g", True)

            'DMCommon.ExcelLogG
            If saBlockAttribTag IsNot Nothing Then
               For iAttribIndex As Integer = 0 To saBlockAttribTag.GetUpperBound(0)
                  mdicAttributes.SetIndex(saBlockAttribTag(iAttribIndex), iAttribIndex)
               Next
               ' DMCommon.Functions.DispArray(miaAttributesID, "01_565T")
					miaAttributeIndices = mdicAttributes.GetIndices(miaAttributesID)
				End If
				'	Erase saBlockAttribTag
         Catch oEx As System.Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "AcadBlockDef - LoadDWG")
         End Try
      End If

   End Sub
 

	Public Function GetBlockInfo(ByVal iAcObjID As Autodesk.AutoCAD.DatabaseServices.ObjectId, ByVal iaAttributeIDs() As enAcadAttributes, ByRef bAcadPoint As Boolean) As String()
		Dim sOut() As String
		Dim iUB As Integer = iaAttributeIDs.GetUpperBound(0)
		Dim iaAttribIndexes(iUB) As Integer
		For iIndex As Integer = 0 To iUB
			If Not mdicAttributes.ContainsKey(iaAttributeIDs(iIndex)) Then
            System.Windows.Forms.MessageBox.Show(msBlockName & vbCrLf & CStr(iaAttributeIDs(iIndex)) & ":" & CStr(iIndex), "AcadBlockDef - GetBlockInfo")

			End If
			iaAttribIndexes(iIndex) = mdicAttributes.Item(iaAttributeIDs(iIndex)).AttributeIndex
		Next
		sOut = AcadTransaction.GetAttribText(iAcObjID, True, bAcadPoint, iaAttribIndexes)
		Return sOut

	End Function
   Public ReadOnly Property BlockName() As String
      Get
         Return msBlockName
      End Get
   End Property

End Class

