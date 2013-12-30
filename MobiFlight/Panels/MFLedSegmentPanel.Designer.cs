namespace MobiFlight.MobiFlight.Panels
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
            this.clkPinLabel = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.csPinLabel = new System.Windows.Forms.Label();
            this.csPinComboBox = new System.Windows.Forms.ComboBox();
            this.mfDinPinLabel = new System.Windows.Forms.Label();
            this.dinPinComboBox = new System.Windows.Forms.ComboBox();
            this.mfIntensityGroupBox = new System.Windows.Forms.GroupBox();
            this.mfIntensityTrackBar = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.mfIntensityGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mfIntensityTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.clkPinLabel);
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.csPinLabel);
            this.groupBox1.Controls.Add(this.csPinComboBox);
            this.groupBox1.Controls.Add(this.mfDinPinLabel);
            this.groupBox1.Controls.Add(this.dinPinComboBox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(186, 104);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pin settings";
            // 
            // clkPinLabel
            // 
            this.clkPinLabel.Location = new System.Drawing.Point(75, 78);
            this.clkPinLabel.Name = "clkPinLabel";
            this.clkPinLabel.Size = new System.Drawing.Size(75, 18);
            this.clkPinLabel.TabIndex = 18;
            this.clkPinLabel.Text = "CLK line";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(18, 75);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(51, 21);
            this.comboBox1.TabIndex = 17;
            // 
            // csPinLabel
            // 
            this.csPinLabel.Location = new System.Drawing.Point(75, 50);
            this.csPinLabel.Name = "csPinLabel";
            this.csPinLabel.Size = new System.Drawing.Size(75, 18);
            this.csPinLabel.TabIndex = 16;
            this.csPinLabel.Text = "CS line";
            // 
            // csPinComboBox
            // 
            this.csPinComboBox.FormattingEnabled = true;
            this.csPinComboBox.Location = new System.Drawing.Point(18, 47);
            this.csPinComboBox.Name = "csPinComboBox";
            this.csPinComboBox.Size = new System.Drawing.Size(51, 21);
            this.csPinComboBox.TabIndex = 15;
            // 
            // mfDinPinLabel
            // 
            this.mfDinPinLabel.Location = new System.Drawing.Point(75, 22);
            this.mfDinPinLabel.Name = "mfDinPinLabel";
            this.mfDinPinLabel.Size = new System.Drawing.Size(75, 18);
            this.mfDinPinLabel.TabIndex = 14;
            this.mfDinPinLabel.Text = "DIN line";
            // 
            // dinPinComboBox
            // 
            this.dinPinComboBox.FormattingEnabled = true;
            this.dinPinComboBox.Location = new System.Drawing.Point(18, 19);
            this.dinPinComboBox.Name = "dinPinComboBox";
            this.dinPinComboBox.Size = new System.Drawing.Size(51, 21);
            this.dinPinComboBox.TabIndex = 13;
            // 
            // mfIntensityGroupBox
            // 
            this.mfIntensityGroupBox.Controls.Add(this.label2);
            this.mfIntensityGroupBox.Controls.Add(this.label1);
            this.mfIntensityGroupBox.Controls.Add(this.mfIntensityTrackBar);
            this.mfIntensityGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.mfIntensityGroupBox.Location = new System.Drawing.Point(0, 104);
            this.mfIntensityGroupBox.Name = "mfIntensityGroupBox";
            this.mfIntensityGroupBox.Size = new System.Drawing.Size(186, 66);
            this.mfIntensityGroupBox.TabIndex = 2;
            this.mfIntensityGroupBox.TabStop = false;
            this.mfIntensityGroupBox.Text = "Global intensity";
            // 
            // mfIntensityTrackBar
            // 
            this.mfIntensityTrackBar.Location = new System.Drawing.Point(3, 15);
            this.mfIntensityTrackBar.Maximum = 15;
            this.mfIntensityTrackBar.Minimum = 1;
            this.mfIntensityTrackBar.Name = "mfIntensityTrackBar";
            this.mfIntensityTrackBar.Size = new System.Drawing.Size(150, 45);
            this.mfIntensityTrackBar.TabIndex = 0;
            this.mfIntensityTrackBar.Value = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 18);
            this.label1.TabIndex = 19;
            this.label1.Text = "Dim";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(87, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 18);
            this.label2.TabIndex = 20;
            this.label2.Text = "Bright";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // MFLedSegmentPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mfIntensityGroupBox);
            this.Controls.Add(this.groupBox1);
            this.Name = "MFLedSegmentPanel";
            this.Size = new System.Drawing.Size(186, 241);
            this.groupBox1.ResumeLayout(false);
            this.mfIntensityGroupBox.ResumeLayout(false);
            this.mfIntensityGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mfIntensityTrackBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label clkPinLabel;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label csPinLabel;
        private System.Windows.Forms.ComboBox csPinComboBox;
        private System.Windows.Forms.Label mfDinPinLabel;
        private System.Windows.Forms.ComboBox dinPinComboBox;
        private System.Windows.Forms.GroupBox mfIntensityGroupBox;
        private System.Windows.Forms.TrackBar mfIntensityTrackBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;

    }
}
