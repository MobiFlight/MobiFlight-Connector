namespace MobiFlight.Panels
{
    partial class EventIdInputPanel
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventIdInputPanel));
            this.fsuipcPresetUseButton = new System.Windows.Forms.Button();
            this.fsuipcPresetComboBox = new System.Windows.Forms.ComboBox();
            this.fsuipcLoadPresetGroupBox = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.testButton = new System.Windows.Forms.Button();
            this.paramTextBox = new System.Windows.Forms.TextBox();
            this.eventIdTextBox = new System.Windows.Forms.TextBox();
            this.EventIdLabel = new System.Windows.Forms.Label();
            this.paramLabel = new System.Windows.Forms.Label();
            this.fsuipcLoadPresetGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // fsuipcPresetUseButton
            // 
            resources.ApplyResources(this.fsuipcPresetUseButton, "fsuipcPresetUseButton");
            this.fsuipcPresetUseButton.Name = "fsuipcPresetUseButton";
            this.fsuipcPresetUseButton.UseVisualStyleBackColor = true;
            // 
            // fsuipcPresetComboBox
            // 
            resources.ApplyResources(this.fsuipcPresetComboBox, "fsuipcPresetComboBox");
            this.fsuipcPresetComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.fsuipcPresetComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.fsuipcPresetComboBox.FormattingEnabled = true;
            this.fsuipcPresetComboBox.Name = "fsuipcPresetComboBox";
            // 
            // fsuipcLoadPresetGroupBox
            // 
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.fsuipcPresetUseButton);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.fsuipcPresetComboBox);
            resources.ApplyResources(this.fsuipcLoadPresetGroupBox, "fsuipcLoadPresetGroupBox");
            this.fsuipcLoadPresetGroupBox.Name = "fsuipcLoadPresetGroupBox";
            this.fsuipcLoadPresetGroupBox.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.testButton);
            this.groupBox1.Controls.Add(this.paramTextBox);
            this.groupBox1.Controls.Add(this.eventIdTextBox);
            this.groupBox1.Controls.Add(this.EventIdLabel);
            this.groupBox1.Controls.Add(this.paramLabel);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // testButton
            // 
            resources.ApplyResources(this.testButton, "testButton");
            this.testButton.Name = "testButton";
            this.testButton.UseVisualStyleBackColor = true;
            this.testButton.Click += new System.EventHandler(this.testButton_Click);
            // 
            // paramTextBox
            // 
            resources.ApplyResources(this.paramTextBox, "paramTextBox");
            this.paramTextBox.Name = "paramTextBox";
            // 
            // eventIdTextBox
            // 
            resources.ApplyResources(this.eventIdTextBox, "eventIdTextBox");
            this.eventIdTextBox.Name = "eventIdTextBox";
            // 
            // EventIdLabel
            // 
            resources.ApplyResources(this.EventIdLabel, "EventIdLabel");
            this.EventIdLabel.Name = "EventIdLabel";
            // 
            // paramLabel
            // 
            resources.ApplyResources(this.paramLabel, "paramLabel");
            this.paramLabel.Name = "paramLabel";
            // 
            // EventIdInputPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.fsuipcLoadPresetGroupBox);
            this.Name = "EventIdInputPanel";
            this.fsuipcLoadPresetGroupBox.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button fsuipcPresetUseButton;
        private System.Windows.Forms.ComboBox fsuipcPresetComboBox;
        private System.Windows.Forms.GroupBox fsuipcLoadPresetGroupBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button testButton;
        private System.Windows.Forms.TextBox paramTextBox;
        private System.Windows.Forms.TextBox eventIdTextBox;
        private System.Windows.Forms.Label EventIdLabel;
        private System.Windows.Forms.Label paramLabel;
    }
}
