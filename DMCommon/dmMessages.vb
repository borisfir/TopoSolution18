Option Explicit On
Option Strict On
Public Structure dmMessages
	Public Shared Function Message(ByVal iMsgID As Integer, ByVal ParamArray oParams() As System.Object) As String
		Dim sMsg As String = zzGetText(iMsgID)
		For iIndex As Integer = 0 To oParams.GetUpperBound(0)
			If oParams(iIndex) IsNot Nothing Then
				sMsg = Strings.Replace(sMsg, "|", oParams(iIndex).ToString(), , 1)
			End If
		Next
		Return sMsg
	End Function
	Public Shared Function GetText(ByVal iMsgID As Integer) As String
		Return zzGetText(iMsgID)
	End Function
	Private Shared Function zzGetText(ByVal iMsgID As Integer) As String

		Select Case iMsgID
			Case 301
				Return "הבלוק '|' לא תקין"
			Case 302
				Return "שטח חלקה בקובץ" & " " & "|" & " " & vbCrLf & "שטח חלקה במאגר מידע" & " " & "|"
			Case 303
				Return ";סטיית שטח" & " " & "שטח רשום | " & " שטח מחושב | "
			Case 304
				Return zzGetText(305) & " " & " לא נמצאת במאגר נתונים"
			Case 305
				' Return " '|' גוש '|' חלקה"
				Return "חלקה | גוש |"
			Case 306
				Return "בפוליגון לא נמצא בלוק"
			Case 307
				Return "הבלוק נמצא מחוץ לתחום של טופולוגיה"
			Case 308
				Return "מס' חלקה '|' הוא לא מספר"
			Case 309
				Return zzGetText(305) & " " & " מס' חלקה כפול"
			Case 310
				Dim s As String = " '|' "
				Return "שם נקודה" & s & " כפול "

			Case 311
				' Return "טופולוגיה '|' לא תקינה"
				Return "Topology '|' Invalid"
			Case 312
				Return "נקודה חדשה במצב מאושר"
			Case 313
				Return "?האם הינך בטוח שברצונך להמשיך"
			Case 314
				Return "אתה עומד לסגור יישום" & vbCrLf & "ולא שמרתה שינוים אחרונים" & vbCrLf & zzGetText(313)
			Case 315
				Return "נקודה ישנה קרובה לנקודה אחרת"
			Case 316
				Return "| polygons out of |"
			Case 317
				Return "The selected point is outside the polygon "
			Case 318
				Return "אי שאינו מחובר"
			Case 319
				Return "ההפרש  של שטחים הוא | מטר"
			Case 320
				Return "בצומת לא נמצא בלוק"
			Case 321
				Return "ישנם שינויים בחלקות: |,|"
			Case 322
				Return "המספר ג.ב. לא נמצא בתוכנית מס' |"
			Case 323
				Return "המספר ג.ב. | כבר קיים בתוכנית מס' |"
			Case 324
				Return "גוש לא מוגדר"
			Case 325
				Return "גוש לא מוגדר"
			Case 326
				Return "החלוקה לחלקה לא נמצאת"
			Case 327
				Return "החלקה לחלוקה לא נמצאת"
			Case 328
				Return "החלוקה בחלקה לא נמצאת"
			Case 329
				Return "טופולוגיה '|' לא קיימת או לא תקינה"
			Case 601
				Return ""
			Case 602
				Return ""
			Case 603
				Return ""
			Case 604
				Return ""
			Case Else
				Return String.Empty
		End Select

	End Function
End Structure

 