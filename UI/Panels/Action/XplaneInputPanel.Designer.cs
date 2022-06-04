namespace MobiFlight.UI.Panels.Action
{
    partial class XplaneInputPanel
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
            this.xplaneGroupBox = new System.Windows.Forms.GroupBox();
            this.HintLabel = new System.Windows.Forms.Label();
            this.ValueTextBox = new System.Windows.Forms.TextBox();
            this.ValueLabel = new System.Windows.Forms.Label();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.PathLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.TypeComboBox = new System.Windows.Forms.ComboBox();
            this.xplaneGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // xplaneGroupBox
            // 
            this.xplaneGroupBox.Controls.Add(this.HintLabel);
            this.xplaneGroupBox.Controls.Add(this.ValueTextBox);
            this.xplaneGroupBox.Controls.Add(this.ValueLabel);
            this.xplaneGroupBox.Controls.Add(this.NameTextBox);
            this.xplaneGroupBox.Controls.Add(this.PathLabel);
            this.xplaneGroupBox.Controls.Add(this.label1);
            this.xplaneGroupBox.Controls.Add(this.TypeComboBox);
            this.xplaneGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xplaneGroupBox.Location = new System.Drawing.Point(0, 0);
            this.xplaneGroupBox.Name = "xplaneGroupBox";
            this.xplaneGroupBox.Size = new System.Drawing.Size(318, 158);
            this.xplaneGroupBox.TabIndex = 0;
            this.xplaneGroupBox.TabStop = false;
            this.xplaneGroupBox.Text = "Input settings";
            // 
            // HintLabel
            // 
            this.HintLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HintLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.HintLabel.Location = new System.Drawing.Point(88, 107);
            this.HintLabel.Name = "HintLabel";
            this.HintLabel.Size = new System.Drawing.Size(224, 38);
            this.HintLabel.TabIndex = 30;
            this.HintLabel.Text = "Supports variable value ($), input value (@) and placeholders (?,#, etc.)\r\n";
            // 
            // ValueTextBox
            // 
            this.ValueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ValueTextBox.Location = new System.Drawing.Point(88, 84);
            this.ValueTextBox.Name = "ValueTextBox";
            this.ValueTextBox.Size = new System.Drawing.Size(224, 20);
            this.ValueTextBox.TabIndex = 5;
            // 
            // ValueLabel
            // 
            this.ValueLabel.Location = new System.Drawing.Point(6, 84);
            this.ValueLabel.Name = "ValueLabel";
            this.ValueLabel.Size = new System.Drawing.Size(76, 20);
            this.ValueLabel.TabIndex = 4;
            this.ValueLabel.Text = "Value";
            this.ValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NameTextBox
            // 
            this.NameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NameTextBox.Location = new System.Drawing.Point(88, 58);
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.Size = new System.Drawing.Size(224, 20);
            this.NameTextBox.TabIndex = 3;
            // 
            // PathLabel
            // 
            this.PathLabel.AutoEllipsis = true;
            this.PathLabel.Location = new System.Drawing.Point(9, 59);
            this.PathLabel.Name = "PathLabel";
            this.PathLabel.Size = new System.Drawing.Size(73, 20);
            this.PathLabel.TabIndex = 2;
            this.PathLabel.Text = "DataRef";
            this.PathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "Input type";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TypeComboBox
            // 
            this.TypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TypeComboBox.FormattingEnabled = true;
            this.TypeComboBox.Items.AddRange(new object[] {
            "DataRef",
            "Command"});
            this.TypeComboBox.Location = new System.Drawing.Point(88, 31);
            this.TypeComboBox.Name = "TypeComboBox";
            this.TypeComboBox.Size = new System.Drawing.Size(121, 21);
            this.TypeComboBox.TabIndex = 0;
            // 
            // XplaneInputPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.xplaneGroupBox);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "XplaneInputPanel";
            this.Size = new System.Drawing.Size(318, 158);
            this.xplaneGroupBox.ResumeLayout(false);
            this.xplaneGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox xplaneGroupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox TypeComboBox;
        private System.Windows.Forms.TextBox ValueTextBox;
        private System.Windows.Forms.Label ValueLabel;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.Label PathLabel;
        private System.Windows.Forms.Label HintLabel;
    }
}
