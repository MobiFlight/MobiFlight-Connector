namespace MobiFlight.UI.Panels.Settings.Device
{
    partial class MFLedSegmentPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MFLedSegmentPanel));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mfNumModulesComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numberOfModulesLabel = new System.Windows.Forms.Label();
            this.mfPin1Label = new System.Windows.Forms.Label();
            this.mfPin1ComboBox = new System.Windows.Forms.ComboBox();
            this.mfPin3Label = new System.Windows.Forms.Label();
            this.mfPin3ComboBox = new System.Windows.Forms.ComboBox();
            this.mfPin2Label = new System.Windows.Forms.Label();
            this.mfPin2ComboBox = new System.Windows.Forms.ComboBox();
            this.mfIntensityGroupBox = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.mfIntensityTrackBar = new System.Windows.Forms.TrackBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.mfDisplayTypeComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.mfIntensityGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mfIntensityTrackBar)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.mfNumModulesComboBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numberOfModulesLabel);
            this.groupBox1.Controls.Add(this.mfPin1Label);
            this.groupBox1.Controls.Add(this.mfPin1ComboBox);
            this.groupBox1.Controls.Add(this.mfPin3Label);
            this.groupBox1.Controls.Add(this.mfPin3ComboBox);
            this.groupBox1.Controls.Add(this.mfPin2Label);
            this.groupBox1.Controls.Add(this.mfPin2ComboBox);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // mfNumModulesComboBox
            // 
            this.mfNumModulesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mfNumModulesComboBox.FormattingEnabled = true;
            this.mfNumModulesComboBox.Items.AddRange(new object[] {
            resources.GetString("mfNumModulesComboBox.Items"),
            resources.GetString("mfNumModulesComboBox.Items1"),
            resources.GetString("mfNumModulesComboBox.Items2"),
            resources.GetString("mfNumModulesComboBox.Items3"),
            resources.GetString("mfNumModulesComboBox.Items4"),
            resources.GetString("mfNumModulesComboBox.Items5"),
            resources.GetString("mfNumModulesComboBox.Items6"),
            resources.GetString("mfNumModulesComboBox.Items7")});
            resources.ApplyResources(this.mfNumModulesComboBox, "mfNumModulesComboBox");
            this.mfNumModulesComboBox.Name = "mfNumModulesComboBox";
            this.mfNumModulesComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // numberOfModulesLabel
            // 
            resources.ApplyResources(this.numberOfModulesLabel, "numberOfModulesLabel");
            this.numberOfModulesLabel.Name = "numberOfModulesLabel";
            // 
            // mfPin1Label
            // 
            resources.ApplyResources(this.mfPin1Label, "mfPin1Label");
            this.mfPin1Label.Name = "mfPin1Label";
            // 
            // mfPin1ComboBox
            // 
            this.mfPin1ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mfPin1ComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.mfPin1ComboBox, "mfPin1ComboBox");
            this.mfPin1ComboBox.Name = "mfPin1ComboBox";
            this.mfPin1ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfPin3Label
            // 
            resources.ApplyResources(this.mfPin3Label, "mfPin3Label");
            this.mfPin3Label.Name = "mfPin3Label";
            // 
            // mfPin3ComboBox
            // 
            this.mfPin3ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mfPin3ComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.mfPin3ComboBox, "mfPin3ComboBox");
            this.mfPin3ComboBox.Name = "mfPin3ComboBox";
            this.mfPin3ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfPin2Label
            // 
            resources.ApplyResources(this.mfPin2Label, "mfPin2Label");
            this.mfPin2Label.Name = "mfPin2Label";
            // 
            // mfPin2ComboBox
            // 
            this.mfPin2ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mfPin2ComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.mfPin2ComboBox, "mfPin2ComboBox");
            this.mfPin2ComboBox.Name = "mfPin2ComboBox";
            this.mfPin2ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfIntensityGroupBox
            // 
            this.mfIntensityGroupBox.Controls.Add(this.label2);
            this.mfIntensityGroupBox.Controls.Add(this.label1);
            this.mfIntensityGroupBox.Controls.Add(this.mfIntensityTrackBar);
            resources.ApplyResources(this.mfIntensityGroupBox, "mfIntensityGroupBox");
            this.mfIntensityGroupBox.Name = "mfIntensityGroupBox";
            this.mfIntensityGroupBox.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // mfIntensityTrackBar
            // 
            resources.ApplyResources(this.mfIntensityTrackBar, "mfIntensityTrackBar");
            this.mfIntensityTrackBar.Maximum = 15;
            this.mfIntensityTrackBar.Minimum = 1;
            this.mfIntensityTrackBar.Name = "mfIntensityTrackBar";
            this.mfIntensityTrackBar.Value = 1;
            this.mfIntensityTrackBar.ValueChanged += new System.EventHandler(this.value_Changed);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox1);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            this.textBox1.TextChanged += new System.EventHandler(this.value_Changed);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.mfDisplayTypeComboBox);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // mfDisplayTypeComboBox
            // 
            this.mfDisplayTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mfDisplayTypeComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.mfDisplayTypeComboBox, "mfDisplayTypeComboBox");
            this.mfDisplayTypeComboBox.Name = "mfDisplayTypeComboBox";
            this.mfDisplayTypeComboBox.SelectedValueChanged += new System.EventHandler(this.mfDisplayTypeComboBox_SelectedValueChanged);
            // 
            // MFLedSegmentPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.mfIntensityGroupBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Name = "MFLedSegmentPanel";
            this.groupBox1.ResumeLayout(false);
            this.mfIntensityGroupBox.ResumeLayout(false);
            this.mfIntensityGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mfIntensityTrackBar)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox mfIntensityGroupBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label mfPin1Label;
        private System.Windows.Forms.ComboBox mfPin1ComboBox;
        private System.Windows.Forms.Label mfPin2Label;
        private System.Windows.Forms.ComboBox mfPin2ComboBox;
        private System.Windows.Forms.TrackBar mfIntensityTrackBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label mfPin3Label;
        private System.Windows.Forms.ComboBox mfPin3ComboBox;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label numberOfModulesLabel;
        private System.Windows.Forms.ComboBox mfNumModulesComboBox;
        private System.Windows.Forms.ComboBox mfDisplayTypeComboBox;
    }
}
