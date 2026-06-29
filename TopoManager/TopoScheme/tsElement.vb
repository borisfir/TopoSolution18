Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Namespace TopoScheme
	Public Enum TopoElemType
		Polygon
		Node
		Branch
	End Enum
    Public MustInherit Class tsElement

      Protected diID As Integer
      Protected diElemType As TopoElemType
      Protected dtAcObjID As ObjectId
      Protected dtEntityHandle As Handle
      Protected dsLayer As String
      Public Sub New(ByVal iID As Integer, ByVal iElemType As TopoElemType)
         diID = iID
         diElemType = iElemType
      End Sub
        Public Property ID() As Integer
            Get
                Return diID
            End Get
            Set(ByVal iValue As Integer)
                diID = iValue
            End Set
        End Property
      Public Property AcObjID() As ObjectId

         Get
            Return dtAcObjID
         End Get
         Set(ByVal tValue As ObjectId)
            dtAcObjID = tValue
            Dim oEntity As Autodesk.AutoCAD.DatabaseServices.Entity
            Try
               oEntity = DMAcadExt.AcadTransaction.GetEntity(dtAcObjID, OpenMode.ForRead)
            Catch oEx As Exception
               System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & tValue.ToString(), "tsElement-AcObjID")
               Return
            End Try


            If oEntity IsNot Nothing Then
               dtEntityHandle = oEntity.Handle
               dsLayer = oEntity.Layer
               dtAcObjID = oEntity.ObjectId
            Else
               DMAcadExt.AcadDocument.WriteMessage("#173 Entity is not exist ID=" & dtAcObjID.ToString())
            End If

         End Set
      End Property
      Public ReadOnly Property EntityHandle As Handle
         Get
            Return dtEntityHandle
         End Get
      End Property
      Public Property Layer As String
         Get
            Return dsLayer
         End Get
         Set(sValue As String)
            dsLayer = sValue
         End Set
      End Property
        Public ReadOnly Property ElemType() As TopoElemType
            Get
                Return diElemType
            End Get
        End Property
    End Class
End Namespace

