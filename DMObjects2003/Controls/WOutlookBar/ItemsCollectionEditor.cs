using System;
using System.ComponentModel.Design;

namespace DMObjects.WOutlookBar
{
	public class ItemsCollectionEditor : System.ComponentModel.Design.CollectionEditor
	{
		public ItemsCollectionEditor(Type type) : base(type) 
		{ 
		}

		protected override object CreateInstance(Type itemType) {
			object obj = null;

			if(itemType.FullName == "DMObjects.WOutlookBar.Item"){
				obj = ((Bar)Context.Instance).Items.Add("bar Item",-1);
			}
			return obj;
		}
	}
}
