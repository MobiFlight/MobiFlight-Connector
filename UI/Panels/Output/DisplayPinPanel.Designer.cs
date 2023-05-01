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
            this.displayPinComboBox = new System.Windows.Forms.ComboBox();
            this.singlePinSelectFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.selectMultiplePinsCheckBox = new System.Windows.Forms.CheckBox();
            this.PinSelectContainer = new System.Windows.Forms.Panel();
            this.PinSelectPanel = new System.Windows.Forms.Panel();
            this.MultiSelectPinSelectContainer = new System.Windows.Forms.Panel();
            this.LabelPinSelectContainer = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.MultiPinSelectPanel = new MobiFlight.UI.Panels.PinSelectPanel();
            this.displayPinBrightnessPanel.SuspendLayout();
            this.displayPinBrightnessLabelPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displayPinBrightnessTrackBar)).BeginInit();
            this.panel2.SuspendLayout();
            this.pwmPinPanel.SuspendLayout();
            this.singlePinSelectFlowLayoutPanel.SuspendLayout();
            this.PinSelectContainer.SuspendLayout();
            this.PinSelectPanel.SuspendLayout();
            this.MultiSelectPinSelectContainer.SuspendLayout();
            this.LabelPinSelectContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // displayPortComboBox
            // 
            this.displayPortComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.displayPortComboBox.FormattingEnabled = true;
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
            // displayPinComboBox
            // 
            this.displayPinComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.displayPinComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.displayPinComboBox, "displayPinComboBox");
            this.displayPinComboBox.Name = "displayPinComboBox";
            // 
            // singlePinSelectFlowLayoutPanel
            // 
            this.singlePinSelectFlowLayoutPanel.Controls.Add(this.displayPortComboBox);
            this.singlePinSelectFlowLayoutPanel.Controls.Add(this.displayPinComboBox);
            resources.ApplyResources(this.singlePinSelectFlowLayoutPanel, "singlePinSelectFlowLayoutPanel");
            this.singlePinSelectFlowLayoutPanel.Name = "singlePinSelectFlowLayoutPanel";
            // 
            // selectMultiplePinsCheckBox
            // 
            resources.ApplyResources(this.selectMultiplePinsCheckBox, "selectMultiplePinsCheckBox");
            this.selectMultiplePinsCheckBox.Name = "selectMultiplePinsCheckBox";
            this.selectMultiplePinsCheckBox.UseVisualStyleBackColor = true;
            this.selectMultiplePinsCheckBox.CheckedChanged += new System.EventHandler(this.selectMultiplePinsCheckBox_CheckedChanged);
            // 
            // PinSelectContainer
            // 
            resources.ApplyResources(this.PinSelectContainer, "PinSelectContainer");
            this.PinSelectContainer.Controls.Add(this.PinSelectPanel);
            this.PinSelectContainer.Controls.Add(this.MultiSelectPinSelectContainer);
            this.PinSelectContainer.Controls.Add(this.LabelPinSelectContainer);
            this.PinSelectContainer.Name = "PinSelectContainer";
            // 
            // PinSelectPanel
            // 
            resources.ApplyResources(this.PinSelectPanel, "PinSelectPanel");
            this.PinSelectPanel.Controls.Add(this.MultiPinSelectPanel);
            this.PinSelectPanel.Controls.Add(this.singlePinSelectFlowLayoutPanel);
            this.PinSelectPanel.Name = "PinSelectPanel";
            // 
            // MultiSelectPinSelectContainer
            // 
            resources.ApplyResources(this.MultiSelectPinSelectContainer, "MultiSelectPinSelectContainer");
            this.MultiSelectPinSelectContainer.Controls.Add(this.selectMultiplePinsCheckBox);
            this.MultiSelectPinSelectContainer.Name = "MultiSelectPinSelectContainer";
            // 
            // LabelPinSelectContainer
            // 
            this.LabelPinSelectContainer.Controls.Add(this.label3);
            resources.ApplyResources(this.LabelPinSelectContainer, "LabelPinSelectContainer");
            this.LabelPinSelectContainer.Name = "LabelPinSelectContainer";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // MultiPinSelectPanel
            // 
            resources.ApplyResources(this.MultiPinSelectPanel, "MultiPinSelectPanel");
            this.MultiPinSelectPanel.Name = "MultiPinSelectPanel";
            // 
            // DisplayPinPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.displayPinBrightnessPanel);
            this.Controls.Add(this.pwmPinPanel);
            this.Controls.Add(this.PinSelectContainer);
            this.DoubleBuffered = true;
            this.Name = "DisplayPinPanel";
            this.displayPinBrightnessPanel.ResumeLayout(false);
            this.displayPinBrightnessLabelPanel.ResumeLayout(false);
            this.displayPinBrightnessLabelPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displayPinBrightnessTrackBar)).EndInit();
            this.panel2.ResumeLayout(false);
            this.pwmPinPanel.ResumeLayout(false);
            this.singlePinSelectFlowLayoutPanel.ResumeLayout(false);
            this.PinSelectContainer.ResumeLayout(false);
            this.PinSelectContainer.PerformLayout();
            this.PinSelectPanel.ResumeLayout(false);
            this.PinSelectPanel.PerformLayout();
            this.MultiSelectPinSelectContainer.ResumeLayout(false);
            this.MultiSelectPinSelectContainer.PerformLayout();
            this.LabelPinSelectContainer.ResumeLayout(false);
            this.ResumeLayout(false);

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
        public System.Windows.Forms.ComboBox displayPinComboBox;
        private PinSelectPanel MultiPinSelectPanel;
        private System.Windows.Forms.FlowLayoutPanel singlePinSelectFlowLayoutPanel;
        private System.Windows.Forms.CheckBox selectMultiplePinsCheckBox;
        private System.Windows.Forms.Panel PinSelectContainer;
        private System.Windows.Forms.Panel MultiSelectPinSelectContainer;
        private System.Windows.Forms.Panel LabelPinSelectContainer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel PinSelectPanel;
    }
}
