using System;

namespace DMObjects
{
	/// <summary>
	/// Summary description for ICalcList.
	/// </summary>
	public interface ICalcList
	{
		string CalcValue(int iKey);
		DMObjects.ItemData[] GetGroupItems(int iGroupKey);
		DMObjects.ItemData[] GetItems();
		void Dispose();
	}
}
