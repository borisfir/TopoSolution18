using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace DMObjects
{
	/// <summary>
	/// Summary description for Core.
	/// </summary>
	public class Core
	{
		public Core()
		{			
		}

		
		#region function LoadIcon
		
		public static Icon LoadIcon(string iconName)
		{			
			Stream strm = Type.GetType("DMObjects.Core").Assembly.GetManifestResourceStream("DMObjects.res." + iconName);
			
			Icon ic = null;
			if(strm != null){
				ic = new System.Drawing.Icon(strm);
				strm.Close();
			}

			return ic;
		}
		public static Bitmap LoadBitmap(string bitmapName)
		{			
			Stream strm = Type.GetType("DMObjects.Core").Assembly.GetManifestResourceStream("DMObjects.res." + bitmapName);
			
			Bitmap bm = null;
			if(strm != null)
			{
				bm = new System.Drawing.Bitmap(strm);
				strm.Close();
			}

			return bm;
		}

		#endregion

		
		#region fucntion ConvertToDeciaml

		/// <summary>
		/// Converts string value to decimal.
		/// If convert fails, returns 0;
		/// </summary>
		/// <param name="val">String value to convert.</param>
		/// <returns></returns>
		public static decimal ConvertToDeciaml(string val)
		{
			decimal retVal = 0;

			try
			{
				retVal = Convert.ToDecimal(val);
			}
			catch(Exception x)
			{
				Console.WriteLine("Generic Exception Handler: {Core}", x.Message);
				retVal = 0;
			}

			return retVal;
		}

		#endregion
	}
}
