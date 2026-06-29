using System;

namespace DMObjects
{
	public enum SortType
	{
		IgnoreCase,
		MatchCase,
		Numerical
	}

	public class ItemData:System.Object
	{
		private int miListIndex = 0;   
		string  msListDispData;

		public ItemData(int iListIndex , string sListDispData)
		{
			miListIndex = iListIndex;
			msListDispData = sListDispData;
		}
		public ItemData()
		{
		}
		public int ListIndex
		{
			get
			{
				return miListIndex;
			}
			set 
			{
				miListIndex = value;
			}
		}
		public  string ListIndexStr
		{
			get
			{
				return miListIndex.ToString();
			}
		}
		public  string ListDispData
		{
			get
			{
				return msListDispData;
			}
			set
			{
				msListDispData=value;
			}
		}

		public override string ToString()
		{
		return msListDispData;
		}
 
		public static string ValueMember   
		{
			get 
			{	
				return "ListIndex";
			}
		}
		public static string DisplayMember   
		{
			get 
			{	
				return "ListDispData";
			}
		}
	}
	public class ItemDataDict:System.Collections.DictionaryBase,System.Collections.IList,ICalcList 
	{
		
		private class ItemDataArrayDict:System.Collections.DictionaryBase
		{
			public void Add(ItemData oItemData,int iGroupKey)
			{  
				if (!base.Dictionary.Contains(iGroupKey))
				{
					ItemData[] oaItemData = new ItemData[1];
					oaItemData[0] = oItemData;
					base.Dictionary.Add(iGroupKey,oaItemData);
				}
				else
				{
					ItemData[] oaItemData=(ItemData[])base.Dictionary[iGroupKey];
					int iNewIndex = oaItemData.GetUpperBound(0) + 1;
					ItemData[] oaItemDataNew = new ItemData[iNewIndex+1];
					Array.Copy(oaItemData, oaItemDataNew,iNewIndex);
					oaItemDataNew[iNewIndex] = oItemData;
					base.Dictionary.Remove(iGroupKey);
					base.Dictionary.Add(iGroupKey,oaItemDataNew);
				}
			}
			public  System.Object this [int iKey]
			{
				get
				{
					return (ItemData[])base.Dictionary[iKey];
				}
			}
			public bool Contains (int iGroupKey)
			{
				return base.Dictionary.Contains(iGroupKey);
			}
	//		public new void Clear()
	//		{
	//			base.Clear(); 
	//		}
			 
		}
		private int[] iaKeys;
		private ItemData[] moaItemData;
		private bool mbHasGrouping = false;
		ItemDataArrayDict mdicItemDataArrayDict;
		public ItemDataDict():base(){}
		public ItemDataDict(bool bHasGrouping):base()
		{
			mbHasGrouping = bHasGrouping;
			if (mbHasGrouping)
				mdicItemDataArrayDict = new ItemDataArrayDict();
		}
		public void Add(ItemData oItemData)
		{  
			zzInsert(true,oItemData.ListIndex, oItemData);
		}
		public void Add(ItemData oItemData,int iGroupKey)
		{  
			zzInsert(true,oItemData.ListIndex, oItemData);
			if (mbHasGrouping)
				mdicItemDataArrayDict.Add(oItemData,iGroupKey);
		}
		public void Add(int[] iaKeys,int iGroupKey)
		{  
			ItemData oItemData;
			if (mbHasGrouping)
				for (int i = 0;i <= iaKeys.GetUpperBound(0);i++)
				{
					oItemData = (ItemData)base.Dictionary[iaKeys[i]];
					mdicItemDataArrayDict.Add(oItemData,iGroupKey);
				}
		}
		public void Add(int iKey,int iGroupKey)
		{  
			ItemData oItemData;
			if (mbHasGrouping)
				{
					oItemData = (ItemData)base.Dictionary[iKey];
					mdicItemDataArrayDict.Add(oItemData,iGroupKey);
				}
		}
		public void Add(int iListIndex, string sListDispData)
		{
			ItemData oItemData = new ItemData(iListIndex, sListDispData);
			this.Add(oItemData);
		}
		public void Add(int iListIndex, string sListDispData, int iGroupKey)
		{
			ItemData oItemData = new ItemData(iListIndex, sListDispData);
			this.Add(oItemData,iGroupKey);
		}
		public int Add(System.Object oItemData)
		{
			this.Add((ItemData)oItemData);
			return base.Count - 1;
		}
		public  void AddAll(int iGroupKey)
		{
			mdicItemDataArrayDict.Add(null,iGroupKey);
		}
		public new void  Clear()
		{
			base.Clear();
			iaKeys = null;
			moaItemData = null;
		}
		protected virtual new void OnInsert (System.Object key , System.Object value )
		{
		}

