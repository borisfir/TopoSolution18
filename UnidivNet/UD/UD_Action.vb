Option Explicit On
Option Strict On
Imports System.Data
Imports TopoManager.TPlanGraph

Public Class UD_Action
   Private moaParcels() As UnidivNet.UD_Parcel
   Private moParcel As UnidivNet.UD_Parcel
   Private miActionType As enActionType
   Public Sub New(iActionType As enActionType)
      miActionType = iActionType
   End Sub
End Class
