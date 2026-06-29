Option Explicit On 
Option Strict On
Imports DMCommon
Public Class BuildRightItem
   Inherits ItemData
	Private miMeasureID As Integer
	Private miCharactID As Integer
	Private miLocationID As Integer


	Public Property MeasureID() As Integer
		Get
			MeasureID = miMeasureID
		End Get
		Set(ByVal iValue As Integer)
			miMeasureID = iValue

		End Set
	End Property
	Public Property CharactID() As Integer
		Get
			CharactID = miCharactID
		End Get
		Set(ByVal iValue As Integer)
			miCharactID = iValue

		End Set
	End Property
	Public Property LocationID() As Integer
		Get
			LocationID = miLocationID
		End Get
		Set(ByVal iValue As Integer)
			miLocationID = iValue

		End Set
	End Property
End Class
