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
            this.label2 = new System.Windows.Forms.Label();
            this.SubStringToTextBox = new System.Windows.Forms.TextBox();
            this.SubStringToLabel = new System.Windows.Forms.Label();
            this.SubStringFromTextBox = new System.Windows.Forms.TextBox();
            this.SubstringPanel.SuspendLayout();
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
            // label2
            // 
            this.label2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(3, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Substring from character";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SubStringToTextBox
            // 
            this.SubStringToTextBox.Location = new System.Drawing.Point(212, 2);
            this.SubStringToTextBox.Name = "SubStringToTextBox";
            this.SubStringToTextBox.Size = new System.Drawing.Size(52, 20);
            this.SubStringToTextBox.TabIndex = 20;
            // 
            // SubStringToLabel
            // 
            this.SubStringToLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.SubStringToLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SubStringToLabel.Location = new System.Drawing.Point(185, 5);
            this.SubStringToLabel.Name = "SubStringToLabel";
            this.SubStringToLabel.Size = new System.Drawing.Size(26, 13);
            this.SubStringToLabel.TabIndex = 19;
            this.SubStringToLabel.Text = "to";
            this.SubStringToLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SubStringFromTextBox
            // 
            this.SubStringFromTextBox.Location = new System.Drawing.Point(133, 2);
            this.SubStringFromTextBox.Name = "SubStringFromTextBox";
            this.SubStringFromTextBox.Size = new System.Drawing.Size(49, 20);
            this.SubStringFromTextBox.TabIndex = 17;
            // 
            // SubstringModifierPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.SubstringPanel);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(400, 0);
            this.Name = "SubstringModifierPanel";
            this.Size = new System.Drawing.Size(400, 25);
            this.SubstringPanel.ResumeLayout(false);
            this.SubstringPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel SubstringPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox SubStringToTextBox;
        private System.Windows.Forms.Label SubStringToLabel;
        private System.Windows.Forms.TextBox SubStringFromTextBox;
    }
}
