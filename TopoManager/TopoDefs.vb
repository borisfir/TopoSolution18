Option Explicit On
Option Strict On

Public Class TopoDefs
	Public Shared giToposUB As Integer
	Public Shared miaTopoIDs() As DMAcadExt.TopoDefID
	Public Shared moaTopoDefs() As DMAcadExt.TopoDef
	Private Shared mdicTopoDefs As System.Collections.Generic.Dictionary(Of Integer, Integer)
	Private Shared Sub zzSetDim()
		ReDim miaTopoIDs(giToposUB)
		ReDim moaTopoDefs(giToposUB)
	End Sub
	Public Shared Sub InitTaba()
		giToposUB = 14

		zzSetDim()
		miaTopoIDs(0) = TPlanGraph.TplnProject.GetBaseID(TPlanGraph.enTopoPurpose.Parcel)
		miaTopoIDs(1) = TPlanGraph.TplnProject.GetBaseID(TPlanGraph.enTopoPurpose.Approved)
		miaTopoIDs(2) = TPlanGraph.TplnProject.GetBaseID(TPlanGraph.enTopoPurpose.Proposed)

		miaTopoIDs(3) = TPlanGraph.TplnProject.GetAdditionalID(DMAcadExt.enTopoPurpose.Parcel)
		miaTopoIDs(4) = TPlanGraph.TplnProject.GetAdditionalID(DMAcadExt.enTopoPurpose.Approved)
		miaTopoIDs(5) = TPlanGraph.TplnProject.GetAdditionalID(DMAcadExt.enTopoPurpose.Proposed)


		miaTopoIDs(6) = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Parcel, DMAcadExt.enTopoPurpose.Approved, DMAcadExt.enOverlayMethod.Merge)
		miaTopoIDs(7) = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Parcel, DMAcadExt.enTopoPurpose.Proposed, DMAcadExt.enOverlayMethod.Merge)
		miaTopoIDs(8) = TPlanGraph.TplnProject.GetBaseID(TPlanGraph.enTopoPurpose.AdditionalA)
		'	MessageBox.Show(CStr(miaTopoIDs(8).ID), "01_900")
		'		moaTopoDefs(8) = New DMAcadExt.TopoDefID(TPlanGraph.enTopoPurpose.AdditionalA)
		miaTopoIDs(9) = TPlanGraph.TplnProject.GetDissolveID(DMAcadExt.enTopoPurpose.Parcel)
		miaTopoIDs(10) = TPlanGraph.TplnProject.GetDissolveID(DMAcadExt.enTopoPurpose.Approved)
		miaTopoIDs(11) = TPlanGraph.TplnProject.GetDissolveID(DMAcadExt.enTopoPurpose.Proposed)


		miaTopoIDs(12) = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Parcel, DMAcadExt.enTopoPurpose.Approved, DMAcadExt.enOverlayMethod.Union)
		miaTopoIDs(13) = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Parcel, DMAcadExt.enTopoPurpose.Proposed, DMAcadExt.enOverlayMethod.Union)
		miaTopoIDs(14) = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Parcel, DMAcadExt.enTopoPurpose.AdditionalA, DMAcadExt.enOverlayMethod.Union)
		mdicTopoDefs = New System.Collections.Generic.Dictionary(Of Integer, Integer)	  'Of DMAcadExt.TopoDefID
		For iTopoIndex As Integer = 0 To giToposUB
			'	System.Windows.Forms.MessageBox.Show(CStr(miaTopoIDs(iTopoIndex).ID), "24_866 ")
			moaTopoDefs(iTopoIndex) = New DMAcadExt.TopoDef(DMAcadExt.DMApp.AppID, miaTopoIDs(iTopoIndex))
			'	System.Windows.Forms.MessageBox.Show(CStr(moaTopoDefs(iTopoIndex).ID.ID), "24_867 ")
			Try
				mdicTopoDefs.Add(miaTopoIDs(iTopoIndex).ID, iTopoIndex)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "TopoDefs - Init")
			End Try
		Next
		Dim oTopoDef As DMAcadExt.TopoDef
		Dim tTopoDefID As DMAcadExt.TopoDefID

		For iTopoIndex As Integer = 0 To giToposUB
			oTopoDef = moaTopoDefs(iTopoIndex)
			tTopoDefID = oTopoDef.ID
			If tTopoDefID.TopoIsMerge Then
				oTopoDef.SourceTopoDef = moaTopoDefs(mdicTopoDefs.Item(tTopoDefID.SourceID.ID))
				oTopoDef.OverlayTopoDef = moaTopoDefs(mdicTopoDefs.Item(tTopoDefID.OverlayID.ID))
			End If
			If tTopoDefID.TopoIsBase Then
				'System.Windows.Forms.MessageBox.Show(CStr(tTopoDefID.ID), "24_872 NET")
				If mdicTopoDefs.ContainsKey(tTopoDefID.AdditionalID.ID) Then
					oTopoDef.AdditionalTopoDef = moaTopoDefs(mdicTopoDefs.Item(tTopoDefID.AdditionalID.ID))
				Else
					System.Windows.Forms.MessageBox.Show(CStr(tTopoDefID.ID), "24_872 NET")
				End If
			End If
		Next
	End Sub
   Public Shared Sub InitTopoMaster()

      giToposUB = 0
      zzSetDim()
      miaTopoIDs(0) = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Bamash)
      '	miaTopoIDs(1) = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Fragments)


      mdicTopoDefs = New System.Collections.Generic.Dictionary(Of Integer, Integer)
      For iTopoIndex As Integer = 0 To giToposUB
         moaTopoDefs(iTopoIndex) = New DMAcadExt.TopoDef(DMAcadExt.enApplications.TopoMaster, miaTopoIDs(iTopoIndex))
         Try
            mdicTopoDefs.Add(miaTopoIDs(iTopoIndex).ID, iTopoIndex)
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message, "TopoDefs - InitTopoMaster")
         End Try
      Next
   End Sub
	Public Shared Sub InitUnidiv()
		giToposUB = 1
		zzSetDim()
		miaTopoIDs(0) = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.Fragments)
		miaTopoIDs(1) = New DMAcadExt.TopoDefID(DMAcadExt.enTopoPurpose.UD_Parcels)


		mdicTopoDefs = New System.Collections.Generic.Dictionary(Of Integer, Integer)

		For iTopoIndex As Integer = 0 To giToposUB
			moaTopoDefs(iTopoIndex) = New DMAcadExt.TopoDef(DMAcadExt.enApplications.Unidiv, miaTopoIDs(iTopoIndex))
			Try
				mdicTopoDefs.Add(miaTopoIDs(iTopoIndex).ID, iTopoIndex)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "TopoDefs - InitUnidiv")
			End Try
		Next
	End Sub
   Public Shared Sub OpenTopoDef(ByVal iTopoIndex As Integer)
		Dim iTopoDefID As DMAcadExt.TopoDefID = miaTopoIDs(iTopoIndex)
		moaTopoDefs(iTopoIndex) = New DMAcadExt.TopoDef(DMAcadExt.DMApp.AppID, iTopoDefID)
		mdicTopoDefs.Add(iTopoDefID.ID, iTopoIndex)
   End Sub
	Public Shared Function Item(ByVal tTopoDefID As DMAcadExt.TopoDefID) As DMAcadExt.TopoDef
		If mdicTopoDefs.ContainsKey(tTopoDefID.ID) Then
			Return moaTopoDefs(mdicTopoDefs.Item(tTopoDefID.ID))
		Else
			Return Nothing
		End If
	End Function
	Public Shared Function GetTopoIndex(ByVal tTopoDefID As DMAcadExt.TopoDefID) As Integer
		If mdicTopoDefs.ContainsKey(tTopoDefID.ID) Then
			Return mdicTopoDefs.Item(tTopoDefID.ID)
		Else
			Return -1
		End If
	End Function
	Public Shared Function GetTopoName(ByVal iTopoDefID As DMAcadExt.TopoDefID) As String
		If mdicTopoDefs.ContainsKey(iTopoDefID.ID) Then
			Return moaTopoDefs(mdicTopoDefs.Item(iTopoDefID.ID)).Name
		Else
			Return Nothing
		End If
	End Function
	Public Shared Function GetTopoName(ByVal iTopoPurpose As DMAcadExt.enTopoPurpose) As String
		Dim iTopoDefID As DMAcadExt.TopoDefID = New DMAcadExt.TopoDefID(iTopoPurpose)
		If mdicTopoDefs.ContainsKey(iTopoDefID.ID) Then
			Return moaTopoDefs(mdicTopoDefs.Item(iTopoDefID.ID)).Name
		Else
			Return Nothing
		End If
	End Function
End Class
