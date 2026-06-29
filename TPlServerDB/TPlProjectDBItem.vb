Option Explicit On 
Option Strict On
Public Structure TPlProjectInfo
	Public ProjectID As Integer
	Public ProjectName As String
	Public DataSource As String
	Public CreatedDate As Date
	Public ModifiedDate As Date
	Public AccessedDate As Date
	Public CommitteeName As String
	Public LocalityName As String
	Public Provider As TPlProvider
	Public SingleProject As Boolean
End Structure
Public Class TPlProjectDBItem
   Inherits DMCommon.ItemData
	Public Shared ResourceTheme As Integer = 12
	Private miID As Integer
	Private msDataSource As String
	Private miProvider As TPlProvider = TPlProvider.ProviderNotDefined
	Private mbSingleProject As Boolean
	Public Property ID() As Integer
		Get
			ID = miID
		End Get
		Set(ByVal iValue As Integer)
			miID = iValue
			MyBase.ListIndex = iValue
		End Set
	End Property
	Public Property DataSource() As String
		Get
			DataSource = msDataSource
		End Get
		Set(ByVal sValue As String)
			msDataSource = sValue
			MyBase.ListDispData = sValue
		End Set
	End Property
	Public Property Provider() As TPlProvider
		Get
			Provider = miProvider
		End Get
		Set(ByVal iValue As TPlProvider)
			miProvider = iValue
		End Set
	End Property
	Public Property SingleProject() As Boolean
		Get
			SingleProject = mbSingleProject
		End Get
		Set(ByVal bValue As Boolean)
			mbSingleProject = bValue
		End Set
	End Property
End Class
