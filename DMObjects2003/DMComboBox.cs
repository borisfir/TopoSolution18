using System;

namespace DMObjects
{
	/// <summary>
	/// Summary description for DMComboBox.
	/// </summary>
	/// 
	
	public class DMComboBox:System.Windows.Forms.ComboBox
	{
		
		private int iIndex;
		
		public DMComboBox()
		{
		base.ValueMember="ListIndex";
		base.DisplayMember = "ListDispData";

		}
		public int Index
		{
			get
			{
				return iIndex;
			}
			set
			{
				iIndex=value;
				if (base.Name=="")
				{
				
				base.Name="cmbX"+iIndex.ToString();
				}
			}
		}
		new public  ItemData     SelectedItem 
		{
			 
			get
			{
				ItemData a;
				a=new ItemData(5,"aa");
			 	return  (ItemData)  base.SelectedItem;
			//	return a;
			}
		}
	}
}
