Option Explicit On
Option Strict On
Public Class Parameters
	Private Const mdLegendPaintFactorDflt As Double = 0.4
	Private Const msLegendPaintFactorSettingKey As String = "LegendPaintFactor"
	Private Shared mdLegendPaintFactor As Double

	Private Shared mdStraightenToleranceDflt As Double = 0.01
	Private Const msStraightenToleranceSettingKey As String = "StraightenTolerance"
	Private Shared mdStraightenTolerance As Double


	Public Shared Sub Init()

		mdLegendPaintFactor = zzLegendPaintFactorSetting
		mdStraightenTolerance = zzStraightToleranceSetting
	End Sub
	Public Shared Sub Save()
		zzLegendPaintFactorSetting = mdLegendPaintFactor
		zzStraightToleranceSetting = mdStraightenTolerance
	End Sub
	Public Shared Property LegendPaintFactor() As Double
		Get
			Return mdLegendPaintFactor
		End Get
		Set(ByVal dValue As Double)
			mdLegendPaintFactor = dValue
		End Set
	End Property
	Public Shared Property StraightenTolerance() As Double
		Get
			Return mdStraightenTolerance
		End Get
		Set(ByVal dValue As Double)
			mdStraightenTolerance = dValue
		End Set
	End Property


	Private Shared Property zzLegendPaintFactorSetting() As Double
		Get
			Dim sSettting As String = Microsoft.VisualBasic.GetSetting(Common.AppName, Common.SettingSectionName, msLegendPaintFactorSettingKey, mdLegendPaintFactorDflt.ToString())
			Dim dSetting As Double
			Try
				dSetting = Convert.ToDouble(sSettting)
			Catch
				dSetting = 0.4
         End Try
         MessageBox.Show(CStr(dSetting), "03_776")
			Return dSetting
		End Get
		Set(ByVal dValue As Double)
			Dim sSetting As String
			sSetting = Convert.ToString(dValue)
			Microsoft.VisualBasic.SaveSetting(Common.AppName, Common.SettingSectionName, msLegendPaintFactorSettingKey, sSetting)
		End Set
	End Property
	Private Shared Property zzStraightToleranceSetting() As Double
		Get
			Dim sSettting As String = Microsoft.VisualBasic.GetSetting(Common.AppName, Common.SettingSectionName, msStraightenToleranceSettingKey, mdStraightenToleranceDflt.ToString())
			Dim dSetting As Double
			Try
				dSetting = Convert.ToDouble(sSettting)
			Catch
				dSetting = mdStraightenToleranceDflt
			End Try
			Return dSetting
		End Get
		Set(ByVal dValue As Double)
			Dim sSetting As String
			sSetting = Convert.ToString(dValue)
			Microsoft.VisualBasic.SaveSetting(Common.AppName, Common.SettingSectionName, msStraightenToleranceSettingKey, sSetting)
		End Set
	End Property
End Class
