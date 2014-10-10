namespace MobiFlight.Panels
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
            this.groupBox1.SuspendLayout();
            this.mfIntensityGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mfIntensityTrackBar)).BeginInit();
            this.groupBox2.SuspendLayout();
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
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(197, 68);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pin settings and number of modules";
            // 
            // mfNumModulesComboBox
            // 
            this.mfNumModulesComboBox.FormattingEnabled = true;
            this.mfNumModulesComboBox.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8"});
            this.mfNumModulesComboBox.Location = new System.Drawing.Point(153, 38);
            this.mfNumModulesComboBox.Name = "mfNumModulesComboBox";
            this.mfNumModulesComboBox.Size = new System.Drawing.Size(38, 21);
            this.mfNumModulesComboBox.TabIndex = 21;
            this.mfNumModulesComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(141, 41);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(11, 11);
            this.label3.TabIndex = 20;
            this.label3.Text = "|";
            // 
            // numberOfModulesLabel
            // 
            this.numberOfModulesLabel.Location = new System.Drawing.Point(156, 21);
            this.numberOfModulesLabel.Name = "numberOfModulesLabel";
            this.numberOfModulesLabel.Size = new System.Drawing.Size(32, 11);
            this.numberOfModulesLabel.TabIndex = 19;
            this.numberOfModulesLabel.Text = "Num";
            // 
            // mfPin1Label
            // 
            this.mfPin1Label.Location = new System.Drawing.Point(6, 21);
            this.mfPin1Label.Name = "mfPin1Label";
            this.mfPin1Label.Size = new System.Drawing.Size(32, 11);
            this.mfPin1Label.TabIndex = 16;
            this.mfPin1Label.Text = "DIN";
            // 
            // mfPin1ComboBox
            // 
            this.mfPin1ComboBox.FormattingEnabled = true;
            this.mfPin1ComboBox.Location = new System.Drawing.Point(9, 38);
            this.mfPin1ComboBox.Name = "mfPin1ComboBox";
            this.mfPin1ComboBox.Size = new System.Drawing.Size(38, 21);
            this.mfPin1ComboBox.TabIndex = 15;
            this.mfPin1ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfPin3Label
            // 
            this.mfPin3Label.Location = new System.Drawing.Point(94, 21);
            this.mfPin3Label.Name = "mfPin3Label";
            this.mfPin3Label.Size = new System.Drawing.Size(32, 11);
            this.mfPin3Label.TabIndex = 18;
            this.mfPin3Label.Text = "CLK";
            // 
            // mfPin3ComboBox
            // 
            this.mfPin3ComboBox.FormattingEnabled = true;
            this.mfPin3ComboBox.Location = new System.Drawing.Point(97, 38);
            this.mfPin3ComboBox.Name = "mfPin3ComboBox";
            this.mfPin3ComboBox.Size = new System.Drawing.Size(38, 21);
            this.mfPin3ComboBox.TabIndex = 17;
            this.mfPin3ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfPin2Label
            // 
            this.mfPin2Label.Location = new System.Drawing.Point(50, 21);
            this.mfPin2Label.Name = "mfPin2Label";
            this.mfPin2Label.Size = new System.Drawing.Size(41, 11);
            this.mfPin2Label.TabIndex = 16;
            this.mfPin2Label.Text = "CS";
            // 
            // mfPin2ComboBox
            // 
            this.mfPin2ComboBox.FormattingEnabled = true;
            this.mfPin2ComboBox.Location = new System.Drawing.Point(53, 38);
            this.mfPin2ComboBox.Name = "mfPin2ComboBox";
            this.mfPin2ComboBox.Size = new System.Drawing.Size(38, 21);
            this.mfPin2ComboBox.TabIndex = 15;
            this.mfPin2ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfIntensityGroupBox
            // 
            this.mfIntensityGroupBox.Controls.Add(this.label2);
            this.mfIntensityGroupBox.Controls.Add(this.label1);
            this.mfIntensityGroupBox.Controls.Add(this.mfIntensityTrackBar);
            this.mfIntensityGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.mfIntensityGroupBox.Location = new System.Drawing.Point(0, 68);
            this.mfIntensityGroupBox.Name = "mfIntensityGroupBox";
            this.mfIntensityGroupBox.Size = new System.Drawing.Size(197, 66);
            this.mfIntensityGroupBox.TabIndex = 2;
            this.mfIntensityGroupBox.TabStop = false;
            this.mfIntensityGroupBox.Text = "Global intensity";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(125, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 18);
            this.label2.TabIndex = 20;
            this.label2.Text = "Bright";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 18);
            this.label1.TabIndex = 19;
            this.label1.Text = "Dim";
            // 
            // mfIntensityTrackBar
            // 
            this.mfIntensityTrackBar.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.mfIntensityTrackBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mfIntensityTrackBar.Location = new System.Drawing.Point(3, 16);
            this.mfIntensityTrackBar.Maximum = 15;
            this.mfIntensityTrackBar.Minimum = 1;
            this.mfIntensityTrackBar.Name = "mfIntensityTrackBar";
            this.mfIntensityTrackBar.Size = new System.Drawing.Size(191, 47);
            this.mfIntensityTrackBar.TabIndex = 0;
            this.mfIntensityTrackBar.Value = 1;
            this.mfIntensityTrackBar.ValueChanged += new System.EventHandler(this.value_Changed);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 134);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(197, 48);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Name";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(18, 19);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(151, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.value_Changed);
            // 
            // MFLedSegmentPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.mfIntensityGroupBox);
            this.Controls.Add(this.groupBox1);
            this.Name = "MFLedSegmentPanel";
            this.Size = new System.Drawing.Size(197, 214);
            this.groupBox1.ResumeLayout(false);
            this.mfIntensityGroupBox.ResumeLayout(false);
            this.mfIntensityGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mfIntensityTrackBar)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label mfPin1Label;
        private System.Windows.Forms.ComboBox mfPin1ComboBox;
        private System.Windows.Forms.Label mfPin2Label;
        private System.Windows.Forms.ComboBox mfPin2ComboBox;
        private System.Windows.Forms.GroupBox mfIntensityGroupBox;
        private System.Windows.Forms.TrackBar mfIntensityTrackBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label mfPin3Label;
        private System.Windows.Forms.ComboBox mfPin3ComboBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label numberOfModulesLabel;
        private System.Windows.Forms.ComboBox mfNumModulesComboBox;

    }
}
