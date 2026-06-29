
Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices
Namespace TPlanGraph


	Public Class TplnRegionSet
		Private Class RegionSetDictionary
			Inherits DBDictionary

			Public Sub New()
				MyBase.New()
			End Sub
			Public Sub New1(o As DBDictionary)
				'MyBase = o
			End Sub

		End Class
		Const msDictionaryName As String = "RegionSet"
		Const msNamePrefix As String = "*"
		Private miNo As Integer
		Private msName As String
		Private msRegionList As String
		Private miIndex As Integer
		'Private Shared mdicMain As DBDictionary
		'Private Shared mdicMain As RegionSetDictionary = New RegionSetDictionary()
		Private Shared mdicMain As DBDictionary = New DBDictionary()

		Private Shared mbOpened As Boolean = False
		Private Shared miMode As OpenMode
		Private Shared miLastNo As Integer
		Private Shared mbHasPrefix As Boolean
		Public Sub New(iNo As Integer, sName As String, sRegionList As String)
			miNo = iNo
			msName = sName
			msRegionList = sRegionList
		End Sub

		Public Sub New(oaTypedValue() As TypedValue, iIndex As Integer)
			If oaTypedValue.GetUpperBound(0) = 2 Then
				miNo = DirectCast(oaTypedValue(0).Value, Integer)
				msName = DirectCast(oaTypedValue(1).Value, String)
				msRegionList = DirectCast(oaTypedValue(2).Value, String)
			End If
			miIndex = iIndex
		End Sub
		Public Sub New(oDBDictionary As DBDictionary)
			oDBDictionary.GetEnumerator()
		End Sub
		Public Shared Sub Load()
			zzOpenDictionary(True, False)
		End Sub

		Public Shared Property HasPrefix As Boolean
			Get
				Return mbHasPrefix
			End Get
			Set(bValue As Boolean)
				mbHasPrefix = bValue
			End Set
		End Property
		Public Overrides Function ToString() As String
			Dim sPrefix As String = String.Empty

			If mbHasPrefix Then
				sPrefix = msNamePrefix
			End If
			Return sPrefix & msName
		End Function
		Public Shared Sub zzAdd(oRegionSet As TplnRegionSet)
			Dim oXrecord As Xrecord = New Xrecord()
			Dim sKey As String = oRegionSet.No.ToString()
			Dim oaValue() As System.Object = {oRegionSet.No, oRegionSet.Name, oRegionSet.RegionList}

			Try
				oXrecord.Data = DMAcadExt.AcadUtil.GetResBuffer(oaValue, False)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & mdicMain.Count.ToString() & vbCrLf & sKey, "Error #2915")

			End Try
			'DMCommon.Debug.ExcelLog.SetNextValue(0, "!+RegS03", sKey, oXrecord.Count, mdicMain Is Nothing, "--- -------")


			Try
				mdicMain.SetAt(sKey, oXrecord)
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & mdicMain.Count.ToString() & vbCrLf & sKey, "Error #2916")
			End Try

			DMAcadExt.AcadTransaction.AppendDBObject(oXrecord)

		End Sub
		Public Shared Sub RefreshDBDictionary()
			Try
				mdicMain = DirectCast(DMAcadExt.AcadTransaction.GetDBObject(mdicMain.ObjectId, OpenMode.ForWrite), Autodesk.AutoCAD.DatabaseServices.DBDictionary)

			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & mdicMain.Count.ToString(), "Error #2908")
			End Try
		End Sub

		Public Shared Function Add(sName As String, sRegionList As String) As TplnRegionSet
			Dim iNo As Integer = miLastNo + 1
			Dim sKey As String = iNo.ToString()
			Dim oRegionSet As TplnRegionSet = New TplnRegionSet(iNo, sName, sRegionList)

			zzAdd(oRegionSet)
			miLastNo = iNo
			Return oRegionSet
		End Function
		Public Shared Sub Delete(iNo As Integer)
			Dim sKey As String = iNo.ToString()
			'	Dim oRegionSet As TplnRegionSet = New TplnRegionSet(iNo, sName, sRegionList)
			mdicMain.Remove(sKey)

		End Sub

		Public Shared Function Replace(iNo As Integer, sName As String, sRegionList As String) As TplnRegionSet
			Dim sKey As String = iNo.ToString()
			Dim oRegionSet As TplnRegionSet = New TplnRegionSet(iNo, sName, sRegionList)
			DMCommon.Debug.MsgBox("!Replace", oRegionSet.No, oRegionSet.Name, oRegionSet.RegionList, oRegionSet.ToString())
			mdicMain.Remove(sKey)
			zzAdd(oRegionSet)

			Return oRegionSet
		End Function


		Public Shared Function GetArray() As TplnRegionSet()
			If mdicMain.Count > 0 Then
				Dim oaRes(mdicMain.Count - 1) As TplnRegionSet
				Dim iXRecIndex As Integer = 0
				Dim oXRecord As Xrecord
				Dim oResBuffer As ResultBuffer
				Dim oaTypedValue() As TypedValue
				Dim tAcObjID As ObjectId
				Dim oDbDictionaryEnumerator As DbDictionaryEnumerator = mdicMain.GetEnumerator()
				'	oDbDictionaryEnumerator.Reset()

				Do While oDbDictionaryEnumerator.MoveNext()

					''For Each tAcObjID As ObjectId In mdicMain.Values
					'DMCommon.Debug.ExcelLog.SetNextValue(0, "!ItemType", oObject, oObject.GetType())				DMCommon.Debug.ExcelLog.SetNextValue(0, "!ItemCount", mdicMain.Count)
					tAcObjID = oDbDictionaryEnumerator.Value
					oXRecord = DMAcadExt.AcadTransaction.GetXrecord(tAcObjID)

					If oXRecord IsNot Nothing Then
						oResBuffer = oXRecord.Data
						oaTypedValue = oResBuffer.AsArray()

						'For iIndex As Integer = 0 To oaTypedValue.GetUpperBound(0)
						'DMCommon.Debug.ExcelLog.SetNextValue(0, "!TypedValue", iXRecIndex, iIndex, oaTypedValue(iIndex).Value, oaTypedValue(iIndex).TypeCode)
						'Next
						oaRes(iXRecIndex) = New TplnRegionSet(oaTypedValue, iXRecIndex)
						'Next
					End If
					iXRecIndex += 1
				Loop
				'Next
				Return oaRes
			Else
				Return Nothing
			End If

		End Function
		Public ReadOnly Property No As Integer
			Get
				Return miNo
			End Get
		End Property
		Public ReadOnly Property Name As String
			Get
				Return msName
			End Get
		End Property
		Public ReadOnly Property RegionList As String
			Get
				Return msRegionList
			End Get
		End Property
		Public ReadOnly Property RegionSet As HashSet(Of Integer)
			Get
				Dim saSeparator() As String = {","}
				Dim saRegions() As String = msRegionList.Split(saSeparator, StringSplitOptions.RemoveEmptyEntries)
				Dim hsRes As HashSet(Of Integer) = New HashSet(Of Integer)()
				Dim iRegionNo As Integer
				For iIndex As Integer = 0 To saRegions.GetUpperBound(0)
					If Integer.TryParse(saRegions(iIndex), iRegionNo) Then
						hsRes.Add(iRegionNo)
					End If
				Next
				Return hsRes
			End Get

		End Property
		Public Property Index As Integer
			Get
				Return miIndex
			End Get
			Set(iValue As Integer)
				miIndex = iValue
			End Set
		End Property

		Private Shared Sub zzOpenDictionary(ByVal bCreate As Boolean, ByVal bReadOnly As Boolean)
			Dim tRegionSetDicObjID As ObjectId
			Dim dicNamed As Autodesk.AutoCAD.DatabaseServices.DBDictionary = Nothing
			Dim iNamedMode As OpenMode
			Dim iMode As OpenMode
			If bCreate Then
				iNamedMode = OpenMode.ForWrite
			Else
				iNamedMode = OpenMode.ForRead
			End If
			If bReadOnly Then '
				iMode = OpenMode.ForRead
			Else
				iMode = OpenMode.ForWrite
			End If
			'	DMCommon.Debug.MsgBox("081120_0", "4", dicNamed Is Nothing, bCreate, bReadOnly, iNamedMode, iMode)
			dicNamed = DMAcadExt.AcadTransaction.GetNamedDictionary(iNamedMode)
			'	DMCommon.Debug.MsgBox("081120_1a", "41", msDictionaryName, dicNamed Is Nothing)
			If dicNamed IsNot Nothing Then
				'	DMCommon.Debug.MsgBox("070720_1", dsDictionaryName, dicNamed.Contains(dsDictionaryName))
				If dicNamed.Contains(msDictionaryName) Then
					'DMCommon.Debug.MsgBox("081120_1", "52a")
					Try
						tRegionSetDicObjID = DirectCast(dicNamed.Item(msDictionaryName), ObjectId)
					Catch oEx As Exception
						DMCommon.Debug.MsgBox("180421_1", oEx.Message, msDictionaryName, dicNamed.Contains(msDictionaryName), dicNamed.Item(msDictionaryName), dicNamed.Item(msDictionaryName).GetType())
					End Try

					mdicMain = DirectCast(DMAcadExt.AcadTransaction.GetDBObject(tRegionSetDicObjID, iMode), Autodesk.AutoCAD.DatabaseServices.DBDictionary)

					'mdicMain = DirectCast(DMAcadExt.AcadTransaction.GetDBObject(tRegionSetDicObjID, iMode), RegionSetDictionary)
					mbOpened = True
					miMode = iMode
				ElseIf bCreate Then
					'	DMCommon.Debug.MsgBox("081120_1", "52b")
					mdicMain = New Autodesk.AutoCAD.DatabaseServices.DBDictionary()
					'mdicMain = New RegionSetDictionary()
					dicNamed.SetAt(msDictionaryName, mdicMain)
					mbOpened = True
					DMAcadExt.AcadTransaction.AppendDBObject(mdicMain)
					dicNamed.Contains(msDictionaryName)

				End If
				dicNamed = Nothing
			Else
				MessageBox.Show("NamedDictionary is nothing " & iNamedMode.ToString())
			End If

		End Sub
		Private Sub zzOpen()
			'For Each o As Object In mdicMain.
		End Sub
	End Class
End Namespace