Imports System.ComponentModel

Public Class TopoPropertyView
   Private miProjectCode As Integer
   Private msProjectName As String


   Private miFirstDetail As Integer
   Private msFirstUser As String
   Private msFirstEmployeeName As String

   Private msFirstMachine As String
   Private mdtFirstDate As Date


   Private miDetail As Integer
   Private msUser As String
	Private msEmployeeName As String
	Private msProgramVersion As String
	Private mdtProgramLastUpdate As Date

	Private msMachine As String
   Private mdtDate As Date

   <DisplayName("Project code"), Description("מס' פרויקט"), Category("Project")> _
   Public ReadOnly Property [Project_Code] As Integer
      Get
         Return miProjectCode
      End Get
   End Property
   <DisplayName("Project name"), Description("שם פרויקט"), Category("Project")> _
   Public ReadOnly Property [Project_Name] As String
      Get
         Return msProjectName
      End Get
   End Property
   <DisplayName("Version"), Description("גרסה בפתיחת עבודה"), Category("First using")> _
   Public ReadOnly Property First_Version As Integer
      Get
         Return miFirstDetail
      End Get
   End Property
   <DisplayName("User name"), Description("שם משתמש בפתיחת עבודה"), Category("First using")> _
   Public ReadOnly Property First_User As String
      Get
         Return msFirstUser
      End Get
   End Property
   <DisplayName("Employee name"), Description("שם עובד"), Category("First using")> _
   Public ReadOnly Property First_Employee_Name As String
      Get
         Return msFirstEmployeeName
      End Get
   End Property


   <DisplayName("Date Created"), Description("תעריך פתיחת עבודה"), Category("First using")> _
   Public ReadOnly Property Date_Created As Date
      Get
         Return mdtFirstDate
      End Get
   End Property
   <DisplayName("Mashine"), Description("שם מחשב\תחנה"), Category("First using")> _
   Public ReadOnly Property First_Mashine As String
      Get
         Return msFirstMachine
      End Get
   End Property
   <DisplayName("Version"), Description("גרסה אחרונה"), Category("Last_update")> _
   Public ReadOnly Property [Version] As Integer
      Get
         Return miDetail
      End Get
   End Property
   <DisplayName("User name"), Description("שם משתמש"), Category("Last_update")> _
   Public ReadOnly Property User As String
      Get
         Return msUser
      End Get
   End Property
   <Description("שם מחשב\תחנה"), Category("Last_update")> _
   Public ReadOnly Property Mashine As String
      Get
         Return msMachine
      End Get
   End Property
   <Description("תעריך"), Category("Last_update")> _
   Public ReadOnly Property Date_last_update As Date
      Get
         Return mdtDate
      End Get
   End Property

   <DisplayName("Employee Name"), Description("שם עובד"), Category("Last_update")> _
   Public ReadOnly Property Employee_Name As String
      Get
         Return msEmployeeName
      End Get
   End Property

	<DisplayName("Program Version"), Description("גרסת התוכנית"), Category("Program")>
	Public ReadOnly Property ProgramVersion As String
		Get
			Return msProgramVersion
		End Get
	End Property

	<DisplayName("Program Last Update"), Description("עדכון אחרון"), Category("Program")>
	Public ReadOnly Property ProgramLastUpdate As Date
		Get
			Return mdtProgramLastUpdate
		End Get
	End Property


	Public Sub SetProject(iProjectCode As Integer, sProjectName As String)
      miProjectCode = iProjectCode
      msProjectName = sProjectName
   End Sub
   Public Sub SetFirstDetail(tDetailUser As DetailUser)

      miFirstDetail = tDetailUser.DetailNo
      msFirstUser = tDetailUser.UserName
      msFirstMachine = tDetailUser.MachineName
      mdtFirstDate = tDetailUser.DbDate
      msFirstEmployeeName = tDetailUser.EmployeeName
   End Sub
	Public Sub SetDetail(tDetailUser As DetailUser)

		miDetail = tDetailUser.DetailNo
		msUser = tDetailUser.UserName
		msMachine = tDetailUser.MachineName
		mdtDate = tDetailUser.DbDate
		msEmployeeName = tDetailUser.EmployeeName
	End Sub

	Public Sub SetProgram(sProgramVersion As String, dtProgramLastUpdate As Date)

		msProgramVersion = sProgramVersion
		mdtProgramLastUpdate = dtProgramLastUpdate
	End Sub
	Public Sub SetProgram(oVersion As Version, dtProgramLastUpdate As Date)

		msProgramVersion = oVersion.Major.ToString() & "." & oVersion.Minor.ToString() '& "." & oVersion.Build.ToString() & "." & oVersion.Revision.ToString()
		mdtProgramLastUpdate = dtProgramLastUpdate
		mdtProgramLastUpdate = New DateTime(2000, 1, 1).AddDays(oVersion.Build).AddSeconds(oVersion.Revision * 2)

	End Sub
End Class
