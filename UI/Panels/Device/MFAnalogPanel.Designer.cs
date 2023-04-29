namespace MobiFlight.UI.Panels.Settings.Device
{
    partial class MFAnalogPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MFAnalogPanel));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mfPinLabel = new System.Windows.Forms.Label();
            this.mfPinComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.SensitivityValueLabel = new System.Windows.Forms.Label();
            this.SensitivityLowLabel1 = new System.Windows.Forms.Label();
            this.SensitivityHighLabel = new System.Windows.Forms.Label();
            this.SensitivityTrackBar = new System.Windows.Forms.TrackBar();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SensitivityTrackBar)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.mfPinLabel);
            this.groupBox1.Controls.Add(this.mfPinComboBox);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // mfPinLabel
            // 
            resources.ApplyResources(this.mfPinLabel, "mfPinLabel");
            this.mfPinLabel.Name = "mfPinLabel";
            // 
            // mfPinComboBox
            // 
            this.mfPinComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mfPinComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.mfPinComboBox, "mfPinComboBox");
            this.mfPinComboBox.Name = "mfPinComboBox";
            this.mfPinComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.SensitivityValueLabel);
            this.groupBox2.Controls.Add(this.SensitivityLowLabel1);
            this.groupBox2.Controls.Add(this.SensitivityHighLabel);
            this.groupBox2.Controls.Add(this.SensitivityTrackBar);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // SensitivityValueLabel
            // 
            resources.ApplyResources(this.SensitivityValueLabel, "SensitivityValueLabel");
            this.SensitivityValueLabel.Name = "SensitivityValueLabel";
            // 
            // SensitivityLowLabel1
            // 
            resources.ApplyResources(this.SensitivityLowLabel1, "SensitivityLowLabel1");
            this.SensitivityLowLabel1.Name = "SensitivityLowLabel1";
            // 
            // SensitivityHighLabel
            // 
            resources.ApplyResources(this.SensitivityHighLabel, "SensitivityHighLabel");
            this.SensitivityHighLabel.Name = "SensitivityHighLabel";
            // 
            // SensitivityTrackBar
            // 
            resources.ApplyResources(this.SensitivityTrackBar, "SensitivityTrackBar");
            this.SensitivityTrackBar.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.SensitivityTrackBar.Maximum = 20;
            this.SensitivityTrackBar.Minimum = 1;
            this.SensitivityTrackBar.Name = "SensitivityTrackBar";
            this.SensitivityTrackBar.TickFrequency = 5;
            this.SensitivityTrackBar.Value = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBox1);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            this.textBox1.TextChanged += new System.EventHandler(this.value_Changed);
            // 
            // MFAnalogPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MFAnalogPanel";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SensitivityTrackBar)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label mfPinLabel;
        private System.Windows.Forms.ComboBox mfPinComboBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TrackBar SensitivityTrackBar;
        private System.Windows.Forms.Label SensitivityLowLabel1;
        private System.Windows.Forms.Label SensitivityHighLabel;
        private System.Windows.Forms.Label SensitivityValueLabel;
    }
}
