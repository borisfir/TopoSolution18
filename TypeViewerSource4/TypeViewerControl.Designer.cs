namespace TypeViewer
{
  partial class TypeViewerControl
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.typeTextBox = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // typeTextBox
      // 
      this.typeTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.typeTextBox.Location = new System.Drawing.Point(0, 0);
      this.typeTextBox.Multiline = true;
      this.typeTextBox.Name = "typeTextBox";
      this.typeTextBox.ReadOnly = true;
      this.typeTextBox.Size = new System.Drawing.Size(354, 368);
      this.typeTextBox.TabIndex = 0;
      // 
      // TypeViewerControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.typeTextBox);
      this.Name = "TypeViewerControl";
      this.Size = new System.Drawing.Size(354, 368);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    public System.Windows.Forms.TextBox typeTextBox;

  }
}
