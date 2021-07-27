namespace MobiFlight.UI.Panels.Action
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JeehellInputPanel));
            this.fsuipcLoadPresetGroupBox = new System.Windows.Forms.GroupBox();
            this.hintLabel = new System.Windows.Forms.Label();
            this.ValueTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.EventIdLabel = new System.Windows.Forms.Label();
            this.fsuipcPresetComboBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.fsuipcLoadPresetGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // fsuipcLoadPresetGroupBox
            // 
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.label5);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.hintLabel);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.ValueTextBox);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.label1);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.EventIdLabel);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.fsuipcPresetComboBox);
            resources.ApplyResources(this.fsuipcLoadPresetGroupBox, "fsuipcLoadPresetGroupBox");
            this.fsuipcLoadPresetGroupBox.Name = "fsuipcLoadPresetGroupBox";
            this.fsuipcLoadPresetGroupBox.TabStop = false;
            // 
            // hintLabel
            // 
            resources.ApplyResources(this.hintLabel, "hintLabel");
            this.hintLabel.Name = "hintLabel";
            // 
            // ValueTextBox
            // 
            resources.ApplyResources(this.ValueTextBox, "ValueTextBox");
            this.ValueTextBox.Name = "ValueTextBox";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // EventIdLabel
            // 
            resources.ApplyResources(this.EventIdLabel, "EventIdLabel");
            this.EventIdLabel.Name = "EventIdLabel";
            // 
            // fsuipcPresetComboBox
            // 
            resources.ApplyResources(this.fsuipcPresetComboBox, "fsuipcPresetComboBox");
            this.fsuipcPresetComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.fsuipcPresetComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.fsuipcPresetComboBox.FormattingEnabled = true;
            this.fsuipcPresetComboBox.Name = "fsuipcPresetComboBox";
            this.fsuipcPresetComboBox.SelectedIndexChanged += new System.EventHandler(this.fsuipcPresetComboBox_SelectedIndexChanged);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // JeehellInputPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fsuipcLoadPresetGroupBox);
            this.Name = "JeehellInputPanel";
            this.Load += new System.EventHandler(this.JeehellInputPanel_Load);
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
        private System.Windows.Forms.Label label5;
    }
}
