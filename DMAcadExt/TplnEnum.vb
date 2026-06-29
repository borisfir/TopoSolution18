Public Enum enTopoPurpose
	Undefined = -1
	Parcel = 1
	Approved = 2
	Proposed = 3
	Block = 4
	AdditionalA = 9
	Bamash = 11
	Fragments = 12
	UD_Parcels = 13
	OwnershipNote = 14
	Expro = 15
	Mitham = 16
End Enum

Public Enum enOverlayMethod
	Undefined = -1
	Merge = 0
	Union = 1
	FDO_Overlay = 2
	Dissolve = 3
End Enum
Public Enum enOverlayIndex
	Undefined = -1
	ApprMerge = 0
	PropMerge = 1
	ApprUnion = 2
	PropUnion = 3
	ApprFDO_Overlay = 4
	PropFDO_Overlay = 5
	OverlayIndexUB = PropFDO_Overlay
End Enum

Public Enum enMapTheme
	Undefined = 0
   Zone = 1
	Blocks = 2
	Parcels = 3
	LotApproved = 4
	LotProposed = 5
	Expropriation = 6
	Ownership = 7
	ParcelAnalytic = 8
   Merhav = 9
   UD_Parcels = 10
   UD_Blocks = 11
   LanduseApproved = 12
   PlanApproved = 14
   PlanProposed = 15
   Mitham = 16
   MithamProx = 17
   Fragments = 18
   BN = 19
	Bamash = 20
	ParcelsXApproved = 21
	ParcelsXProposed = 22
	ParcelsXMitham = 23
	AnalyticInParcels = 24
	ParcelsXExpro = 25
	ParcelSettlement = 26
	ExproXLotApproved = 27
	ParcelsXExproXLots = 31
	MerhavOverlay = 32
	ExproZoneOverlay = 33
	OwnershipNotes = 51
	ParcelsXOwnershipNotes = 52
	EntConnectedLot = 53

End Enum
Public Class TplnEnum
	Public Shared Function ToTopoPurpose(iValue As Integer) As enTopoPurpose
		If [Enum].IsDefined(GetType(enTopoPurpose), iValue) Then
			Return CType(iValue, enTopoPurpose)
		Else
			Return enTopoPurpose.Undefined
		End If
	End Function
End Class