		public void Remove(int iKey)
		{
			base.Dictionary.Remove((Object)iKey);
		}
		public void Remove (System.Object oItemData )
		{
			int iIndex=this.IndexOf(oItemData);
			this.RemoveAt(iIndex);
			
		}
		public   void RemoveAt (int iIndex )
		{
		   int iKey=iaKeys[iIndex];
			int[] iaKeysNew;
			int iNewIndex = iaKeys.GetUpperBound(0) - 1;
			iaKeysNew=new int[iNewIndex+1];
			Array.Copy(iaKeys,  iaKeysNew,iIndex);
			if(iIndex<=iNewIndex)			
				Array.Copy(iaKeys,iIndex+1,iaKeysNew,iIndex,iNewIndex-iIndex+1 );
				iaKeys=iaKeysNew;
			base.Dictionary.Remove(iKey);
		 
		}

		public bool Contains(int iKey)
		{
			return(base.Dictionary.Contains((Object)iKey));
		}
		public System.Boolean Contains(Object oItemData)
		{
			int iKey=((ItemData)oItemData).ListIndex;
			return(base.Dictionary.Contains((Object)iKey));
		}
		 

		public   int IndexOf ( System.Object oItemData )
		{
		   int iKey=((ItemData)oItemData).ListIndex;	 
			if (base.Dictionary.Contains((Object)iKey))
			{
				int i;
				for(i=0;iaKeys[i]!=iKey;i++){}
            return i;
			}
			else
				return -1;
		}
		public void Insert ( int iIndex , System.Object oItemData )
		{
			zzInsert(false,iIndex,oItemData);
		}
		public bool IsFixedSize
		{
			get{return false;}
		}

		public bool IsReadOnly
		{
			get{return false;}
		}
		public  System.Object this [int iIndex]
	{
			get
			{
		//		System.Collections.DictionaryEntry oEnt;
		//		string s=new string('a',iIndex+1);
		//		oEnt=new System.Collections.DictionaryEntry(iIndex,s);
		//		ItemData oItem=new ItemData(iIndex,s);
			//	oEnt.Key=100;
			//	oEnt.Value="aaa";
				int iKey = iaKeys[iIndex];
			//   return oItem;
			  	return (ItemData)base.Dictionary[iKey];
			//	return (System.Object)"aaa";
			}
	set{}
}

		public  ItemData GetItem (int iKey)
		{
  			 
			return ((ItemData)base.Dictionary[iKey]);
		}
		public  virtual  void SetItem  (int iKey,ItemData oItemData)
		{

			base.Dictionary[iKey] = oItemData; 		 
		}
		public ItemData[] Values
		{
			get
			{
			//05.11.04	ItemData[] oaItemData = new ItemData[base.Dictionary.Count];
			//05.11.04	(base.Dictionary.Values).CopyTo(oaItemData,0);
				return moaItemData;
			}
		}
		public string GetText(int iKey)
		{
			try
			{
				System.Object oTest = base.Dictionary[iKey];
				if (oTest!=null)
				{
					ItemData oTestItemData = (ItemData)oTest;
					return ((ItemData)base.Dictionary[iKey]).ListDispData;
				}
				else return null;
			}
			catch(Exception x)
			{
				Console.WriteLine("Generic Exception Handler: {ItemData}", x.Message);
				return "ERROR*";
			}

			
		}
		#region Implementation ICalcList 

