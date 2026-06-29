using System;

namespace DMObjects
{
	
	public class ItemsSortedCollection:System.Collections.CollectionBase
	{
		public ItemsSortedCollection()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public int  Add(ItemSorted oItemSorted )
		{
			return List.Add(oItemSorted);
		}

		public int Add(int iSubItemIndex , SortType iSortType , System.Windows.Forms.SortOrder iItemSortOrder) 
		{
			ItemSorted oItemSorted = new ItemSorted();
			oItemSorted.ListViewSubitemIndex = iSubItemIndex;
			oItemSorted.ItemSortType = iSortType;
			oItemSorted.ItemSortOrder = iItemSortOrder;
			return  List.Add(oItemSorted);
		}
		public void Remove(int iIndex )
		{
			if ((iIndex > Count - 1) | (iIndex < 0))
				System.Windows.Forms.MessageBox.Show("Index not valid!");
			else
				List.Remove(iIndex);
			
		}
		public bool Contains(int iIndex )
		{
			if ((iIndex > Count - 1) | (iIndex < 0))
			{
				System.Windows.Forms.MessageBox.Show("Index not valid!");
				return false;
			}
			else
				return List.Contains(iIndex);
		}

		public void Insert(int iIndex  , ItemSorted oItemSorted  )
		{
			if ((iIndex > Count - 1) | (iIndex < 0))
				System.Windows.Forms.MessageBox.Show("Index not valid!");
			else
				List.Insert(iIndex, oItemSorted);
		}
				 
		public void Insert(int iIndex  , int iSubItemIndex  , SortType iSortType    , System.Windows.Forms.SortOrder iItemSortOrder )
		{
			ItemSorted oItemSorted = new ItemSorted();
			if (iIndex > Count - 1 | iIndex < 0 )
				System.Windows.Forms.MessageBox.Show("Index not valid!");
			else
			{
				oItemSorted.ListViewSubitemIndex = iSubItemIndex;
				oItemSorted.ItemSortType = iSortType;
				oItemSorted.ItemSortOrder = iItemSortOrder;
				List.Insert(iIndex, oItemSorted);
			}
		}
		public int IndexOf(ItemSorted oItemSorted   ) 
		{
			return List.IndexOf(oItemSorted);
		}

		public ItemSorted this[int iIndex]  
		{
			get
			{
				return (ItemSorted)(List[iIndex]);
			}
			
			//				  {
			//              return CType(List.Item(iIndex),ItemSorted);
			//				  }
			
		}

	}

}
