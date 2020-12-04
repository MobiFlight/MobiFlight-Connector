namespace MobiFlight.UI.Panels
{
    partial class DisplayPinPanel
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.displayPortComboBox = new System.Windows.Forms.ComboBox();
            this.displayPinComboBox = new System.Windows.Forms.ComboBox();
            this.displayPinComoBoxLabel = new System.Windows.Forms.Label();
            this.displayPinBrightnessPanel = new System.Windows.Forms.Panel();
            this.displayPinBrightnessLabelPanel = new System.Windows.Forms.Panel();
            this.displayPinBrightnessDimLabel = new System.Windows.Forms.Label();
            this.displayPinBrightnessMediumLabel = new System.Windows.Forms.Label();
            this.displayPinBrightnessBrightLabel = new System.Windows.Forms.Label();
            this.displayPinBrightnessTrackBar = new System.Windows.Forms.TrackBar();
            this.panel2 = new System.Windows.Forms.Panel();
            this.displayPinBrightnessLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.displayPinBrightnessPanel.SuspendLayout();
            this.displayPinBrightnessLabelPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displayPinBrightnessTrackBar)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Controls.Add(this.displayPinComoBoxLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(263, 30);
            this.panel1.TabIndex = 5;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.displayPortComboBox);
            this.flowLayoutPanel1.Controls.Add(this.displayPinComboBox);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(90, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(167, 26);
            this.flowLayoutPanel1.TabIndex = 7;
            // 
            // displayPortComboBox
            // 
            this.displayPortComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.displayPortComboBox.FormattingEnabled = true;
            this.displayPortComboBox.Items.AddRange(new object[] {
            "Pin",
            "7-Segment",
            "3BCD-8Bit-with-Strobe"});
            this.displayPortComboBox.Location = new System.Drawing.Point(3, 3);
            this.displayPortComboBox.MaximumSize = new System.Drawing.Size(122, 0);
            this.displayPortComboBox.MinimumSize = new System.Drawing.Size(47, 0);
            this.displayPortComboBox.Name = "displayPortComboBox";
            this.displayPortComboBox.Size = new System.Drawing.Size(47, 21);
            this.displayPortComboBox.TabIndex = 5;
            // 
            // displayPinComboBox
            // 
            this.displayPinComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.displayPinComboBox.FormattingEnabled = true;
            this.displayPinComboBox.Items.AddRange(new object[] {
            "Pin",
            "7-Segment",
            "3BCD-8Bit-with-Strobe"});
            this.displayPinComboBox.Location = new System.Drawing.Point(56, 3);
            this.displayPinComboBox.MaximumSize = new System.Drawing.Size(122, 0);
            this.displayPinComboBox.MinimumSize = new System.Drawing.Size(47, 0);
            this.displayPinComboBox.Name = "displayPinComboBox";
            this.displayPinComboBox.Size = new System.Drawing.Size(47, 21);
            this.displayPinComboBox.TabIndex = 6;
            // 
            // displayPinComoBoxLabel
            // 
            this.displayPinComoBoxLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.displayPinComoBoxLabel.Location = new System.Drawing.Point(3, 7);
            this.displayPinComoBoxLabel.Name = "displayPinComoBoxLabel";
            this.displayPinComoBoxLabel.Size = new System.Drawing.Size(82, 13);
            this.displayPinComoBoxLabel.TabIndex = 0;
            this.displayPinComoBoxLabel.Text = "Pin";
            this.displayPinComoBoxLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // displayPinBrightnessPanel
            // 
            this.displayPinBrightnessPanel.Controls.Add(this.displayPinBrightnessLabelPanel);
            this.displayPinBrightnessPanel.Controls.Add(this.panel2);
            this.displayPinBrightnessPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayPinBrightnessPanel.Location = new System.Drawing.Point(0, 30);
            this.displayPinBrightnessPanel.Name = "displayPinBrightnessPanel";
            this.displayPinBrightnessPanel.Size = new System.Drawing.Size(263, 69);
            this.displayPinBrightnessPanel.TabIndex = 15;
            // 
            // displayPinBrightnessLabelPanel
            // 
            this.displayPinBrightnessLabelPanel.Controls.Add(this.displayPinBrightnessDimLabel);
            this.displayPinBrightnessLabelPanel.Controls.Add(this.displayPinBrightnessMediumLabel);
            this.displayPinBrightnessLabelPanel.Controls.Add(this.displayPinBrightnessBrightLabel);
            this.displayPinBrightnessLabelPanel.Controls.Add(this.displayPinBrightnessTrackBar);
            this.displayPinBrightnessLabelPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayPinBrightnessLabelPanel.Location = new System.Drawing.Point(90, 0);
            this.displayPinBrightnessLabelPanel.Name = "displayPinBrightnessLabelPanel";
            this.displayPinBrightnessLabelPanel.Size = new System.Drawing.Size(173, 69);
            this.displayPinBrightnessLabelPanel.TabIndex = 17;
            // 
            // displayPinBrightnessDimLabel
            // 
            this.displayPinBrightnessDimLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayPinBrightnessDimLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.displayPinBrightnessDimLabel.Location = new System.Drawing.Point(27, 45);
            this.displayPinBrightnessDimLabel.Name = "displayPinBrightnessDimLabel";
            this.displayPinBrightnessDimLabel.Size = new System.Drawing.Size(117, 24);
            this.displayPinBrightnessDimLabel.TabIndex = 15;
            this.displayPinBrightnessDimLabel.Text = "Medium";
            this.displayPinBrightnessDimLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // displayPinBrightnessMediumLabel
            // 
            this.displayPinBrightnessMediumLabel.AutoSize = true;
            this.displayPinBrightnessMediumLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.displayPinBrightnessMediumLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.displayPinBrightnessMediumLabel.Location = new System.Drawing.Point(144, 45);
            this.displayPinBrightnessMediumLabel.Name = "displayPinBrightnessMediumLabel";
            this.displayPinBrightnessMediumLabel.Size = new System.Drawing.Size(29, 13);
            this.displayPinBrightnessMediumLabel.TabIndex = 16;
            this.displayPinBrightnessMediumLabel.Text = "High";
            this.displayPinBrightnessMediumLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // displayPinBrightnessBrightLabel
            // 
            this.displayPinBrightnessBrightLabel.AutoSize = true;
            this.displayPinBrightnessBrightLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.displayPinBrightnessBrightLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.displayPinBrightnessBrightLabel.Location = new System.Drawing.Point(0, 45);
            this.displayPinBrightnessBrightLabel.Name = "displayPinBrightnessBrightLabel";
            this.displayPinBrightnessBrightLabel.Size = new System.Drawing.Size(27, 13);
            this.displayPinBrightnessBrightLabel.TabIndex = 14;
            this.displayPinBrightnessBrightLabel.Text = "Low";
            // 
            // displayPinBrightnessTrackBar
            // 
            this.displayPinBrightnessTrackBar.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.displayPinBrightnessTrackBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.displayPinBrightnessTrackBar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.displayPinBrightnessTrackBar.Location = new System.Drawing.Point(0, 0);
            this.displayPinBrightnessTrackBar.Maximum = 9;
            this.displayPinBrightnessTrackBar.Minimum = 1;
            this.displayPinBrightnessTrackBar.Name = "displayPinBrightnessTrackBar";
            this.displayPinBrightnessTrackBar.Size = new System.Drawing.Size(173, 45);
            this.displayPinBrightnessTrackBar.TabIndex = 16;
            this.displayPinBrightnessTrackBar.Value = 9;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.displayPinBrightnessLabel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(90, 69);
            this.panel2.TabIndex = 7;
            // 
            // displayPinBrightnessLabel
            // 
            this.displayPinBrightnessLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.displayPinBrightnessLabel.Location = new System.Drawing.Point(31, 3);
            this.displayPinBrightnessLabel.Name = "displayPinBrightnessLabel";
            this.displayPinBrightnessLabel.Size = new System.Drawing.Size(60, 24);
            this.displayPinBrightnessLabel.TabIndex = 15;
            this.displayPinBrightnessLabel.Text = "Brightness";
            this.displayPinBrightnessLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // DisplayPinPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.displayPinBrightnessPanel);
            this.Controls.Add(this.panel1);
            this.Name = "DisplayPinPanel";
            this.Size = new System.Drawing.Size(263, 99);
            this.panel1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.displayPinBrightnessPanel.ResumeLayout(false);
            this.displayPinBrightnessLabelPanel.ResumeLayout(false);
            this.displayPinBrightnessLabelPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displayPinBrightnessTrackBar)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.ComboBox displayPinComboBox;
        public System.Windows.Forms.ComboBox displayPortComboBox;
        private System.Windows.Forms.Label displayPinComoBoxLabel;
        public System.Windows.Forms.Panel displayPinBrightnessPanel;
        public System.Windows.Forms.TrackBar displayPinBrightnessTrackBar;
        private System.Windows.Forms.Panel displayPinBrightnessLabelPanel;
        private System.Windows.Forms.Label displayPinBrightnessDimLabel;
        private System.Windows.Forms.Label displayPinBrightnessMediumLabel;
        private System.Windows.Forms.Label displayPinBrightnessBrightLabel;
        private System.Windows.Forms.Label displayPinBrightnessLabel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}
