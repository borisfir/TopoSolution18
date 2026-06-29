Option Explicit On
Option Strict On
'Imports MSWord
Public Class frmTextEditor
   Public msDefaultFolder As String
   Public msDefaultTxtFileName As String
   Public msDefaultPDFFileName As String
   Public Sub AppendText(sText As String)
      Dim iBase As Integer = 0
      Dim iAddT As Integer

      If sText IsNot Nothing Then
         iAddT = sText.Length
         If Me.txtMain.Text IsNot Nothing Then

            iBase = Me.txtMain.Text.Length
         End If
         Me.txtMain.AppendText(sText)


      End If

   End Sub
   Public Property ContentText As String
      Get
         Return Me.txtMain.Text
      End Get
      Set(sValue As String)
         Me.txtMain.Text = sValue
      End Set
   End Property
   Public Sub AddLine(sLine As String)
      Me.txtMain.AppendText(sLine & vbCrLf)
   End Sub

   Public Sub New()

      ' This call is required by the designer.
      InitializeComponent()

      ' Add any initialization after the InitializeComponent() call.

   End Sub

   Public Property DefaultFolder As String
      Get
         Return msDefaultFolder
      End Get
      Set(sValue As String)
         msDefaultFolder = sValue
      End Set
   End Property
   Public Property DefaultTxtFileName As String
      Get
         Return msDefaultTxtFileName
      End Get
      Set(sValue As String)
         msDefaultTxtFileName = sValue
      End Set
   End Property
   Public Property DefaultPDFFileName As String
      Get
         Return msDefaultPDFFileName
      End Get
      Set(sValue As String)
         msDefaultPDFFileName = sValue
      End Set
   End Property
   Private Sub tsbSaveAsTxt_Click(oSender As System.Object, e As EventArgs) Handles tsbSaveAsTxt.Click
      zzSaveText()

   End Sub
   Private Sub zzSaveText()
      System.IO.File.WriteAllLines(msDefaultFolder & "\" & msDefaultTxtFileName, Me.txtMain.Lines)
   End Sub
    

   Private Sub tsbOpenFolder_Click(oSender As System.Object, e As EventArgs) Handles tsbOpenFolder.Click
      Dim iID As Integer = Microsoft.VisualBasic.Interaction.Shell("Explorer.exe " & msDefaultFolder, AppWinStyle.NormalFocus, False, 100000)
   End Sub

   Private Sub tsbExit_Click(oSender As System.Object, e As EventArgs) Handles tsbExit.Click
      Me.Close()
   End Sub

   Private Sub tsbCreatePDF_Click(oSender As System.Object, e As EventArgs) Handles tsbCreatePDF.Click
      Dim sSourceFullName As String = msDefaultFolder & "\" & msDefaultTxtFileName
      Dim sOutputFullName As String = msDefaultFolder & "\" & msDefaultPDFFileName
      zzSaveText()
      Try
         MSWordApp.Util.CreatePDF(msDefaultFolder, msDefaultTxtFileName, sOutputFullName)
      Catch oEx As Exception
         System.Windows.Forms.MessageBox.Show(oEx.Message & vbCrLf & oEx.StackTrace, "04_179")
      End Try

   End Sub

   Private Sub frmTextEditor_Load(oSender As System.Object, e As EventArgs) Handles Me.Load
      '  System.Windows.Forms.MessageBox.Show(Me.Location.X.ToString() & vbCrLf & Me.Location.Y.ToString(), "Nathali")
      If Me.Location.X < 0 OrElse Me.Location.Y < 0 Then
         Me.Location = New System.Drawing.Point(240, 240)
      End If
   End Sub

  

   Private Sub frmTextEditor_Shown(oSender As System.Object, e As EventArgs) Handles Me.Shown
      ' System.Windows.Forms.MessageBox.Show(Me.Location.X.ToString() & vbCrLf & Me.Location.Y.ToString(), "Nathali")
      If Me.Location.X < 0 OrElse Me.Location.Y < 0 Then
         Me.Location = New System.Drawing.Point(400, 400)
      End If
   End Sub
End Class