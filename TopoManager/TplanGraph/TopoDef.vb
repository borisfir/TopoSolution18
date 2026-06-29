Option Explicit On
Option Strict On

Public Class TopoDefAAAA
	Private mstTopoDefID As TopoDefID
	Private msName As String
	Private msDescription As String = String.Empty



	'Private msaIncludeLayers As String()
	Private msIncludeLayers As String = String.Empty

	Private msLinkLayer As String = String.Empty

	Private msaCentroidBlocks As String()
	Private msCentroidLayers As String = String.Empty

	Private msCentroidBlocks As String = String.Empty
	Private msMissingCentroidLayer As String = String.Empty
	Private msODTableName As String = String.Empty


	Private mbCreateCentroid As Boolean

	Private mbLinkLayerEmpty As Boolean = False
	Private Shared miFormat As Integer = 0
	Public Const ValueDel As String = ","

	Public Sub New(ByVal oDataReader As IDataReader)
		zzNew(oDataReader)
	End Sub
	Public Sub New(ByVal sTopoName As String)
		Dim sComText As String = "SELECT * FROM TopoDefs WHERE Name='" & sTopoName & "'"
		Dim oDataReader As IDataReader = TPlServerDB.ServerDB.GetDataIReader(sComText)
		If oDataReader IsNot Nothing Then
			zzNew(oDataReader)
		End If
	End Sub
	Public Sub New(ByVal iTopoDefID As TopoDefID)
		Dim sComText As String = "SELECT * FROM TopoDefs WHERE (AppID=" & CStr(Common.AppID) & ") AND (FormatID=" & CStr(miFormat) & ") AND (ID=" & CStr(iTopoDefID.ID) & ")"

		Try
			Dim oDataReader As System.Data.IDataReader = TPlServerDB.ServerDB.GetDataIReader(sComText)
			If oDataReader IsNot Nothing Then
				zzNew(oDataReader)
			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "TopoDef - New")
		End Try


	End Sub

	Public Shared Property Format() As Integer
		Get
			Return miFormat
		End Get
		Set(ByVal iValue As Integer)
			miFormat = iValue
		End Set
	End Property
	Public Property ID() As TopoDefID
		Get
			Return mstTopoDefID
		End Get
		Set(ByVal stValue As TopoDefID)
			mstTopoDefID = stValue
		End Set
	End Property
	Public Property Name() As String
		Get
			Return msName
		End Get
		Set(ByVal sValue As String)
			msName = sValue
		End Set
	End Property
	Public Property Description() As String
		Get
			Return msDescription
		End Get
		Set(ByVal sValue As String)
			msDescription = sValue
		End Set
	End Property

	Public Property NewProperty() As String
		Get
			Return msDescription
		End Get
		Set(ByVal sValue As String)
			msDescription = sValue
		End Set
	End Property

	Public Property LinkLayer() As String
		Get
			Return msLinkLayer
		End Get
		Set(ByVal sValue As String)
			msLinkLayer = sValue
		End Set
	End Property
	Public ReadOnly Property MissingCentroidLayer() As String
		Get
			Return msMissingCentroidLayer
		End Get
	End Property
	Public ReadOnly Property ODTableName() As String
		Get
			Return msODTableName
		End Get
	End Property

	Public Property IncludeLayers() As String
		Get
			Return msIncludeLayers
		End Get
		Set(ByVal sValue As String)
			msIncludeLayers = sValue
		End Set
	End Property
	Public ReadOnly Property TopoLayers() As String
		Get
			If msLinkLayer.Length = 0 Then
				Return msIncludeLayers
			Else
				Return msLinkLayer
			End If

		End Get
	End Property
	Public ReadOnly Property CentroidBlockExists() As Boolean
		Get
			Return (msCentroidBlocks.Length <> 0)
		End Get
	End Property
	Public Property CentroidBlocks() As String()
		Get
			'zzDispArray(msaCentroidBlocks, "!!!Property")
			Return msaCentroidBlocks
		End Get
		Set(ByVal saValue As String())
			msaCentroidBlocks = saValue
		End Set
	End Property
	Public ReadOnly Property CentroidLayers() As String
		Get
			If msCentroidLayers.Length = 0 Then
				Return msCentroidLayers
			Else
				Return msCentroidLayers
			End If

		End Get
	End Property
	Public ReadOnly Property CleanupEnabled() As Boolean
		Get
			Return Not mstTopoDefID.TopoIsUnion
		End Get

	End Property
	Public ReadOnly Property LinkLayersExists() As Boolean
		Get
			Return (msLinkLayer.Length <> 0)
		End Get

	End Property
	Public Property LinkLayerEmpty() As Boolean
		Get
			Return mbLinkLayerEmpty
		End Get
		Set(ByVal bValue As Boolean)
			mbLinkLayerEmpty = bValue
		End Set
	End Property
	Public Property CreateCentroid() As Boolean
		Get
			Return mbCreateCentroid
		End Get
		Set(ByVal bValue As Boolean)
			mbCreateCentroid = bValue
		End Set
	End Property
	Public Shared Function AddDelim(ByVal sValue As String) As String
		Return ValueDel & sValue & ValueDel	'ValueDel & UCase(sValue) & ValueDel
	End Function
	Public Function GetAllLayers() As String()
		Const sDelim As String = ","
		Dim sOutput As String = Me.TopoLayers

		If msCentroidLayers.Length <> 0 Then
			sOutput &= sDelim & msCentroidLayers
		End If
		If msMissingCentroidLayer.Length <> 0 Then
			sOutput &= sDelim & msMissingCentroidLayer
		End If
		Return Strings.Split(sOutput, sDelim)
	End Function
	Private Sub zzNew(ByVal oDataReader As IDataReader)
		Try
			If oDataReader.Read Then

				For iIndex As Integer = 0 To oDataReader.FieldCount - 1

					Select Case oDataReader.GetName(iIndex)
						Case "ID"
							mstTopoDefID.ID = oDataReader.GetInt32(iIndex)
						Case "Name"
							msName = oDataReader.GetString(iIndex)
						Case "Description"
							If Not oDataReader.IsDBNull(iIndex) Then
								msDescription = oDataReader.GetString(iIndex)
							End If
						Case "IncludeLayers"
							If Not oDataReader.IsDBNull(iIndex) Then
								msIncludeLayers = oDataReader.GetString(iIndex)
								' msaIncludeLayers = Split(oDataReader.GetString(iIndex), ValueDel)
							End If
						Case "LinkLayers"
							If Not oDataReader.IsDBNull(iIndex) Then
								msLinkLayer = oDataReader.GetString(iIndex)
							End If
						Case "CentroidBlocks"
							If Not oDataReader.IsDBNull(iIndex) Then
								msCentroidBlocks = oDataReader.GetString(iIndex)
								msaCentroidBlocks = Strings.Split(msCentroidBlocks, ValueDel)
								'  zzDispArray(msaCentroidBlocks, "!!!TopoDef-zzNew-2")
							End If
						Case "CentroidLayers"
							If Not oDataReader.IsDBNull(iIndex) Then
								msCentroidLayers = oDataReader.GetString(iIndex)
							End If
						Case "CreateCentroid"
							mbCreateCentroid = oDataReader.GetBoolean(iIndex)
						Case "MissingCentroidLayer"
							If Not oDataReader.IsDBNull(iIndex) Then
								msMissingCentroidLayer = oDataReader.GetString(iIndex)
							End If
						Case "ODTableName"
							If Not oDataReader.IsDBNull(iIndex) Then
								msODTableName = oDataReader.GetString(iIndex)
							End If
					End Select
				Next

			End If
		Catch oEx As Exception
			System.Windows.Forms.MessageBox.Show(oEx.Message, "TopoDef - zzNew")
		End Try

	End Sub
	Private Sub zzSetTopoNames()
		'   Dim iTopoPurpose As TPlanGraph.enTopoPurpose
		If mstTopoDefID.TopoIsBase Then
			Select Case mstTopoDefID.BaseID
				Case TPlanGraph.enTopoPurpose.Parcel

			End Select
		End If

	End Sub
	Private Sub zzDispArrayAA(ByVal saVal() As String, ByVal sTitle As String)
		Dim sOut As String = String.Empty
		If saVal IsNot Nothing Then
			For iIndex As Integer = 0 To saVal.GetUpperBound(0)
				sOut += ":" & saVal(iIndex)
			Next
		Else
			sOut = "Nothing"
		End If

		System.Windows.Forms.MessageBox.Show(sOut, sTitle)
	End Sub
End Class
