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
            this.displayPortComboBox = new System.Windows.Forms.ComboBox();
            this.displayPinBrightnessPanel = new System.Windows.Forms.Panel();
            this.displayPinBrightnessLabelPanel = new System.Windows.Forms.Panel();
            this.displayPinBrightnessDimLabel = new System.Windows.Forms.Label();
            this.displayPinBrightnessMediumLabel = new System.Windows.Forms.Label();
            this.displayPinBrightnessBrightLabel = new System.Windows.Forms.Label();
            this.displayPinBrightnessTrackBar = new System.Windows.Forms.TrackBar();
            this.panel2 = new System.Windows.Forms.Panel();
            this.displayPinBrightnessLabel = new System.Windows.Forms.Label();
            this.pwmPinPanel = new System.Windows.Forms.Panel();
            this.displayPwmCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pinSelectPanel = new MobiFlight.UI.Panels.PinSelectPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.displayPinBrightnessPanel.SuspendLayout();
            this.displayPinBrightnessLabelPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displayPinBrightnessTrackBar)).BeginInit();
            this.panel2.SuspendLayout();
            this.pwmPinPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // displayPortComboBox
            // 
            this.displayPortComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.displayPortComboBox.FormattingEnabled = true;
            this.displayPortComboBox.Items.AddRange(new object[] {
            "Pin",
            "7-Segment",
            "3BCD-8Bit-with-Strobe"});
            this.displayPortComboBox.Location = new System.Drawing.Point(24, 32);
            this.displayPortComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.displayPortComboBox.MaximumSize = new System.Drawing.Size(161, 0);
            this.displayPortComboBox.MinimumSize = new System.Drawing.Size(61, 0);
            this.displayPortComboBox.Name = "displayPortComboBox";
            this.displayPortComboBox.Size = new System.Drawing.Size(61, 24);
            this.displayPortComboBox.TabIndex = 5;
            // 
            // displayPinBrightnessPanel
            // 
            this.displayPinBrightnessPanel.Controls.Add(this.displayPinBrightnessLabelPanel);
            this.displayPinBrightnessPanel.Controls.Add(this.panel2);
            this.displayPinBrightnessPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayPinBrightnessPanel.Location = new System.Drawing.Point(0, 243);
            this.displayPinBrightnessPanel.Margin = new System.Windows.Forms.Padding(4);
            this.displayPinBrightnessPanel.Name = "displayPinBrightnessPanel";
            this.displayPinBrightnessPanel.Size = new System.Drawing.Size(351, 79);
            this.displayPinBrightnessPanel.TabIndex = 15;
            // 
            // displayPinBrightnessLabelPanel
            // 
            this.displayPinBrightnessLabelPanel.Controls.Add(this.displayPinBrightnessDimLabel);
            this.displayPinBrightnessLabelPanel.Controls.Add(this.displayPinBrightnessMediumLabel);
            this.displayPinBrightnessLabelPanel.Controls.Add(this.displayPinBrightnessBrightLabel);
            this.displayPinBrightnessLabelPanel.Controls.Add(this.displayPinBrightnessTrackBar);
            this.displayPinBrightnessLabelPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayPinBrightnessLabelPanel.Location = new System.Drawing.Point(120, 0);
            this.displayPinBrightnessLabelPanel.Margin = new System.Windows.Forms.Padding(4);
            this.displayPinBrightnessLabelPanel.Name = "displayPinBrightnessLabelPanel";
            this.displayPinBrightnessLabelPanel.Size = new System.Drawing.Size(231, 79);
            this.displayPinBrightnessLabelPanel.TabIndex = 17;
            // 
            // displayPinBrightnessDimLabel
            // 
            this.displayPinBrightnessDimLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayPinBrightnessDimLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.displayPinBrightnessDimLabel.Location = new System.Drawing.Point(33, 56);
            this.displayPinBrightnessDimLabel.Name = "displayPinBrightnessDimLabel";
            this.displayPinBrightnessDimLabel.Size = new System.Drawing.Size(161, 23);
            this.displayPinBrightnessDimLabel.TabIndex = 15;
            this.displayPinBrightnessDimLabel.Text = "Medium";
            this.displayPinBrightnessDimLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // displayPinBrightnessMediumLabel
            // 
            this.displayPinBrightnessMediumLabel.AutoSize = true;
            this.displayPinBrightnessMediumLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.displayPinBrightnessMediumLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.displayPinBrightnessMediumLabel.Location = new System.Drawing.Point(194, 56);
            this.displayPinBrightnessMediumLabel.Name = "displayPinBrightnessMediumLabel";
            this.displayPinBrightnessMediumLabel.Size = new System.Drawing.Size(37, 17);
            this.displayPinBrightnessMediumLabel.TabIndex = 16;
            this.displayPinBrightnessMediumLabel.Text = "High";
            this.displayPinBrightnessMediumLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // displayPinBrightnessBrightLabel
            // 
            this.displayPinBrightnessBrightLabel.AutoSize = true;
            this.displayPinBrightnessBrightLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.displayPinBrightnessBrightLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.displayPinBrightnessBrightLabel.Location = new System.Drawing.Point(0, 56);
            this.displayPinBrightnessBrightLabel.Name = "displayPinBrightnessBrightLabel";
            this.displayPinBrightnessBrightLabel.Size = new System.Drawing.Size(33, 17);
            this.displayPinBrightnessBrightLabel.TabIndex = 14;
            this.displayPinBrightnessBrightLabel.Text = "Low";
            // 
            // displayPinBrightnessTrackBar
            // 
            this.displayPinBrightnessTrackBar.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.displayPinBrightnessTrackBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.displayPinBrightnessTrackBar.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.displayPinBrightnessTrackBar.Location = new System.Drawing.Point(0, 0);
            this.displayPinBrightnessTrackBar.Margin = new System.Windows.Forms.Padding(4);
            this.displayPinBrightnessTrackBar.Maximum = 9;
            this.displayPinBrightnessTrackBar.Minimum = 1;
            this.displayPinBrightnessTrackBar.Name = "displayPinBrightnessTrackBar";
            this.displayPinBrightnessTrackBar.Size = new System.Drawing.Size(231, 56);
            this.displayPinBrightnessTrackBar.TabIndex = 16;
            this.displayPinBrightnessTrackBar.Value = 9;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.displayPortComboBox);
            this.panel2.Controls.Add(this.displayPinBrightnessLabel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(120, 79);
            this.panel2.TabIndex = 7;
            // 
            // displayPinBrightnessLabel
            // 
            this.displayPinBrightnessLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.displayPinBrightnessLabel.Location = new System.Drawing.Point(41, 4);
            this.displayPinBrightnessLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.displayPinBrightnessLabel.Name = "displayPinBrightnessLabel";
            this.displayPinBrightnessLabel.Size = new System.Drawing.Size(80, 30);
            this.displayPinBrightnessLabel.TabIndex = 15;
            this.displayPinBrightnessLabel.Text = "Brightness";
            this.displayPinBrightnessLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // pwmPinPanel
            // 
            this.pwmPinPanel.Controls.Add(this.displayPwmCheckBox);
            this.pwmPinPanel.Controls.Add(this.label1);
            this.pwmPinPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.pwmPinPanel.Location = new System.Drawing.Point(0, 203);
            this.pwmPinPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pwmPinPanel.MinimumSize = new System.Drawing.Size(0, 40);
            this.pwmPinPanel.Name = "pwmPinPanel";
            this.pwmPinPanel.Size = new System.Drawing.Size(351, 40);
            this.pwmPinPanel.TabIndex = 16;
            // 
            // displayPwmCheckBox
            // 
            this.displayPwmCheckBox.Location = new System.Drawing.Point(124, 2);
            this.displayPwmCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.displayPwmCheckBox.Name = "displayPwmCheckBox";
            this.displayPwmCheckBox.Size = new System.Drawing.Size(188, 27);
            this.displayPwmCheckBox.TabIndex = 9;
            this.displayPwmCheckBox.Text = "Enabled (Values 0-255)";
            this.displayPwmCheckBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(8, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 16);
            this.label1.TabIndex = 8;
            this.label1.Text = "PWM Mode";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // pinSelectPanel
            // 
            this.pinSelectPanel.AutoScroll = true;
            this.pinSelectPanel.AutoSize = true;
            this.pinSelectPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pinSelectPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pinSelectPanel.Location = new System.Drawing.Point(0, 0);
            this.pinSelectPanel.Margin = new System.Windows.Forms.Padding(4);
            this.pinSelectPanel.Name = "pinSelectPanel";
            this.pinSelectPanel.Size = new System.Drawing.Size(351, 203);
            this.pinSelectPanel.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.pinSelectPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(351, 203);
            this.panel1.TabIndex = 5;
            // 
            // DisplayPinPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.displayPinBrightnessPanel);
            this.Controls.Add(this.pwmPinPanel);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "DisplayPinPanel";
            this.Size = new System.Drawing.Size(351, 322);
            this.displayPinBrightnessPanel.ResumeLayout(false);
            this.displayPinBrightnessLabelPanel.ResumeLayout(false);
            this.displayPinBrightnessLabelPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displayPinBrightnessTrackBar)).EndInit();
            this.panel2.ResumeLayout(false);
            this.pwmPinPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.ComboBox displayPortComboBox;
        public System.Windows.Forms.Panel displayPinBrightnessPanel;
        private System.Windows.Forms.Label displayPinBrightnessLabel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel pwmPinPanel;
        private System.Windows.Forms.CheckBox displayPwmCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel displayPinBrightnessLabelPanel;
        private System.Windows.Forms.Label displayPinBrightnessDimLabel;
        private System.Windows.Forms.Label displayPinBrightnessMediumLabel;
        private System.Windows.Forms.Label displayPinBrightnessBrightLabel;
        public System.Windows.Forms.TrackBar displayPinBrightnessTrackBar;
        private PinSelectPanel pinSelectPanel;
        private System.Windows.Forms.Panel panel1;
    }
}
