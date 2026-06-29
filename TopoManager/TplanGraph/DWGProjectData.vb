
Option Explicit On
Option Strict On
Imports DMCommon.Functions

Public Class DWGProjectData
   Inherits TopoManager.ProjectData
   Const msProjectCodeKey As String = "ProjectCode"
   Const msDetailUserFirstKey As String = "DetailUserFirst"
   Const msDetailUserKey As String = "DetailUser"


   Public Overrides Sub OpenData(bCreateValues As Boolean, bReadOnly As Boolean)


		MyBase.OpenDictionary(bCreateValues, bReadOnly)
   End Sub

   Public Overrides Sub Update()

   End Sub
   Public ReadOnly Property ProjectCode As Integer
		Get
			Dim iProjectCode As Integer
			Dim oValue As System.Object = MyBase.Item(msProjectCodeKey)
			If oValue IsNot Nothing AndAlso Integer.TryParse(oValue.ToString(), iProjectCode) Then
				Return iProjectCode
			Else
				Return 0
			End If
			'  System.Windows.Forms.MessageBox.Show(CStr(oValue Is Nothing), "02_114a")

		End Get

	End Property
   Public ReadOnly Property DetailNo As Integer
      Get
			Dim oRes As System.Object = MyBase.ItemArray(msDetailUserKey)
			If oRes Is Nothing Then
				Return -1
			Else
				Dim oaValues() As System.Object = DirectCast(oRes, System.Object())
				If oaValues IsNot Nothing Then
					Dim tDetailUser As DetailUser = New DetailUser(oaValues)
					Return tDetailUser.DetailNo
				Else
					Return 0
				End If
			End If
			'Dim oaValues() As System.Object = MyBase.ItemArray(msDetailUserKey)

			'   System.Windows.Forms.MessageBox.Show(CStr(oaValues Is Nothing), "02_115a")
		End Get
    
   End Property
   Public Sub UpdateData(iProjectCode As Integer, iDetailNo As Integer)
		Dim iFirstProjectCode As Integer = 0
		Dim oaValues() As System.Object = New System.Object() {iDetailNo, System.Environment.UserName, System.Environment.MachineName, Date.Now}
      If MyBase.KeyExists(msDetailUserFirstKey) AndAlso MyBase.KeyExists(msProjectCodeKey) Then
         iFirstProjectCode = Me.ProjectCode
      End If
		'   MessageBox.Show(MyBase.KeyExists(msDetailUserFirstKey).ToString() & ":" & MyBase.KeyExists(msProjectCodeKey).ToString(), "08_090")
		'    MessageBox.Show(iFirstProjectCode.ToString() & ":" & iProjectCode.ToString(), "08_091")
		'	DMCommon.Debug.MsgBox("!UpdateData", MyBase.KeyExists(msDetailUserFirstKey), MyBase.KeyExists(msProjectCodeKey), iFirstProjectCode, iProjectCode, iDetailNo)
		If iProjectCode <> iFirstProjectCode Then
         MyBase.Item(msProjectCodeKey) = iProjectCode
         MyBase.ItemArray(msDetailUserFirstKey) = oaValues
      End If
      MyBase.ItemArray(msDetailUserKey) = oaValues


   End Sub
   Public Function GetDetailUserInfo(bFirst As Boolean) As DetailUser
      Dim tDetailUser As DetailUser = New DetailUser()
      Dim sKey As String
      If bFirst Then
         sKey = msDetailUserFirstKey
      Else
         sKey = msDetailUserKey
      End If
      Dim oaValues() As System.Object = MyBase.ItemArray(sKey)
      If oaValues IsNot Nothing Then
         tDetailUser = New DetailUser(oaValues)
      End If
      Return tDetailUser
   End Function
   Public Function GetPropertyView() As TopoPropertyView
      Dim oPropertyView As TopoPropertyView = New TopoPropertyView()
      Dim iProjectCode As Integer = Me.ProjectCode
      Dim sProjectName As String
      Dim tDetailUser As DetailUser
      If iProjectCode <> 0 Then
         sProjectName = TPlServerDB.ServerDB.CurrentServerDB.GetProjectName(iProjectCode)
      Else
         sProjectName = Nothing
      End If




		oPropertyView.SetProject(iProjectCode, sProjectName)
      tDetailUser = GetDetailUserInfo(True)
      tDetailUser.EmployeeName = TPlServerDB.ServerDB.CurrentServerDB.GetEmplioyeeName(UCase(tDetailUser.UserName))
      oPropertyView.SetFirstDetail(tDetailUser)
      tDetailUser = GetDetailUserInfo(False)
      tDetailUser.EmployeeName = TPlServerDB.ServerDB.CurrentServerDB.GetEmplioyeeName(UCase(tDetailUser.UserName))
		oPropertyView.SetDetail(tDetailUser)
		'	Dim oAssemblyInfo As Microsoft.VisualBasic.ApplicationServices.AssemblyInfo = My.Application.Info

		'DMCommon.Debug.MsgBox("13_217z", oAssemblyInfo.Version.Revision, oAssemblyInfo.Version.Build)




		Return oPropertyView
   End Function

   Public Sub New()

   End Sub
End Class
Public Structure DetailUser
   Private miDetailNo As Integer
   Private msUserName As String
   Private msEmployeeName As String
   Private msMachineName As String
   Private mdtDbDate As Date

   Public Sub New(oaValues() As System.Object)
      If oaValues.GetUpperBound(0) = 3 Then
         If oaValues(0) IsNot Nothing Then
            miDetailNo = DirectCast(oaValues(0), Integer)
         End If
         If oaValues(1) IsNot Nothing Then
            msUserName = DirectCast(oaValues(1), String)
         End If

         If oaValues(2) IsNot Nothing Then
            msMachineName = DirectCast(oaValues(2), String)
         End If

         If oaValues(3) IsNot Nothing Then
            Dim dValue As Double = DirectCast(oaValues(3), Double)
            mdtDbDate = Date.FromOADate(dValue)

         End If


      End If
   End Sub
   Public ReadOnly Property DetailNo As Integer
      Get
         Return miDetailNo
      End Get
   End Property
   Public ReadOnly Property UserName As String
      Get
         Return msUserName
      End Get
   End Property
   Public Property EmployeeName As String
      Get
         Return msEmployeeName
      End Get
      Set(sValue As String)
         msEmployeeName = sValue
      End Set
   End Property

   Public ReadOnly Property MachineName As String
      Get
         Return msMachineName
      End Get
   End Property
   Public ReadOnly Property DbDate As DateTime
      Get
         Return mdtDbDate
      End Get
   End Property
End Structure
