namespace MobiFlight.Panels
{
    partial class JeehellInputPanel
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
            this.fsuipcLoadPresetGroupBox = new System.Windows.Forms.GroupBox();
            this.hintLabel = new System.Windows.Forms.Label();
            this.ValueTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.EventIdLabel = new System.Windows.Forms.Label();
            this.fsuipcPresetComboBox = new System.Windows.Forms.ComboBox();
            this.fsuipcLoadPresetGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // fsuipcLoadPresetGroupBox
            // 
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.hintLabel);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.ValueTextBox);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.label1);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.EventIdLabel);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.fsuipcPresetComboBox);
            this.fsuipcLoadPresetGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fsuipcLoadPresetGroupBox.Location = new System.Drawing.Point(0, 0);
            this.fsuipcLoadPresetGroupBox.Name = "fsuipcLoadPresetGroupBox";
            this.fsuipcLoadPresetGroupBox.Size = new System.Drawing.Size(211, 104);
            this.fsuipcLoadPresetGroupBox.TabIndex = 24;
            this.fsuipcLoadPresetGroupBox.TabStop = false;
            this.fsuipcLoadPresetGroupBox.Text = "Jeehell Data Pump Settings";
            // 
            // hintLabel
            // 
            this.hintLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hintLabel.Location = new System.Drawing.Point(3, 72);
            this.hintLabel.Name = "hintLabel";
            this.hintLabel.Size = new System.Drawing.Size(205, 29);
            this.hintLabel.TabIndex = 18;
            this.hintLabel.Text = "label2";
            // 
            // ValueTextBox
            // 
            this.ValueTextBox.Location = new System.Drawing.Point(81, 49);
            this.ValueTextBox.Name = "ValueTextBox";
            this.ValueTextBox.Size = new System.Drawing.Size(100, 20);
            this.ValueTextBox.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(27, 52);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Value";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // EventIdLabel
            // 
            this.EventIdLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.EventIdLabel.Location = new System.Drawing.Point(27, 25);
            this.EventIdLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.EventIdLabel.Name = "EventIdLabel";
            this.EventIdLabel.Size = new System.Drawing.Size(49, 13);
            this.EventIdLabel.TabIndex = 15;
            this.EventIdLabel.Text = "Function";
            this.EventIdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fsuipcPresetComboBox
            // 
            this.fsuipcPresetComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fsuipcPresetComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.fsuipcPresetComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.fsuipcPresetComboBox.FormattingEnabled = true;
            this.fsuipcPresetComboBox.Location = new System.Drawing.Point(81, 22);
            this.fsuipcPresetComboBox.Name = "fsuipcPresetComboBox";
            this.fsuipcPresetComboBox.Size = new System.Drawing.Size(124, 21);
            this.fsuipcPresetComboBox.TabIndex = 13;
            this.fsuipcPresetComboBox.SelectedIndexChanged += new System.EventHandler(this.fsuipcPresetComboBox_SelectedIndexChanged);
            // 
            // JeehellInputPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fsuipcLoadPresetGroupBox);
            this.Name = "JeehellInputPanel";
            this.Size = new System.Drawing.Size(211, 104);
            this.fsuipcLoadPresetGroupBox.ResumeLayout(false);
            this.fsuipcLoadPresetGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox fsuipcLoadPresetGroupBox;
        private System.Windows.Forms.ComboBox fsuipcPresetComboBox;
        private System.Windows.Forms.TextBox ValueTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label EventIdLabel;
        private System.Windows.Forms.Label hintLabel;
    }
}
