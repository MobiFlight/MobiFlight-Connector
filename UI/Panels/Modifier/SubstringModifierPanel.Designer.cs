namespace MobiFlight.UI.Panels.Modifier
{
    partial class SubstringModifierPanel
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
            this.SubstringPanel = new System.Windows.Forms.Panel();
            this.SubStringToTextBox = new System.Windows.Forms.TextBox();
            this.SubStringToLabel = new System.Windows.Forms.Label();
            this.SubStringFromTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.SubstringPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SubstringPanel
            // 
            this.SubstringPanel.AutoSize = true;
            this.SubstringPanel.Controls.Add(this.label2);
            this.SubstringPanel.Controls.Add(this.SubStringToTextBox);
            this.SubstringPanel.Controls.Add(this.SubStringToLabel);
            this.SubstringPanel.Controls.Add(this.SubStringFromTextBox);
            this.SubstringPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.SubstringPanel.Location = new System.Drawing.Point(0, 0);
            this.SubstringPanel.Name = "SubstringPanel";
            this.SubstringPanel.Size = new System.Drawing.Size(400, 25);
            this.SubstringPanel.TabIndex = 17;
            // 
            // SubStringToTextBox
            // 
            this.SubStringToTextBox.Location = new System.Drawing.Point(189, 2);
            this.SubStringToTextBox.Name = "SubStringToTextBox";
            this.SubStringToTextBox.Size = new System.Drawing.Size(52, 20);
            this.SubStringToTextBox.TabIndex = 20;
            // 
            // SubStringToLabel
            // 
            this.SubStringToLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.SubStringToLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SubStringToLabel.Location = new System.Drawing.Point(162, 5);
            this.SubStringToLabel.Name = "SubStringToLabel";
            this.SubStringToLabel.Size = new System.Drawing.Size(26, 13);
            this.SubStringToLabel.TabIndex = 19;
            this.SubStringToLabel.Text = "to";
            this.SubStringToLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SubStringFromTextBox
            // 
            this.SubStringFromTextBox.Location = new System.Drawing.Point(110, 2);
            this.SubStringFromTextBox.Name = "SubStringFromTextBox";
            this.SubStringFromTextBox.Size = new System.Drawing.Size(49, 20);
            this.SubStringFromTextBox.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(303, 34);
            this.label1.TabIndex = 0;
            this.label1.Text = "You can use ncalc expession syntax for more complex transformations.\r\n";
            // 
            // panel1
            // 
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(81, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(303, 34);
            this.panel1.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(3, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Substring to";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SubstringModifierPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.SubstringPanel);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(400, 0);
            this.Name = "SubstringModifierPanel";
            this.Size = new System.Drawing.Size(400, 70);
            this.SubstringPanel.ResumeLayout(false);
            this.SubstringPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel SubstringPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox SubStringToTextBox;
        private System.Windows.Forms.Label SubStringToLabel;
        private System.Windows.Forms.TextBox SubStringFromTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
    }
}
