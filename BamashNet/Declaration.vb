Option Explicit On
Option Strict On

Module Declaration



	Public Const gsAppName As String = "Bamash"
	Public Const gsRegSectionParameters As String = "Parameters"
	Public Const glTypeMain As Long = 1&

	Public gsAreaFmt As String
	Public gsInpBlock As String
	Public gsInpParcel As String
	Public gsInpCity As String
	Public gsInpStreet As String
	Public gsInpBldNum As String
	Public gdParcelArea As Double
	'Public gsInpCity As String
	'Public gsInpCity As String

	Public gfrmEditBlockAttr As frmEditBlockAttr
	'Public gfrmMsg As frmMsg
	Public gfrmParam As frmParam

	Public gsPrmLayerPaint As String
	Public Const gsPrmLayerPaintDflt As String = "BLDH002"

	Public gsPrmLayerTable As String
	Public Const gsPrmLayerTableDflt As String = "DRWS031"

	Public gdPrmInterval As Double
	Public Const gdPrmIntervalDflt As Double = 12.0#


	Public giPrmDec As Integer
	Public Const giPrmDecDflt As Integer = 1

	Public gdPrmZebraAngle As Double
	Public Const gdPrmZebraAngleDflt As Double = 45.0#

	Public gdPrmZebraWidth As Double
	Public Const gdPrmZebraWidthDflt As Double = 0.8#

	Public gdPrmLegendZebraWidth As Double
	Public Const gdPrmLegendZebraWidthDflt As Double = 0.2

	Public gdPrmPaintScale As Double
	Public Const gdPrmPaintScaleDflt As Double = 100.0#

	Public gdPrmBorderWidth As Double
	Public Const gdPrmBorderWidthDflt As Double = 0.03

	Public gdPrmDissolveBorderWidth As Double
	Public Const gdPrmDissolveBorderWidthDflt As Double = 0.2

	Public gbPrmExproZebra As Boolean
	Public Const gbPrmExproZebraDflt As Boolean = True

	Public gbPrmSubNum As Boolean
	Public gbPrmMainNum As Boolean

	Public Const gbPrmSubNumDflt As Boolean = True
	Public Const gbPrmMainNumDflt As Boolean = True

	Public giPrmDissolveBorderColor As Integer '??????


	''''''''	Public goTopoMaster As GGCOTOPOMASTERLib.ggTopoMas1

	'Session Variable



	Public grsPrint As DataTable
	Public goPrintRow As DataRow

	Public mrsShare As DataTable
	
	Public gdicColors As Dictionary(Of Integer, Integer)
	'Public Drawing Data

	Public gdTotalArea As Double



End Module

