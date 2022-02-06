namespace MobiFlight.UI.Panels.OutputWizard
{
    partial class DisplayPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisplayPanel));
            this.displayTypeGroupBox = new System.Windows.Forms.GroupBox();
            this.arcazeSerialLabel = new System.Windows.Forms.Label();
            this.displayModuleNameComboBox = new System.Windows.Forms.ComboBox();
            this.displayTypeComboBoxLabel = new System.Windows.Forms.Label();
            this.displayTypeComboBox = new System.Windows.Forms.ComboBox();
            this.OutputTypeLabel = new System.Windows.Forms.Label();
            this.testSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.displayPinTestStopButton = new System.Windows.Forms.Button();
            this.displayPinTestButton = new System.Windows.Forms.Button();
            this.groupBoxDisplaySettings = new System.Windows.Forms.GroupBox();
            this.inputActionGroupBox = new System.Windows.Forms.GroupBox();
            this.OutputDevicePanel = new System.Windows.Forms.Panel();
            this.DisplayPanelTextLabel = new System.Windows.Forms.Label();
            this.OutputTypeComboBox = new System.Windows.Forms.ComboBox();
            this.DisplayTypePanel = new System.Windows.Forms.Panel();
            this.OutputTypePanel = new System.Windows.Forms.Panel();
            this.InputActionTypePanel = new System.Windows.Forms.Panel();
            this.InputTypeButtonRadioButton = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.analogPanel1 = new MobiFlight.UI.Panels.Input.AnalogPanel();
            this.displayTypeGroupBox.SuspendLayout();
            this.testSettingsGroupBox.SuspendLayout();
            this.inputActionGroupBox.SuspendLayout();
            this.OutputDevicePanel.SuspendLayout();
            this.DisplayTypePanel.SuspendLayout();
            this.OutputTypePanel.SuspendLayout();
            this.InputActionTypePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // displayTypeGroupBox
            // 
            resources.ApplyResources(this.displayTypeGroupBox, "displayTypeGroupBox");
            this.displayTypeGroupBox.Controls.Add(this.InputActionTypePanel);
            this.displayTypeGroupBox.Controls.Add(this.DisplayTypePanel);
            this.displayTypeGroupBox.Controls.Add(this.OutputTypePanel);
            this.displayTypeGroupBox.Name = "displayTypeGroupBox";
            this.displayTypeGroupBox.TabStop = false;
            // 
            // arcazeSerialLabel
            // 
            resources.ApplyResources(this.arcazeSerialLabel, "arcazeSerialLabel");
            this.arcazeSerialLabel.Name = "arcazeSerialLabel";
            // 
            // displayModuleNameComboBox
            // 
            this.displayModuleNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.displayModuleNameComboBox.FormattingEnabled = true;
            this.displayModuleNameComboBox.Items.AddRange(new object[] {
            resources.GetString("displayModuleNameComboBox.Items"),
            resources.GetString("displayModuleNameComboBox.Items1")});
            resources.ApplyResources(this.displayModuleNameComboBox, "displayModuleNameComboBox");
            this.displayModuleNameComboBox.Name = "displayModuleNameComboBox";
            this.displayModuleNameComboBox.SelectedIndexChanged += new System.EventHandler(this.displaySerialComboBox_SelectedIndexChanged);
            // 
            // displayTypeComboBoxLabel
            // 
            resources.ApplyResources(this.displayTypeComboBoxLabel, "displayTypeComboBoxLabel");
            this.displayTypeComboBoxLabel.Name = "displayTypeComboBoxLabel";
            // 
            // displayTypeComboBox
            // 
            this.displayTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.displayTypeComboBox.FormattingEnabled = true;
            this.displayTypeComboBox.Items.AddRange(new object[] {
            resources.GetString("displayTypeComboBox.Items"),
            resources.GetString("displayTypeComboBox.Items1"),
            resources.GetString("displayTypeComboBox.Items2")});
            resources.ApplyResources(this.displayTypeComboBox, "displayTypeComboBox");
            this.displayTypeComboBox.Name = "displayTypeComboBox";
            this.displayTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.displayTypeComboBox_SelectedIndexChanged);
            // 
            // OutputTypeLabel
            // 
            resources.ApplyResources(this.OutputTypeLabel, "OutputTypeLabel");
            this.OutputTypeLabel.Name = "OutputTypeLabel";
            // 
            // testSettingsGroupBox
            // 
            this.testSettingsGroupBox.Controls.Add(this.displayPinTestStopButton);
            this.testSettingsGroupBox.Controls.Add(this.displayPinTestButton);
            resources.ApplyResources(this.testSettingsGroupBox, "testSettingsGroupBox");
            this.testSettingsGroupBox.Name = "testSettingsGroupBox";
            this.testSettingsGroupBox.TabStop = false;
            // 
            // displayPinTestStopButton
            // 
            resources.ApplyResources(this.displayPinTestStopButton, "displayPinTestStopButton");
            this.displayPinTestStopButton.Image = global::MobiFlight.Properties.Resources.media_stop;
            this.displayPinTestStopButton.Name = "displayPinTestStopButton";
            this.displayPinTestStopButton.UseVisualStyleBackColor = true;
            this.displayPinTestStopButton.Click += new System.EventHandler(this.displayPinTestStopButton_Click);
            // 
            // displayPinTestButton
            // 
            resources.ApplyResources(this.displayPinTestButton, "displayPinTestButton");
            this.displayPinTestButton.Image = global::MobiFlight.Properties.Resources.media_play;
            this.displayPinTestButton.Name = "displayPinTestButton";
            this.displayPinTestButton.UseVisualStyleBackColor = true;
            this.displayPinTestButton.Click += new System.EventHandler(this.displayPinTestButton_Click);
            // 
            // groupBoxDisplaySettings
            // 
            resources.ApplyResources(this.groupBoxDisplaySettings, "groupBoxDisplaySettings");
            this.groupBoxDisplaySettings.Name = "groupBoxDisplaySettings";
            this.groupBoxDisplaySettings.TabStop = false;
            // 
            // inputActionGroupBox
            // 
            resources.ApplyResources(this.inputActionGroupBox, "inputActionGroupBox");
            this.inputActionGroupBox.Controls.Add(this.analogPanel1);
            this.inputActionGroupBox.Name = "inputActionGroupBox";
            this.inputActionGroupBox.TabStop = false;
            // 
            // OutputDevicePanel
            // 
            resources.ApplyResources(this.OutputDevicePanel, "OutputDevicePanel");
            this.OutputDevicePanel.Controls.Add(this.testSettingsGroupBox);
            this.OutputDevicePanel.Controls.Add(this.groupBoxDisplaySettings);
            this.OutputDevicePanel.Controls.Add(this.displayTypeGroupBox);
            this.OutputDevicePanel.Name = "OutputDevicePanel";
            // 
            // DisplayPanelTextLabel
            // 
            resources.ApplyResources(this.DisplayPanelTextLabel, "DisplayPanelTextLabel");
            this.DisplayPanelTextLabel.Name = "DisplayPanelTextLabel";
            // 
            // OutputTypeComboBox
            // 
            this.OutputTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.OutputTypeComboBox.FormattingEnabled = true;
            this.OutputTypeComboBox.Items.AddRange(new object[] {
            resources.GetString("OutputTypeComboBox.Items"),
            resources.GetString("OutputTypeComboBox.Items1")});
            resources.ApplyResources(this.OutputTypeComboBox, "OutputTypeComboBox");
            this.OutputTypeComboBox.Name = "OutputTypeComboBox";
            this.OutputTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.OutputTypeComboBox_SelectedIndexChanged);
            // 
            // DisplayTypePanel
            // 
            this.DisplayTypePanel.Controls.Add(this.arcazeSerialLabel);
            this.DisplayTypePanel.Controls.Add(this.displayModuleNameComboBox);
            this.DisplayTypePanel.Controls.Add(this.displayTypeComboBoxLabel);
            this.DisplayTypePanel.Controls.Add(this.displayTypeComboBox);
            resources.ApplyResources(this.DisplayTypePanel, "DisplayTypePanel");
            this.DisplayTypePanel.Name = "DisplayTypePanel";
            // 
            // OutputTypePanel
            // 
            this.OutputTypePanel.Controls.Add(this.OutputTypeComboBox);
            this.OutputTypePanel.Controls.Add(this.OutputTypeLabel);
            resources.ApplyResources(this.OutputTypePanel, "OutputTypePanel");
            this.OutputTypePanel.Name = "OutputTypePanel";
            // 
            // InputActionTypePanel
            // 
            this.InputActionTypePanel.Controls.Add(this.radioButton1);
            this.InputActionTypePanel.Controls.Add(this.InputTypeButtonRadioButton);
            resources.ApplyResources(this.InputActionTypePanel, "InputActionTypePanel");
            this.InputActionTypePanel.Name = "InputActionTypePanel";
            // 
            // InputTypeButtonRadioButton
            // 
            resources.ApplyResources(this.InputTypeButtonRadioButton, "InputTypeButtonRadioButton");
            this.InputTypeButtonRadioButton.Name = "InputTypeButtonRadioButton";
            this.InputTypeButtonRadioButton.TabStop = true;
            this.InputTypeButtonRadioButton.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            resources.ApplyResources(this.radioButton1, "radioButton1");
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.TabStop = true;
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // analogPanel1
            // 
            resources.ApplyResources(this.analogPanel1, "analogPanel1");
            this.analogPanel1.Name = "analogPanel1";
            // 
            // DisplayPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.inputActionGroupBox);
            this.Controls.Add(this.OutputDevicePanel);
            this.Controls.Add(this.DisplayPanelTextLabel);
            this.Name = "DisplayPanel";
            this.displayTypeGroupBox.ResumeLayout(false);
            this.testSettingsGroupBox.ResumeLayout(false);
            this.inputActionGroupBox.ResumeLayout(false);
            this.inputActionGroupBox.PerformLayout();
            this.OutputDevicePanel.ResumeLayout(false);
            this.OutputDevicePanel.PerformLayout();
            this.DisplayTypePanel.ResumeLayout(false);
            this.OutputTypePanel.ResumeLayout(false);
            this.InputActionTypePanel.ResumeLayout(false);
            this.InputActionTypePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox displayTypeGroupBox;
        private System.Windows.Forms.Label OutputTypeLabel;
        private System.Windows.Forms.Label arcazeSerialLabel;
        private System.Windows.Forms.ComboBox displayModuleNameComboBox;
        private System.Windows.Forms.Label displayTypeComboBoxLabel;
        private System.Windows.Forms.ComboBox displayTypeComboBox;
        private System.Windows.Forms.GroupBox testSettingsGroupBox;
        private System.Windows.Forms.Button displayPinTestStopButton;
        private System.Windows.Forms.Button displayPinTestButton;
        private System.Windows.Forms.GroupBox groupBoxDisplaySettings;
        private Input.AnalogPanel analogPanel1;
        private System.Windows.Forms.GroupBox inputActionGroupBox;
        private System.Windows.Forms.Panel OutputDevicePanel;
        private System.Windows.Forms.Label DisplayPanelTextLabel;
        private System.Windows.Forms.ComboBox OutputTypeComboBox;
        private System.Windows.Forms.Panel InputActionTypePanel;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton InputTypeButtonRadioButton;
        private System.Windows.Forms.Panel DisplayTypePanel;
        private System.Windows.Forms.Panel OutputTypePanel;
    }
}
