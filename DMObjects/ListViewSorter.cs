using System;

namespace DMObjects
{

	public class ListViewSorter:System.Collections.IComparer
	{
		private int miItemSorted;    
		private System.Windows.Forms.SortOrder miSortOrder;   
		private SortType miItemSortType=SortType.IgnoreCase;   
		private bool mbSingleColumnSort=true;    
		private ItemsSortedCollection mcolItemsSortedCollection;   
		//	Implements System.Collections.IComparer
		public ListViewSorter()
		{
			mcolItemsSortedCollection=new ItemsSortedCollection();
			//
			// TODO: Add constructor logic here
			//
		}
		public int  Compare(Object oX  , Object oY ) 
			//Implements System.Collections.IComparer.Compare
		{
			System.Windows.Forms.ListViewItem lviX, lviY; 
			 

			lviX = (System.Windows.Forms.ListViewItem)oX;
			lviY = (System.Windows.Forms.ListViewItem)oY;
			if (mbSingleColumnSort)
				return   zzCompareBySingleColumn(lviX, lviY);
			else
				return   zzCompareByCollection(lviX, lviY);
		}

		private int zzCompareBySingleColumn(System.Windows.Forms.ListViewItem lviX  , System.Windows.Forms.ListViewItem lviY   ) 
		{
			string sX, sY;   
			int iOutput=0; 
			sX = lviX.SubItems[miItemSorted].Text;
			sY = lviY.SubItems[miItemSorted].Text;

			switch (miItemSortType)
			{
				case SortType.IgnoreCase:
					iOutput = string.Compare(sX, sY, true);
					break;
				case SortType.MatchCase:
					iOutput = string.Compare(sX, sY, false);
					break;
				case SortType.Numerical:
					try
					{
						iOutput = zzCompareNumbers(System.Convert.ToDouble(sX),System.Convert.ToDouble(sY));
					}
					catch (Exception e)   
					{
						Console.WriteLine("Generic Exception Handler: {0}", e.ToString());
					}
					break;
			}		
			if (miSortOrder == System.Windows.Forms.SortOrder.Ascending)
				return iOutput;
			else 
				return -iOutput;		 
																																
		}

		private int zzCompareByCollection(System.Windows.Forms.ListViewItem lviX , System.Windows.Forms.ListViewItem lviY   ) 
		{
			string sX, sY;
			int iOutput=0;
			//			ItemSorted oItemSorted;
			foreach (ItemSorted oItemSorted in mcolItemsSortedCollection)
			{
				sX = lviX.SubItems[oItemSorted.ListViewSubitemIndex].Text;
				sY = lviY.SubItems[oItemSorted.ListViewSubitemIndex].Text;
				switch(oItemSorted.ItemSortType)
				{
					case SortType.IgnoreCase:
						iOutput = string.Compare(sX, sY, true);
						break;
					case SortType.MatchCase:
						iOutput = string.Compare(sX, sY, false);
						break;
					case SortType.Numerical:
						try
						{
							iOutput = zzCompareNumbers(System.Convert.ToDouble(sX), System.Convert.ToDouble(sY));
						}
						catch (Exception e) 
						{
							Console.WriteLine("Generic Exception Handler: {0}", e.Message);
							iOutput=0;
						}
						break;
				}
				if (iOutput != 0)
				{
					if (oItemSorted.ItemSortOrder == System.Windows.Forms.SortOrder.Ascending)
						return iOutput;
					else
						return -iOutput;
				}
			}
			return 0;
		}
		private int zzCompareNumbers(double dX, double dY) 
		{
			if (dX > dY)
				return 1;
			else
				if (dX < dY)
				return -1;
			else
				return 0;
		}
		public SortType ItemSortType 
		{
			get
			{
				return miItemSortType;
			}
			set
			{
				miItemSortType = value;
			}
		}
		public void ColumnClick(int iColumn)
		{
			if ((miItemSorted == iColumn ) & ( miSortOrder == System.Windows.Forms.SortOrder.Ascending ))
				miSortOrder = System.Windows.Forms.SortOrder.Descending;
			else
				miSortOrder = System.Windows.Forms.SortOrder.Ascending;

			miItemSorted = iColumn;
			mbSingleColumnSort = true;
		}
		public int ItemSorted   
		{
			get
			{
				return (miItemSorted);
			}
			set
			{
				miItemSorted=value;
			}
		}


		public void PrintStatus()
		{
			// ItemSorted oItemSorted; 
			string sStatus="";
			foreach (ItemSorted oItemSorted in mcolItemsSortedCollection)
			{
				if (sStatus.Length != 0 )
					sStatus = sStatus + ",";
				sStatus = sStatus + System.Convert.ToString(oItemSorted.ListViewSubitemIndex);
			}
			Console.WriteLine (sStatus);
		}
		public ItemsSortedCollection ItemsSorted
		{
			get
			{
				return mcolItemsSortedCollection;
			}
		}
		public bool SingleColumnSort 
		{
			get
			{
				return mbSingleColumnSort;
			}
      
			set
			{
				mbSingleColumnSort = value;
			}
		}



	}

}
