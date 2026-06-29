Option Explicit On
Option Strict On
Imports Autodesk.AutoCAD.DatabaseServices

Public MustInherit Class ProjectData
	Public Enum enControlType
		TextBox = 1
		CheckBox = 2
		ComboBox = 10
		ComboBoxColorSet = 11
		ComboBoxScale = 12
	End Enum

	Protected dsDictionaryName As String = "ProjectData"
	Private mdicMain As DBDictionary
	Private mbOpened As Boolean = False
	Private miMode As OpenMode


	Protected diaControlTypes() As enControlType
	Protected dsaValues() As String
	Protected doaDMValues() As DMCommon.DMValue
	Protected dbDirty As Boolean = False

	Protected dsAppBuild As String

	Public MustOverride Sub OpenData(ByVal bCreateValues As Boolean, ByVal bReadOnly As Boolean)
	Public MustOverride Sub Update()
	Public Sub TestDictionaryList()

		Dim dicNamed As Autodesk.AutoCAD.DatabaseServices.DBDictionary
		Dim iNamedMode As OpenMode

 

		dicNamed = DMAcadExt.AcadTransaction.GetNamedDictionary(OpenMode.ForRead)
		MessageBox.Show(CStr(dicNamed.Count) & ":" & dsDictionaryName, "01_492")
		If dicNamed IsNot Nothing Then
			Try
				For Each oEntry As DBDictionaryEntry In dicNamed

					DMAcadExt.AcadDocument.WriteMessage("#199" & oEntry.m_key & "//" & oEntry.m_value.ToString())
				Next
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message, "ProjectData - TestDictionaryList")
			End Try
			
		Else
			MessageBox.Show("NamedDictionary is nothing !!!" & iNamedMode.ToString())
		End If

	End Sub
	Protected Sub OpenDictionary(ByVal bCreate As Boolean, ByVal bReadOnly As Boolean)
		Dim tPrjDataDicObjID As ObjectId
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
		'DMCommon.Debug.MsgBox("081120_1", "4", dicNamed Is Nothing, bCreate, bReadOnly,iNamedMode)
		dicNamed = DMAcadExt.AcadTransaction.GetNamedDictionary(iNamedMode)
		'DMCommon.Debug.MsgBox("081120_1", "41", dsDictionaryName, dicNamed Is Nothing)
		If dicNamed IsNot Nothing Then
			'	DMCommon.Debug.MsgBox("070720_1", dsDictionaryName, dicNamed.Contains(dsDictionaryName))
			If dicNamed.Contains(dsDictionaryName) Then
				'DMCommon.Debug.MsgBox("081120_1", "52a")
				tPrjDataDicObjID = DirectCast(dicNamed.Item(dsDictionaryName), ObjectId)
				mdicMain = DirectCast(DMAcadExt.AcadTransaction.GetDBObject(tPrjDataDicObjID, iMode), Autodesk.AutoCAD.DatabaseServices.DBDictionary)
				mbOpened = True
				miMode = iMode
			ElseIf bCreate Then
				'	DMCommon.Debug.MsgBox("081120_1", "52b")
				mdicMain = New Autodesk.AutoCAD.DatabaseServices.DBDictionary()
				dicNamed.SetAt(dsDictionaryName, mdicMain)
				mbOpened = True
				DMAcadExt.AcadTransaction.AppendDBObject(mdicMain)
				dicNamed.Contains(dsDictionaryName)
			Else
				DMAcadExt.AcadDocument.WriteMessage(dsDictionaryName & " was not found")
			End If
			dicNamed = Nothing
		Else
			MessageBox.Show("NamedDictionary is nothing " & iNamedMode.ToString())
		End If

	End Sub
	Public Sub CloseDictionary()
		mdicMain = Nothing
		mbOpened = False
	End Sub
	Protected Sub SetControlTypes(ByVal iaControlTypes() As enControlType)
		diaControlTypes = iaControlTypes
		ReDim dsaValues(diaControlTypes.GetUpperBound(0))
		ReDim doaDMValues(diaControlTypes.GetUpperBound(0))
	End Sub

	Public ReadOnly Property Opened() As Boolean
		Get
			Return mbOpened
		End Get
	End Property
	Public ReadOnly Property Mode() As OpenMode
		Get
			Return miMode
		End Get
	End Property
	Public Property dmItem(ByVal iIndex As Integer) As DMCommon.DMValue
		Get
			Try
				Return doaDMValues(iIndex)
			Catch oEx As Exception
				Return Nothing
			End Try
		End Get
		Set(ByVal tValue As DMCommon.DMValue)
			doaDMValues(iIndex) = tValue
		End Set
	End Property

	Default Public Property Item(ByVal iIndex As Integer) As String
		Get
			Try
				Return dsaValues(iIndex)
			Catch oEx As Exception
				Return Nothing
			End Try
		End Get
		Set(ByVal oValue As String)
			dsaValues(iIndex) = oValue
		End Set
	End Property
	Default Public Property Item(ByVal sKey As String) As System.Object
      Get
         Dim tXrecObjID As ObjectId
			Try

				If mdicMain IsNot Nothing AndAlso mdicMain.Contains(sKey) Then
					tXrecObjID = DirectCast(mdicMain.Item(sKey), ObjectId)

					Dim oXrecord As Autodesk.AutoCAD.DatabaseServices.Xrecord = DMAcadExt.AcadTransaction.GetXrecord(tXrecObjID)
					If oXrecord IsNot Nothing Then
						Dim oResBuffer As ResultBuffer = oXrecord.Data
						If oResBuffer IsNot Nothing Then
							Dim oaTypedValue() As TypedValue = oResBuffer.AsArray()
							If oaTypedValue IsNot Nothing Then
								Return oaTypedValue(0).Value
							Else
								System.Windows.Forms.MessageBox.Show("oaTypedValue Is Nothing", "08_027")
								Return Nothing
							End If
						Else
							Return Nothing
						End If

					Else
						Return Nothing
					End If

				Else
					'   System.Windows.Forms.MessageBox.Show(sKey & vbCrLf & (mdicMain IsNot Nothing).ToString(), "08_012")
					Return Nothing
				End If
			Catch oEx As Exception
				System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace & vbCrLf & tXrecObjID.ToString() & vbCrLf & DMCommon.Functions.CStrN(sKey, "Key is Nothing"), "ProjectData - Item")
            Return Nothing
         End Try
      End Get
		Set(ByVal oValue As System.Object)
			If oValue Is Nothing Then
				System.Windows.Forms.MessageBox.Show(CStr(mdicMain Is Nothing), "12_799")
			Else
				If mdicMain.Contains(sKey) Then
					Dim oXrecord As Autodesk.AutoCAD.DatabaseServices.Xrecord = New Xrecord()
					'	Dim oResBuffer As ResultBuffer = New ResultBuffer()
					'	Dim oTypedValue As TypedValue = DMAcadExt.AcadConst.ToTypedValue(oValue)
					'	oResBuffer.Add(oTypedValue)
					oXrecord.Data = DMAcadExt.AcadUtil.GetResBuffer(oValue, False)
					mdicMain.Item(sKey) = oXrecord
					DMAcadExt.AcadTransaction.AppendDBObject(oXrecord)
				Else
					zzAddItem(sKey, oValue)
				End If
			End If

		End Set
   End Property
   Public Property ItemArray(ByVal sKey As String) As System.Object()
      Get
         Try
				If mdicMain IsNot Nothing AndAlso mdicMain.Contains(sKey) Then
					Dim oItem As System.Object = mdicMain.Item(sKey)
					If oItem IsNot Nothing Then
						Dim tXrecObjID As ObjectId = DirectCast(oItem, ObjectId)
						Dim oXrecord As Autodesk.AutoCAD.DatabaseServices.Xrecord = DMAcadExt.AcadTransaction.GetXrecord(tXrecObjID)
						'  System.Windows.Forms.MessageBox.Show(sKey & vbCrLf & tXrecObjID.ToString(), "08_021")
						Dim oResBuffer As ResultBuffer = oXrecord.Data
						Dim oaTypedValue() As TypedValue = oResBuffer.AsArray()
						Dim oaResValue(oaTypedValue.GetUpperBound(0)) As System.Object
						If oaTypedValue IsNot Nothing Then
							For iIndex As Integer = 0 To oaTypedValue.GetUpperBound(0)
								oaResValue(iIndex) = oaTypedValue(iIndex).Value
							Next
							Return oaResValue
						Else
							Return Nothing
						End If
					Else
						Return Nothing
					End If
				Else
					Return Nothing
            End If
         Catch oEx As Exception
            System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "ProjectData - ItemArray")
            Return Nothing
         End Try
      End Get
      Set(ByVal oaValue As System.Object())
         If oaValue Is Nothing Then
            System.Windows.Forms.MessageBox.Show(CStr(mdicMain Is Nothing), "12_799")
         Else
            If mdicMain.Contains(sKey) Then
               Dim oXrecord As Autodesk.AutoCAD.DatabaseServices.Xrecord = New Xrecord()
					'	Dim oResBuffer As ResultBuffer = New ResultBuffer()
					'	Dim oTypedValue As TypedValue = DMAcadExt.AcadConst.ToTypedValue(oValue)
					'	oResBuffer.Add(oTypedValue)
					oXrecord.Data = DMAcadExt.AcadUtil.GetResBuffer(oaValue, False)
					mdicMain.Item(sKey) = oXrecord
               DMAcadExt.AcadTransaction.AppendDBObject(oXrecord)
            Else
               zzAddItem(sKey, oaValue)
            End If
         End If

      End Set
   End Property
   Public ReadOnly Property KeyExists(ByVal sKey As String) As Boolean
      Get
         Return (mdicMain IsNot Nothing) AndAlso mdicMain.Contains(sKey)
      End Get
   End Property
	ReadOnly Property ControlTypes() As enControlType()
		Get
			Return diaControlTypes
		End Get
	End Property


	Private Sub zzAddItem(ByVal sKey As String, ByVal oValue As System.Object)
		Dim oXrecord As Autodesk.AutoCAD.DatabaseServices.Xrecord = New Xrecord()
		'Dim oResBuffer As ResultBuffer = New ResultBuffer()
		'Dim tTypedValue As TypedValue = DMAcadExt.AcadConst.ToTypedValue(oValue)
		'	oResBuffer.Add(tTypedValue)

		'	Dim oTest As ResultBuffer = DMAcadExt.AcadUtil.GetResBuffer(oValue)




		Try
			oXrecord.Data = DMAcadExt.AcadUtil.GetResBuffer(oValue, False)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & mdicMain.Count.ToString() & vbCrLf & sKey & vbCrLf & oValue.ToString() & vbCrLf & oValue.GetType().ToString(), "Error #2913")
		End Try




		Try
			mdicMain.SetAt(sKey, oXrecord)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & mdicMain.Count.ToString() & vbCrLf & sKey & vbCrLf & oValue.ToString() & vbCrLf & oValue.GetType().ToString(), "Error #2914")
		End Try

		'	MessageBox.Show(sKey & ":" & tTypedValue.ToString, "ADD  18_355")
		DMAcadExt.AcadTransaction.AppendDBObject(oXrecord)
	End Sub

	Private Sub zzAddItem(ByVal sKey As String, ByVal oaValue() As System.Object)
      Dim oXrecord As Autodesk.AutoCAD.DatabaseServices.Xrecord = New Xrecord()
		'Dim oResBuffer As ResultBuffer = New ResultBuffer()
		'Dim tTypedValue As TypedValue = DMAcadExt.AcadConst.ToTypedValue(oValue)
		'	oResBuffer.Add(tTypedValue)


		Try
			oXrecord.Data = DMAcadExt.AcadUtil.GetResBuffer(oaValue, False)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & mdicMain.Count.ToString() & vbCrLf & sKey, "Error #2815")

		End Try
		Try
			mdicMain.SetAt(sKey, oXrecord)
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & mdicMain.Count.ToString() & vbCrLf & sKey, "Error #2816")
		End Try

		'	MessageBox.Show(sKey & ":" & tTypedValue.ToString, "ADD  18_355")
		DMAcadExt.AcadTransaction.AppendDBObject(oXrecord)
   End Sub
End Class
