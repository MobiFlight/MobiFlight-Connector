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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisplayPinPanel));
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.pinSelectPanel = new MobiFlight.UI.Panels.PinSelectPanel();
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
            resources.GetString("displayPortComboBox.Items"),
            resources.GetString("displayPortComboBox.Items1"),
            resources.GetString("displayPortComboBox.Items2")});
            resources.ApplyResources(this.displayPortComboBox, "displayPortComboBox");
            this.displayPortComboBox.Name = "displayPortComboBox";
            // 
            // displayPinBrightnessPanel
            // 
            this.displayPinBrightnessPanel.Controls.Add(this.displayPinBrightnessLabelPanel);
            this.displayPinBrightnessPanel.Controls.Add(this.panel2);
            resources.ApplyResources(this.displayPinBrightnessPanel, "displayPinBrightnessPanel");
            this.displayPinBrightnessPanel.Name = "displayPinBrightnessPanel";
            // 
            // displayPinBrightnessLabelPanel
            // 
            this.displayPinBrightnessLabelPanel.Controls.Add(this.displayPinBrightnessDimLabel);
            this.displayPinBrightnessLabelPanel.Controls.Add(this.displayPinBrightnessMediumLabel);
            this.displayPinBrightnessLabelPanel.Controls.Add(this.displayPinBrightnessBrightLabel);
            this.displayPinBrightnessLabelPanel.Controls.Add(this.displayPinBrightnessTrackBar);
            resources.ApplyResources(this.displayPinBrightnessLabelPanel, "displayPinBrightnessLabelPanel");
            this.displayPinBrightnessLabelPanel.Name = "displayPinBrightnessLabelPanel";
            // 
            // displayPinBrightnessDimLabel
            // 
            resources.ApplyResources(this.displayPinBrightnessDimLabel, "displayPinBrightnessDimLabel");
            this.displayPinBrightnessDimLabel.Name = "displayPinBrightnessDimLabel";
            // 
            // displayPinBrightnessMediumLabel
            // 
            resources.ApplyResources(this.displayPinBrightnessMediumLabel, "displayPinBrightnessMediumLabel");
            this.displayPinBrightnessMediumLabel.Name = "displayPinBrightnessMediumLabel";
            // 
            // displayPinBrightnessBrightLabel
            // 
            resources.ApplyResources(this.displayPinBrightnessBrightLabel, "displayPinBrightnessBrightLabel");
            this.displayPinBrightnessBrightLabel.Name = "displayPinBrightnessBrightLabel";
            // 
            // displayPinBrightnessTrackBar
            // 
            this.displayPinBrightnessTrackBar.BackColor = System.Drawing.SystemColors.ControlLightLight;
            resources.ApplyResources(this.displayPinBrightnessTrackBar, "displayPinBrightnessTrackBar");
            this.displayPinBrightnessTrackBar.Maximum = 9;
            this.displayPinBrightnessTrackBar.Minimum = 1;
            this.displayPinBrightnessTrackBar.Name = "displayPinBrightnessTrackBar";
            this.displayPinBrightnessTrackBar.Value = 9;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.displayPortComboBox);
            this.panel2.Controls.Add(this.displayPinBrightnessLabel);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // displayPinBrightnessLabel
            // 
            resources.ApplyResources(this.displayPinBrightnessLabel, "displayPinBrightnessLabel");
            this.displayPinBrightnessLabel.Name = "displayPinBrightnessLabel";
            // 
            // pwmPinPanel
            // 
            this.pwmPinPanel.Controls.Add(this.displayPwmCheckBox);
            this.pwmPinPanel.Controls.Add(this.label1);
            resources.ApplyResources(this.pwmPinPanel, "pwmPinPanel");
            this.pwmPinPanel.Name = "pwmPinPanel";
            // 
            // displayPwmCheckBox
            // 
            resources.ApplyResources(this.displayPwmCheckBox, "displayPwmCheckBox");
            this.displayPwmCheckBox.Name = "displayPwmCheckBox";
            this.displayPwmCheckBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.pinSelectPanel);
            this.panel1.Name = "panel1";
            // 
            // pinSelectPanel
            // 
            resources.ApplyResources(this.pinSelectPanel, "pinSelectPanel");
            this.pinSelectPanel.Name = "pinSelectPanel";
            // 
            // DisplayPinPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.displayPinBrightnessPanel);
            this.Controls.Add(this.pwmPinPanel);
            this.Controls.Add(this.panel1);
            this.Name = "DisplayPinPanel";
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
