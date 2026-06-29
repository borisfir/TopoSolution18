Option Explicit On
Option Strict On
Imports TopoManager.TPlanGraph
Public Enum enActionType
   [Default] = -1
   Registered = 0
   Divide = 1
   Union = 2
   Transfer = 3
	Update = 4
	RestoreLayer = 5

End Enum



Public Class UD_OperDetail
	Private miOperation As Integer
	Private miActionType As enActionType
	Private miFragment As Integer
	Private mtOldParcel As UD_ParcelKey
	Private mtNewParcel As UD_ParcelKey
	Private mbIsOrigin As Boolean
   Private miStageNo As Integer
   Public Sub New(ByVal oDataReader As System.Data.Common.DbDataReader)
      If oDataReader IsNot Nothing Then
         Dim iBlockNo As Integer
         Dim iParcelNo As Integer
         Dim bOrigin As Boolean
         Dim iActionType As Integer

         miOperation = Convert.ToInt32(oDataReader.GetInt16(0))
         iActionType = Convert.ToInt32(oDataReader.GetInt16(1))

         If [Enum].IsDefined(GetType(enActionType), iActionType) Then
            miActionType = CType(iActionType, enActionType)
         End If
         miFragment = Convert.ToInt32(oDataReader.GetDouble(2))

         iBlockNo = Convert.ToInt32(oDataReader.GetInt16(3))
         iParcelNo = Convert.ToInt32(oDataReader.GetInt16(4))
         bOrigin = oDataReader.GetBoolean(5)
         If iBlockNo = 1 Then
            mtOldParcel = New UD_ParcelKey(iParcelNo, bOrigin)
         Else
            If iBlockNo < 0 Then
               iBlockNo += UShort.MaxValue + 1
            End If
            mtOldParcel = New UD_ParcelKey(iBlockNo, iParcelNo, bOrigin)
         End If

         mbIsOrigin = oDataReader.GetBoolean(5)

         iBlockNo = Convert.ToInt32(oDataReader.GetInt16(6))
         iParcelNo = Convert.ToInt32(oDataReader.GetInt16(7))
         bOrigin = oDataReader.GetBoolean(8)
         If iBlockNo = 1 Then
            mtNewParcel = New UD_ParcelKey(iParcelNo, bOrigin)
         Else
            If iBlockNo < 0 Then
               iBlockNo += UShort.MaxValue + 1
            End If
            mtNewParcel = New UD_ParcelKey(iBlockNo, iParcelNo, bOrigin)
         End If

         'mbOrigin = oDataReader.GetBoolean(8)
      End If
   End Sub
   Public ReadOnly Property OldParcel As UD_ParcelKey
      Get
         Return mtOldParcel
      End Get
   End Property
   Public ReadOnly Property NewParcel As UD_ParcelKey
      Get
         Return mtNewParcel
      End Get
   End Property
   Public ReadOnly Property ActionType As enActionType
      Get
         Return miActionType
      End Get
   End Property
   Public ReadOnly Property Fragment As Integer
      Get
         Return miFragment
      End Get
   End Property
   Public ReadOnly Property Operation As Integer
      Get
         Return miOperation
      End Get
   End Property
   Public ReadOnly Property IsOrigin As Boolean
      Get
         Return mbIsOrigin
      End Get
   End Property
   Public Property StageNo As Integer
      Get
         Return miStageNo
      End Get
      Set(iValue As Integer)
         miStageNo = iValue
      End Set
   End Property
End Class
