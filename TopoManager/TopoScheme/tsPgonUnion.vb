Option Explicit On
Option Strict On
Imports Autodesk.Gis.Map.Topology
Imports Autodesk.AutoCAD.DatabaseServices
Namespace TopoScheme


   Public Class tsPgonUnion
      Inherits tsPolygon
      Private mhsPolygons As HashSet(Of Integer) = New HashSet(Of Integer)()
      Private mhsInnerBranches As HashSet(Of Integer) = New HashSet(Of Integer)()
      Private mhsNewInnerBranches As HashSet(Of Integer) = New HashSet(Of Integer)()
		Private mhsBoundaryBranches As HashSet(Of Integer) = New HashSet(Of Integer)()


		Private mcolFragmentPgons As ObjectModel.Collection(Of tsPolygon) = New ObjectModel.Collection(Of tsPolygon)()
      '  Private mhsFragments As HashSet(Of Integer) = New HashSet(Of Integer)
      Private miParcelDbID As Integer
      Private mtParcelKey As TPlanGraph.UD_ParcelKey
		Private mdForcedArea As Double

		Public Sub New(ByRef dicElements As tsElements)
			MyBase.New(0, dicElements)
		End Sub

		Public Sub AddPolygon(oPgonScheme As tsPolygon)
			Dim hsMyBoundaryBranchesCopy As HashSet(Of Integer) = New HashSet(Of Integer)(mhsBoundaryBranches)

			mhsPolygons.Add(oPgonScheme.ID)
			mcolFragmentPgons.Add(oPgonScheme)

			hsMyBoundaryBranchesCopy.IntersectWith(oPgonScheme.Branches) ''--> New inner
			mhsBranches.UnionWith(oPgonScheme.Branches) ''--> All
			mhsInnerBranches.UnionWith(hsMyBoundaryBranchesCopy) ''--> All inner  
			mhsInnerBranches.UnionWith(oPgonScheme.Isthmuses)  ''''''''''''''''''090724
			mhsBoundaryBranches.SymmetricExceptWith(oPgonScheme.BoundaryBranches) ''--> New Boundary

		End Sub
		Public Sub Reset()
         mhsNewInnerBranches.Clear()
      End Sub
      Public Sub AddPgonUnion(oPgonUnion As tsPgonUnion)

			For Each iPgonID As Integer In oPgonUnion.Polygons
				mhsPolygons.Add(iPgonID)
			Next
			For Each oPgonScheme As tsPolygon In oPgonUnion.FragmentPgons
            mcolFragmentPgons.Add(oPgonScheme)
         Next

			Dim hsBoundaryBranchesCopy As HashSet(Of Integer) = New HashSet(Of Integer)(mhsBoundaryBranches)
			hsBoundaryBranchesCopy.IntersectWith(oPgonUnion.BoundaryBranches) ''--> New inner
         mhsBranches.UnionWith(oPgonUnion.Branches) ''--> All
			mhsInnerBranches.UnionWith(hsBoundaryBranchesCopy)  ''-->  Inner + New inner
			mhsNewInnerBranches.UnionWith(hsBoundaryBranchesCopy)  ''-->    + new Only After Reset
         mhsInnerBranches.UnionWith(oPgonUnion.InnerBranches) ''-->   + operand inner=All inner

         mhsBoundaryBranches.SymmetricExceptWith(oPgonUnion.BoundaryBranches) ''--> New Boundary



      End Sub
      Public Sub UnionWith(oPgonScheme As tsPgonUnion)
         Dim hsAddIntersectBranches As HashSet(Of Integer) = New HashSet(Of Integer)(mhsBranches)
         hsAddIntersectBranches.IntersectWith(oPgonScheme.Branches)
         mhsBoundaryBranches.UnionWith(hsAddIntersectBranches)
         mhsBranches.SymmetricExceptWith(oPgonScheme.Branches)
      End Sub
      Public ReadOnly Property InnerBranches As HashSet(Of Integer)
         Get
            Return mhsInnerBranches
         End Get
      End Property
      Public ReadOnly Property NewInnerBranches As HashSet(Of Integer)
         Get
            Return mhsNewInnerBranches
         End Get
      End Property
      Public ReadOnly Property FragmentPgons As ObjectModel.Collection(Of tsPolygon)
         Get
            Return mcolFragmentPgons
         End Get
      End Property
      Public ReadOnly Property FragmentCount As Integer
         Get
            If mcolFragmentPgons IsNot Nothing Then
               Return mcolFragmentPgons.Count
            Else
               Return 0
            End If

         End Get
      End Property
      Public ReadOnly Property Polygons As HashSet(Of Integer)
         Get
            Return mhsPolygons
         End Get
      End Property
		Public Overloads ReadOnly Property BoundaryBranches As HashSet(Of Integer)
			Get
				Return mhsBoundaryBranches
			End Get
		End Property
		Public ReadOnly Property PolygonList As String
         Get
            Dim sRes As String = Nothing
            For Each iID As Integer In mhsPolygons
               If sRes Is Nothing Then
                  sRes = iID.ToString()
               Else
                  sRes &= "," & iID.ToString()
               End If
            Next
            Return sRes
         End Get
      End Property
      Public Property ParcelDbID As Integer
         Get
            Return miParcelDbID
         End Get
         Set(iValue As Integer)
            miParcelDbID = iValue
         End Set
      End Property
      Public Property ParcelKey As TPlanGraph.UD_ParcelKey
         Get
            Return mtParcelKey
         End Get
         Set(iValue As TPlanGraph.UD_ParcelKey)
            mtParcelKey = iValue
         End Set
      End Property

		Public Property ForcedArea As Double
			Get
				Return mdForcedArea
			End Get
			Set(dValue As Double)
				mdForcedArea = dValue
			End Set
		End Property

		Public Sub DebugExcel(Optional sCaption As String = "")
			DMCommon.Debug.ExcelLog.SetValue(0, "", mhsPolygons.Count)
			DMCommon.Debug.ExcelLog.SetEnumerable(1, sCaption & "Polygons", mhsPolygons)

			DMCommon.Debug.ExcelLog.SetValue(0, "", mhsInnerBranches.Count)
			DMCommon.Debug.ExcelLog.SetEnumerable(1, sCaption & "InnerBranches", mhsInnerBranches)

			DMCommon.Debug.ExcelLog.SetValue(0, "", mhsNewInnerBranches.Count)
			DMCommon.Debug.ExcelLog.SetEnumerable(1, sCaption & "NewInnerBranches", mhsNewInnerBranches)

			DMCommon.Debug.ExcelLog.SetValue(0, "", mhsBoundaryBranches.Count)
			DMCommon.Debug.ExcelLog.SetEnumerable(1, sCaption & "BoundaryBranches", mhsBoundaryBranches)

			Dim iaFragmnts(mcolFragmentPgons.Count - 1) As Integer
			For iIndex As Integer = 0 To mcolFragmentPgons.Count - 1
				iaFragmnts(iIndex) = mcolFragmentPgons.Item(iIndex).ID
			Next

			DMCommon.Debug.ExcelLog.SetEnumerable(1, sCaption & "FragmentPgons", iaFragmnts)

		End Sub
	End Class
End Namespace