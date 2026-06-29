using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace DMObjects
{
	/// <summary>
	/// Button control.
	/// </summary>
	[DefaultEvent("ButtonPressed"),]
	public class WButton : WControlBase
	{		
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public event ButtonPressedEventHandler ButtonPressed = null;

		private string m_Text = "";

		/// <summary>
		/// Default constructor.
		/// </summary>
		public WButton()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call
		}

		#region function Dispose

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
			// 
			// WButton
			// 
			this.Name = "WButton";
			this.Size = new System.Drawing.Size(80, 24);
			((System.ComponentModel.ISupportInitialize)(this)).EndInit();

		}
		#endregion


		#region function DrawControl

		protected override void DrawControl(Graphics g,bool hot)
		{	
			DrawControl(g,hot,false);
		}

		private void DrawControl(Graphics g,bool hot,bool pressed)
		{
			g.Clear(m_ViewStyle.GetButtonColor(hot,pressed));
			g.DrawRectangle(new Pen(m_ViewStyle.GetBorderColor(hot),2),this.ClientRectangle);

			StringFormat format  = new StringFormat();
			format.LineAlignment = StringAlignment.Center;
			format.Alignment     = StringAlignment.Center;

			Rectangle txtRect = this.ClientRectangle;
			if(pressed){
				txtRect.Location = new Point(txtRect.Left+1,txtRect.Top+1);
			}

			if(this.Enabled){
				g.DrawString(m_Text,this.Font,new SolidBrush(Color.Black),txtRect,format);
			}
			else{
				g.DrawString(m_Text,this.Font,new SolidBrush(Color.FromArgb(128,128,128)),txtRect,format);
			}
		}

		#endregion


		#region override OnMouseDown

		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if(this.Enabled){
				using(Graphics g = this.CreateGraphics()){
					DrawControl(g,true,true);
				}
			}
		}

		#endregion

		#region override OnMouseUp

		protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseUp(e);

			if(this.Enabled && this.IsMouseInControl){
				using(Graphics g = this.CreateGraphics()){
					DrawControl(g,this.Focused,false);
				}

				OnButtonClicked();
			}
		}

		#endregion

		#region override OnKeyDown

		protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if(this.Enabled && (e.KeyData == Keys.Space || e.KeyData == Keys.Enter)){
				using(Graphics g = this.CreateGraphics()){
					DrawControl(g,true,true);
				}

				OnButtonClicked();
			}
		}

		#endregion

		#region override OnKeyUp

		protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs e)
		{
			base.OnKeyUp(e);

			if(this.Enabled && (e.KeyData == Keys.Space || e.KeyData == Keys.Enter)){
				using(Graphics g = this.CreateGraphics()){
					DrawControl(g,this.Focused,false);
				}				
			}
		}

		#endregion
        

		#region Properties Implementation

		[
		Browsable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
		]
		public override string Text
		{
			get{ return m_Text; }

			set{ 
				m_Text = value; 
				this.Invalidate();
			}
		}

		#endregion

		#region Events Implementation

		#region function OnButtonClicked

		protected void OnButtonClicked()
		{
			if(this.ButtonPressed != null){
				this.ButtonPressed(this,new System.EventArgs());
			}
		}

		#endregion

		#endregion

	}
}
