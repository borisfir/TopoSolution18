using System;

namespace DMObjects
{
	/// <summary>
	/// Summary description for VarData.
	/// </summary>
	public   enum VarDataType
	{
		NotData,
		Int32,
		String

	}

	public class VarData
	{  public int Int32; 
		public string String;
		public VarDataType DataType;
		public VarData()
		{
			DataType = VarDataType.NotData;
		}
		public VarData(int iValue)
		{
			Int32 = iValue;
			DataType = VarDataType.Int32;
		}
		public VarData(string sValue)
		{
			String = sValue; 
			DataType = VarDataType.String;
		}
		public System.Object GetValue()
		{
			switch(DataType)
			{
				case VarDataType.Int32:
					return (System.Object) Int32;
				case VarDataType.String:
					return (System.Object) String;
				default:
					return null;

			}

		}
	}
}
