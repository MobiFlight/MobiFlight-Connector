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
            this.InputActionRadioButton = new System.Windows.Forms.RadioButton();
            this.OutputTypeDeviceRadioButton = new System.Windows.Forms.RadioButton();
            this.testSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.displayPinTestStopButton = new System.Windows.Forms.Button();
            this.displayPinTestButton = new System.Windows.Forms.Button();
            this.groupBoxDisplaySettings = new System.Windows.Forms.GroupBox();
            this.displayTabTextBox = new System.Windows.Forms.TextBox();
            this.analogPanel1 = new MobiFlight.UI.Panels.Input.AnalogPanel();
            this.inputActionGroupBox = new System.Windows.Forms.GroupBox();
            this.OutputTypeGroupBox = new System.Windows.Forms.GroupBox();
            this.displayTypeGroupBox.SuspendLayout();
            this.testSettingsGroupBox.SuspendLayout();
            this.inputActionGroupBox.SuspendLayout();
            this.OutputTypeGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // displayTypeGroupBox
            // 
            this.displayTypeGroupBox.Controls.Add(this.arcazeSerialLabel);
            this.displayTypeGroupBox.Controls.Add(this.displayModuleNameComboBox);
            this.displayTypeGroupBox.Controls.Add(this.displayTypeComboBoxLabel);
            this.displayTypeGroupBox.Controls.Add(this.displayTypeComboBox);
            resources.ApplyResources(this.displayTypeGroupBox, "displayTypeGroupBox");
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
            // InputActionRadioButton
            // 
            resources.ApplyResources(this.InputActionRadioButton, "InputActionRadioButton");
            this.InputActionRadioButton.Name = "InputActionRadioButton";
            this.InputActionRadioButton.UseVisualStyleBackColor = true;
            this.InputActionRadioButton.CheckedChanged += new System.EventHandler(this.OutputTypeDeviceRadioButton_CheckedChanged);
            // 
            // OutputTypeDeviceRadioButton
            // 
            resources.ApplyResources(this.OutputTypeDeviceRadioButton, "OutputTypeDeviceRadioButton");
            this.OutputTypeDeviceRadioButton.Name = "OutputTypeDeviceRadioButton";
            this.OutputTypeDeviceRadioButton.UseVisualStyleBackColor = true;
            this.OutputTypeDeviceRadioButton.CheckedChanged += new System.EventHandler(this.OutputTypeDeviceRadioButton_CheckedChanged);
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
            // displayTabTextBox
            // 
            this.displayTabTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.displayTabTextBox.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.displayTabTextBox, "displayTabTextBox");
            this.displayTabTextBox.Name = "displayTabTextBox";
            this.displayTabTextBox.ReadOnly = true;
            this.displayTabTextBox.TabStop = false;
            // 
            // analogPanel1
            // 
            resources.ApplyResources(this.analogPanel1, "analogPanel1");
            this.analogPanel1.Name = "analogPanel1";
            // 
            // inputActionGroupBox
            // 
            resources.ApplyResources(this.inputActionGroupBox, "inputActionGroupBox");
            this.inputActionGroupBox.Controls.Add(this.analogPanel1);
            this.inputActionGroupBox.Name = "inputActionGroupBox";
            this.inputActionGroupBox.TabStop = false;
            // 
            // OutputTypeGroupBox
            // 
            this.OutputTypeGroupBox.Controls.Add(this.OutputTypeLabel);
            this.OutputTypeGroupBox.Controls.Add(this.InputActionRadioButton);
            this.OutputTypeGroupBox.Controls.Add(this.OutputTypeDeviceRadioButton);
            resources.ApplyResources(this.OutputTypeGroupBox, "OutputTypeGroupBox");
            this.OutputTypeGroupBox.Name = "OutputTypeGroupBox";
            this.OutputTypeGroupBox.TabStop = false;
            // 
            // DisplayPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.testSettingsGroupBox);
            this.Controls.Add(this.inputActionGroupBox);
            this.Controls.Add(this.groupBoxDisplaySettings);
            this.Controls.Add(this.displayTypeGroupBox);
            this.Controls.Add(this.OutputTypeGroupBox);
            this.Controls.Add(this.displayTabTextBox);
            this.Name = "DisplayPanel";
            this.displayTypeGroupBox.ResumeLayout(false);
            this.testSettingsGroupBox.ResumeLayout(false);
            this.inputActionGroupBox.ResumeLayout(false);
            this.inputActionGroupBox.PerformLayout();
            this.OutputTypeGroupBox.ResumeLayout(false);
            this.OutputTypeGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox displayTypeGroupBox;
        private System.Windows.Forms.Label OutputTypeLabel;
        private System.Windows.Forms.RadioButton InputActionRadioButton;
        private System.Windows.Forms.RadioButton OutputTypeDeviceRadioButton;
        private System.Windows.Forms.Label arcazeSerialLabel;
        private System.Windows.Forms.ComboBox displayModuleNameComboBox;
        private System.Windows.Forms.Label displayTypeComboBoxLabel;
        private System.Windows.Forms.ComboBox displayTypeComboBox;
        private System.Windows.Forms.GroupBox testSettingsGroupBox;
        private System.Windows.Forms.Button displayPinTestStopButton;
        private System.Windows.Forms.Button displayPinTestButton;
        private System.Windows.Forms.GroupBox groupBoxDisplaySettings;
        private System.Windows.Forms.TextBox displayTabTextBox;
        private Input.AnalogPanel analogPanel1;
        private System.Windows.Forms.GroupBox inputActionGroupBox;
        private System.Windows.Forms.GroupBox OutputTypeGroupBox;
    }
}
