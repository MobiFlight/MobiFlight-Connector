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
            this.label2 = new System.Windows.Forms.Label();
            this.SubStringToTextBox = new System.Windows.Forms.TextBox();
            this.SubStringToLabel = new System.Windows.Forms.Label();
            this.SubStringFromTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(3, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Substring from position";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SubStringToTextBox
            // 
            this.SubStringToTextBox.Location = new System.Drawing.Point(219, 1);
            this.SubStringToTextBox.Name = "SubStringToTextBox";
            this.SubStringToTextBox.Size = new System.Drawing.Size(52, 20);
            this.SubStringToTextBox.TabIndex = 20;
            // 
            // SubStringToLabel
            // 
            this.SubStringToLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.SubStringToLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SubStringToLabel.Location = new System.Drawing.Point(187, 4);
            this.SubStringToLabel.Name = "SubStringToLabel";
            this.SubStringToLabel.Size = new System.Drawing.Size(26, 13);
            this.SubStringToLabel.TabIndex = 19;
            this.SubStringToLabel.Text = "to";
            this.SubStringToLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SubStringFromTextBox
            // 
            this.SubStringFromTextBox.Location = new System.Drawing.Point(132, 1);
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
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SubStringToTextBox);
            this.Controls.Add(this.SubStringToLabel);
            this.Controls.Add(this.SubStringFromTextBox);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(400, 0);
            this.Name = "SubstringModifierPanel";
            this.Size = new System.Drawing.Size(400, 24);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox SubStringToTextBox;
        private System.Windows.Forms.Label SubStringToLabel;
        private System.Windows.Forms.TextBox SubStringFromTextBox;
    }
}
