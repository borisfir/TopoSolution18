using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
namespace DMObjects.DataGrid
{
	/// <summary>
	/// Summary description for DataGridButtonTextStyle.
	/// </summary>
	public class DataGridButtonTextColumn:DataGridTextBoxColumn
	{
		Button moButton=new Button();
		public DataGridButtonTextColumn()
		{
			moButton.Visible = false;
			moButton.Text = "v";
			moButton.BackColor = System.Drawing.SystemColors.Control;
		}
		protected  override bool Commit(System.Windows.Forms.CurrencyManager oDataSource , int iRowNum) 
		{
			moButton.Bounds = Rectangle.Empty;
			base.Invalidate();
			return  base.Commit(oDataSource, iRowNum);
		}


		protected   override  void Edit(System.Windows.Forms.CurrencyManager oDataSource, int iRowNum, System.Drawing.Rectangle rectBounds, bool bReadOnly, string sInstantText, bool bCellIsVisible)
		{
		
			if (bCellIsVisible) 
			{
				moButton.Bounds = new Rectangle(rectBounds.X, rectBounds.Y + 1, 16, rectBounds.Height - 2);
				moButton.Visible = true;
			}
			else
				moButton.Visible = false;
		

			if (moButton.Visible)  
				DataGridTableStyle.DataGrid.Invalidate(rectBounds);
		 

		}
	}
}