		public DMObjects.ItemData[] GetGroup(int iGroupKey)
		{  
			if (mbHasGrouping)
				if(mdicItemDataArrayDict.Contains(iGroupKey))
				{
					object oGroup = mdicItemDataArrayDict[iGroupKey];
               if (oGroup == null)
						return this.GetItems();
					else
						return (ItemData[]) mdicItemDataArrayDict[iGroupKey];
				}
				else
					return null;
			else
				return null;
		}
		public string CalcValue(int iKey)
		{
			return GetText(iKey);
		}
		public DMObjects.ItemData[] GetGroupItems(int iGroupKey)
		{
			return GetGroup(iGroupKey);
		}
		public DMObjects.ItemData[] GetItems()
		{
			return Values;
		}
		public void Dispose()
		{
			if(mdicItemDataArrayDict!=null)
				mdicItemDataArrayDict.Clear();
		}
		#endregion

		private void zzInsert(bool bAdd, int iIndex, System.Object oItemData)
		{
			int[] iaKeysNew;
			ItemData[] oaItemDataNew;
			ItemData oItemDataX = (ItemData)oItemData;
			int iKey = oItemDataX.ListIndex;
			if(iaKeys == null)
			{
				iaKeys = new int[1];
				iaKeys[0] = iKey;
				moaItemData = new ItemData[1];
				moaItemData[0] = oItemDataX;
			}
			else
			{
				int iNewIndex = iaKeys.GetUpperBound(0) + 1;
				if (bAdd)
				{
					iIndex = iNewIndex;
				}
			
				iaKeysNew = new int[iNewIndex + 1];
				oaItemDataNew = new ItemData[iNewIndex + 1];
				Array.Copy(iaKeys, iaKeysNew,iIndex);
				Array.Copy(moaItemData, oaItemDataNew,iIndex);
				iaKeysNew[iIndex] = iKey;
				oaItemDataNew[iIndex] = oItemDataX;
				if(iIndex < iNewIndex)
				{			
					Array.Copy(iaKeys,iIndex,iaKeysNew,iIndex+1,iNewIndex-iIndex );
					Array.Copy(moaItemData,iIndex,oaItemDataNew,iIndex+1,iNewIndex-iIndex );
				}
				iaKeys = iaKeysNew;
				moaItemData = oaItemDataNew;
				
			}
			base.Dictionary.Add(iKey,oItemData);
		}
	}
	public class ItemDataList:System.Collections.SortedList 
	{
		public ItemDataList():base()
		{ 
		}
		public void Add(ItemData oItemData)
		{
			base.Add(oItemData.ListIndex,oItemData);
		}
		public void Remove(int iKey)
		{
			base.Remove((System.Object)iKey);
		}
		public bool ContainsKey(int iKey)
		{
			return(base.ContainsKey((Object)iKey));
		}
		public int IndexOfKey(int iKey)
		{
			return(base.IndexOfKey((Object)iKey));
		}
		public  virtual ItemData this [int iIndex]
		{
			get
			{
	  		return ((ItemData)base[iIndex]);} 
			set{base[iIndex] = value;} 		 
		}
		public string  GetValue (int iIndex)
		{
			return ((ItemData)base[iIndex]).ListDispData;
		}	 
		public void	SetValue(int iIndex,string sValue)
		{
			ItemData oItemData;
			oItemData= (ItemData)base[iIndex] ;
			oItemData.ListDispData=sValue;
		} 		 
	}

	public class ItemSorted
	{
		private System.Windows.Forms.SortOrder miSortOrder;
		private SortType miItemSortType; 
		private int miListViewSubitemIndex;

		public System.Windows.Forms.SortOrder ItemSortOrder
		{
			get
			{
				return   miSortOrder;
			}
			set
			{
				miSortOrder = value;
			}
		}
	
		public SortType ItemSortType
		{
			get
			{
				return miItemSortType;
			}
			set
			{
				miItemSortType=value;
			}
		}
	
		public int ListViewSubitemIndex
		{
			get
			{
				return miListViewSubitemIndex;
			}
			set
			{
				miListViewSubitemIndex=value;
			}
		}
	}



}
