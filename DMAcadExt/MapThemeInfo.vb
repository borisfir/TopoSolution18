Option Explicit On
Option Strict On
Public Structure MapThemeInfo
	Private miLinkCount As Integer
	Private miLinkWithArcCount As Integer
	Private miArcCount As Integer
	Private miLineLinkCount As Integer
	Private miCentroidBlocksCount As Integer
	Private miMPgonsCount As Integer
	Private miPgonSetCount As Integer
   Private miWorkAreaRingCount As Integer
   Private mbPgonSetIsCorrect As Boolean
   Public Property LinkCount As Integer
      Get
         Return miLinkCount
      End Get
      Set(iValue As Integer)
         miLinkCount = iValue
      End Set
   End Property
   Public Property MPgonsCount As Integer
      Get
         Return miMPgonsCount
      End Get
      Set(iValue As Integer)
         miMPgonsCount = iValue
      End Set
   End Property
   Public Property PgonSetCount As Integer
      Get
         Return miPgonSetCount
      End Get
      Set(iValue As Integer)
         miPgonSetCount = iValue
      End Set
   End Property

   Public Sub AddLink(iValue As Integer)
      miLinkCount += iValue
   End Sub
   Public Sub AddPolylineAAA(iValue As Integer)
      miLinkCount += iValue
   End Sub

   Public Sub Reset()
      miLinkCount = 0
      miLinkWithArcCount = 0
      miArcCount = 0
      miLineLinkCount = 0
      miCentroidBlocksCount = 0
   End Sub
   Public Property LinkWithArcCount As Integer
      Get
         Return miLinkWithArcCount
      End Get
      Set(iValue As Integer)
         miLinkWithArcCount = iValue
      End Set
   End Property
   Public Sub AddLinkWithArcCount(iValue As Integer)
      miLinkWithArcCount += iValue
   End Sub

   Public Property ArcCount As Integer
      Get
         Return miArcCount
      End Get
      Set(iValue As Integer)
         miArcCount = iValue
      End Set
   End Property
   Public Sub AddArcCount(iValue As Integer)
      If iValue > 0 Then
         miLinkWithArcCount += 1
         miArcCount += iValue
      End If

   End Sub

   Public Property LineLinkCount As Integer
      Get
         Return miLineLinkCount
      End Get
      Set(iValue As Integer)
         miLineLinkCount = iValue
      End Set
   End Property
   Public Sub AddLineLinkCount(iValue As Integer)
      miLineLinkCount += iValue
   End Sub
   Public Property WorkAreaRingCount As Integer
      Get
         Return miWorkAreaRingCount
      End Get
      Set(iValue As Integer)
         miWorkAreaRingCount = iValue
      End Set
   End Property
   Public Property CentroidBlocksCount As Integer
      Get
         Return miCentroidBlocksCount
      End Get
      Set(iValue As Integer)
         miCentroidBlocksCount = iValue
      End Set
   End Property
   Public Property PgonSetIsCorrect As Boolean
      Get
         Return mbPgonSetIsCorrect
      End Get
      Set(bValue As Boolean)
         mbPgonSetIsCorrect = bValue
      End Set
   End Property
   Public Sub AddCentroidBlocks(iValue As Integer)
      miCentroidBlocksCount += iValue
   End Sub
   Public Sub AddMpgons(iValue As Integer)
      miMPgonsCount += iValue
   End Sub
   Public Sub AddPgons(iValue As Integer)
      miPgonSetCount += iValue
   End Sub
   Public Sub AddWorkAreaRing(iValue As Integer)
      miWorkAreaRingCount += iValue


   End Sub
	Public Function LineLinksOK() As Boolean
		Return (miLineLinkCount = miLinkWithArcCount) AndAlso (miLinkCount > 0)
	End Function



End Structure
