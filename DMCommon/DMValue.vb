Option Explicit On
Option Strict On

Public Structure DMValue
	Public Enum enDataType
		[Integer]
		[Double]
		[String]
		[Boolean]
		[Date]
	End Enum
	Private miValue As Integer
	Private mdValue As Double
	Private msValue As String
	Private mbValue As Boolean
	Private mdtValue As Date
	Private miType As enDataType
	Private mbHasValue As Boolean
	Private mbChanged As Boolean


	Public Sub New(ByVal iValue As Integer)
		miType = enDataType.Integer
		miValue = iValue
		mbHasValue = True
	End Sub
	Public Sub New(ByVal dValue As Double)
		miType = enDataType.Double
		mdValue = dValue
		mbHasValue = True
	End Sub
	Public Sub New(ByVal oValue As System.Object, ByVal iDataType As enDataType)
		Select Case iDataType
			Case enDataType.Integer
				If oValue IsNot Nothing AndAlso oValue.GetType().ToString() = "System.Int32" Then
					miValue = DirectCast(oValue, Integer)
				End If
			Case enDataType.String
				If oValue.GetType().ToString() = "System.String" Then
					msValue = DirectCast(oValue, String)
				End If
			Case enDataType.Double
		End Select
	End Sub
	Public Sub New(ByVal sValue As String)
		miType = enDataType.String
		msValue = sValue
		mbHasValue = True
	End Sub
	Public Sub New(ByVal bValue As Boolean)
		miType = enDataType.Boolean
		mbValue = bValue
		mbHasValue = True
	End Sub
	Public Sub New(ByVal dtValue As Date)
		miType = enDataType.Date
		mdtValue = dtValue
		mbHasValue = True
	End Sub
	Public Sub Update(ByVal iValue As Integer)
		mbChanged = True
		miValue = iValue
	End Sub
	Public Sub Update(ByVal dValue As Double)
		mbChanged = True
		mdValue = dValue
	End Sub
	Public Sub Update(ByVal sValue As String)
		mbChanged = True
		msValue = sValue
	End Sub
	Public Sub Update(ByVal bValue As Boolean)
		mbChanged = True
		mbValue = bValue
	End Sub
	Public Sub Update(ByVal dtValue As Date)
		mbChanged = True
		mdtValue = dtValue
	End Sub
	Public Property IntValue() As Integer
		Get
			Return miValue
		End Get
		Set(ByVal iValue As Integer)
			miValue = iValue
			mbHasValue = True
		End Set
	End Property
	Public Property DoubleValue() As Double
		Get
			Return mdValue
		End Get
		Set(ByVal dValue As Double)
			mdValue = dValue
			mbHasValue = True
		End Set
	End Property
	Public Property StrValue() As String
		Get
			Return msValue
		End Get
		Set(ByVal sValue As String)
			msValue = sValue
			mbHasValue = True
		End Set
	End Property
	Public Property BoolValue() As Boolean
		Get
			Return mbValue
		End Get
		Set(ByVal bValue As Boolean)
			mbValue = bValue
			mbHasValue = True
		End Set
	End Property
	Public Property DateValue() As Date
		Get
			Return mdtValue
		End Get
		Set(ByVal dtValue As Date)
			mdtValue = dtValue
			mbHasValue = True
		End Set
	End Property
	Public Function GetString() As String
		  
		If mbHasValue Then
			Select Case miType
				Case enDataType.Boolean
					Return mbValue.ToString()
				Case enDataType.Date
					Return mdtValue.ToString()
				Case enDataType.Double
					Return mdValue.ToString()
				Case enDataType.Integer
					Return miValue.ToString()
				Case enDataType.String
					Return msValue
				Case Else
					Return String.Empty
			End Select
		Else
			Return String.Empty
		End If
	End Function
	Public ReadOnly Property HasValue() As Boolean
		Get
			Return mbHasValue
		End Get
	End Property
	Public ReadOnly Property Changed() As Boolean
		Get
			Return mbChanged
		End Get
	End Property
	Public Function ToObject() As System.Object
		Select Case miType
			Case enDataType.Boolean
				Return mbValue
			Case enDataType.Date
				Return mdtValue
			Case enDataType.Double
				Return mdValue
			Case enDataType.Integer
				Return miValue
			Case enDataType.String
				Return msValue
			Case Else
				Return Nothing
		End Select
	End Function
	Public Overrides Function ToString() As String
		Return Me.ToObject().ToString()
	End Function
End Structure
