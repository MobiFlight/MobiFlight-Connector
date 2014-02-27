namespace ArcazeUSB.Panels
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
            this.displayPinBrightnessPanel = new System.Windows.Forms.Panel();
            this.displayPinBrightnessLabelPanel = new System.Windows.Forms.Panel();
            this.displayPinBrightnessDimLabel = new System.Windows.Forms.Label();
            this.displayPinBrightnessMediumLabel = new System.Windows.Forms.Label();
            this.displayPinBrightnessBrightLabel = new System.Windows.Forms.Label();
            this.displayPinBrightnessTrackBar = new System.Windows.Forms.TrackBar();
            this.displayPinBrightnessLabel = new System.Windows.Forms.Label();
            this.displayPinComboBox = new System.Windows.Forms.ComboBox();
            this.displayPortComboBox = new System.Windows.Forms.ComboBox();
            this.displayPinComoBoxLabel = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.displayPinBrightnessPanel.SuspendLayout();
            this.displayPinBrightnessLabelPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displayPinBrightnessTrackBar)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.displayPinComboBox);
            this.panel1.Controls.Add(this.displayPortComboBox);
            this.panel1.Controls.Add(this.displayPinComoBoxLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(248, 30);
            this.panel1.TabIndex = 5;
            // 
            // displayPinBrightnessPanel
            // 
            this.displayPinBrightnessPanel.Controls.Add(this.displayPinBrightnessLabelPanel);
            this.displayPinBrightnessPanel.Controls.Add(this.displayPinBrightnessTrackBar);
            this.displayPinBrightnessPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayPinBrightnessPanel.Location = new System.Drawing.Point(66, 30);
            this.displayPinBrightnessPanel.Name = "displayPinBrightnessPanel";
            this.displayPinBrightnessPanel.Size = new System.Drawing.Size(182, 66);
            this.displayPinBrightnessPanel.TabIndex = 15;
            // 
            // displayPinBrightnessLabelPanel
            // 
            this.displayPinBrightnessLabelPanel.Controls.Add(this.displayPinBrightnessDimLabel);
            this.displayPinBrightnessLabelPanel.Controls.Add(this.displayPinBrightnessMediumLabel);
            this.displayPinBrightnessLabelPanel.Controls.Add(this.displayPinBrightnessBrightLabel);
            this.displayPinBrightnessLabelPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.displayPinBrightnessLabelPanel.Location = new System.Drawing.Point(0, 45);
            this.displayPinBrightnessLabelPanel.Name = "displayPinBrightnessLabelPanel";
            this.displayPinBrightnessLabelPanel.Size = new System.Drawing.Size(182, 21);
            this.displayPinBrightnessLabelPanel.TabIndex = 17;
            // 
            // displayPinBrightnessDimLabel
            // 
            this.displayPinBrightnessDimLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.displayPinBrightnessDimLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.displayPinBrightnessDimLabel.Location = new System.Drawing.Point(27, 0);
            this.displayPinBrightnessDimLabel.Name = "displayPinBrightnessDimLabel";
            this.displayPinBrightnessDimLabel.Size = new System.Drawing.Size(126, 21);
            this.displayPinBrightnessDimLabel.TabIndex = 15;
            this.displayPinBrightnessDimLabel.Text = "Medium";
            this.displayPinBrightnessDimLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // displayPinBrightnessMediumLabel
            // 
            this.displayPinBrightnessMediumLabel.AutoSize = true;
            this.displayPinBrightnessMediumLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.displayPinBrightnessMediumLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.displayPinBrightnessMediumLabel.Location = new System.Drawing.Point(153, 0);
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
            this.displayPinBrightnessBrightLabel.Location = new System.Drawing.Point(0, 0);
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
            this.displayPinBrightnessTrackBar.Size = new System.Drawing.Size(182, 45);
            this.displayPinBrightnessTrackBar.TabIndex = 16;
            this.displayPinBrightnessTrackBar.Value = 9;
            // 
            // displayPinBrightnessLabel
            // 
            this.displayPinBrightnessLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.displayPinBrightnessLabel.Location = new System.Drawing.Point(3, 3);
            this.displayPinBrightnessLabel.Name = "displayPinBrightnessLabel";
            this.displayPinBrightnessLabel.Size = new System.Drawing.Size(60, 24);
            this.displayPinBrightnessLabel.TabIndex = 15;
            this.displayPinBrightnessLabel.Text = "Brightness";
            this.displayPinBrightnessLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // displayPinComboBox
            // 
            this.displayPinComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.displayPinComboBox.FormattingEnabled = true;
            this.displayPinComboBox.Items.AddRange(new object[] {
            "Pin",
            "7-Segment",
            "3BCD-8Bit-with-Strobe"});
            this.displayPinComboBox.Location = new System.Drawing.Point(119, 3);
            this.displayPinComboBox.Name = "displayPinComboBox";
            this.displayPinComboBox.Size = new System.Drawing.Size(47, 21);
            this.displayPinComboBox.TabIndex = 6;
            // 
            // displayPortComboBox
            // 
            this.displayPortComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.displayPortComboBox.FormattingEnabled = true;
            this.displayPortComboBox.Items.AddRange(new object[] {
            "Pin",
            "7-Segment",
            "3BCD-8Bit-with-Strobe"});
            this.displayPortComboBox.Location = new System.Drawing.Point(66, 3);
            this.displayPortComboBox.Name = "displayPortComboBox";
            this.displayPortComboBox.Size = new System.Drawing.Size(47, 21);
            this.displayPortComboBox.TabIndex = 5;
            // 
            // displayPinComoBoxLabel
            // 
            this.displayPinComoBoxLabel.AutoSize = true;
            this.displayPinComoBoxLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.displayPinComoBoxLabel.Location = new System.Drawing.Point(35, 7);
            this.displayPinComoBoxLabel.Name = "displayPinComoBoxLabel";
            this.displayPinComoBoxLabel.Size = new System.Drawing.Size(22, 13);
            this.displayPinComoBoxLabel.TabIndex = 0;
            this.displayPinComoBoxLabel.Text = "Pin";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.displayPinBrightnessLabel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 30);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(66, 66);
            this.panel2.TabIndex = 7;
            // 
            // DisplayPinPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.displayPinBrightnessPanel);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "DisplayPinPanel";
            this.Size = new System.Drawing.Size(248, 96);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.displayPinBrightnessPanel.ResumeLayout(false);
            this.displayPinBrightnessPanel.PerformLayout();
            this.displayPinBrightnessLabelPanel.ResumeLayout(false);
            this.displayPinBrightnessLabelPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displayPinBrightnessTrackBar)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox displayPinComboBox;
        private System.Windows.Forms.ComboBox displayPortComboBox;
        private System.Windows.Forms.Label displayPinComoBoxLabel;
        private System.Windows.Forms.Panel displayPinBrightnessPanel;
        private System.Windows.Forms.TrackBar displayPinBrightnessTrackBar;
        private System.Windows.Forms.Panel displayPinBrightnessLabelPanel;
        private System.Windows.Forms.Label displayPinBrightnessDimLabel;
        private System.Windows.Forms.Label displayPinBrightnessMediumLabel;
        private System.Windows.Forms.Label displayPinBrightnessBrightLabel;
        private System.Windows.Forms.Label displayPinBrightnessLabel;
        private System.Windows.Forms.Panel panel2;
    }
}
