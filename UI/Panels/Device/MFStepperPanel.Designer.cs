namespace MobiFlight.UI.Panels.Settings.Device
{
    partial class MFStepperPanel
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MFStepperPanel));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mfPin4Label = new System.Windows.Forms.Label();
            this.mfPin4ComboBox = new System.Windows.Forms.ComboBox();
            this.mfPin1Label = new System.Windows.Forms.Label();
            this.mfPin1ComboBox = new System.Windows.Forms.ComboBox();
            this.mfPin3Label = new System.Windows.Forms.Label();
            this.mfPin3ComboBox = new System.Windows.Forms.ComboBox();
            this.mfPin2Label = new System.Windows.Forms.Label();
            this.mfPin2ComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mfNameTextBox = new System.Windows.Forms.TextBox();
            this.autoZeroPinGroupBox = new System.Windows.Forms.GroupBox();
            this.autoZeroCheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.mfBtnPinComboBox = new System.Windows.Forms.ComboBox();
            this.additionalSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.DefaultSpeedTextBox = new System.Windows.Forms.Label();
            this.DefaultAccelerationTextBox = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.deactivateCheckBox = new System.Windows.Forms.CheckBox();
            this.backlashPanel = new System.Windows.Forms.Panel();
            this.backlashValueLabel = new System.Windows.Forms.Label();
            this.BacklashTextBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ModeComboBox = new System.Windows.Forms.ComboBox();
            this.stepperModeLabel = new System.Windows.Forms.Label();
            this.stepperProfilePanel = new System.Windows.Forms.Panel();
            this.PresetComboBox = new System.Windows.Forms.ComboBox();
            this.stepperProfileLabel = new System.Windows.Forms.Label();
            this.stepperPanelToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.autoZeroPinGroupBox.SuspendLayout();
            this.additionalSettingsGroupBox.SuspendLayout();
            this.panel2.SuspendLayout();
            this.backlashPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.stepperProfilePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.mfPin4Label);
            this.groupBox1.Controls.Add(this.mfPin4ComboBox);
            this.groupBox1.Controls.Add(this.mfPin1Label);
            this.groupBox1.Controls.Add(this.mfPin1ComboBox);
            this.groupBox1.Controls.Add(this.mfPin3Label);
            this.groupBox1.Controls.Add(this.mfPin3ComboBox);
            this.groupBox1.Controls.Add(this.mfPin2Label);
            this.groupBox1.Controls.Add(this.mfPin2ComboBox);
            this.errorProvider.SetError(this.groupBox1, resources.GetString("groupBox1.Error"));
            this.errorProvider.SetIconAlignment(this.groupBox1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox1.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.groupBox1, ((int)(resources.GetObject("groupBox1.IconPadding"))));
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.stepperPanelToolTip.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // mfPin4Label
            // 
            resources.ApplyResources(this.mfPin4Label, "mfPin4Label");
            this.errorProvider.SetError(this.mfPin4Label, resources.GetString("mfPin4Label.Error"));
            this.errorProvider.SetIconAlignment(this.mfPin4Label, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mfPin4Label.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.mfPin4Label, ((int)(resources.GetObject("mfPin4Label.IconPadding"))));
            this.mfPin4Label.Name = "mfPin4Label";
            this.stepperPanelToolTip.SetToolTip(this.mfPin4Label, resources.GetString("mfPin4Label.ToolTip"));
            // 
            // mfPin4ComboBox
            // 
            resources.ApplyResources(this.mfPin4ComboBox, "mfPin4ComboBox");
            this.mfPin4ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.errorProvider.SetError(this.mfPin4ComboBox, resources.GetString("mfPin4ComboBox.Error"));
            this.mfPin4ComboBox.FormattingEnabled = true;
            this.errorProvider.SetIconAlignment(this.mfPin4ComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mfPin4ComboBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.mfPin4ComboBox, ((int)(resources.GetObject("mfPin4ComboBox.IconPadding"))));
            this.mfPin4ComboBox.Name = "mfPin4ComboBox";
            this.stepperPanelToolTip.SetToolTip(this.mfPin4ComboBox, resources.GetString("mfPin4ComboBox.ToolTip"));
            this.mfPin4ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfPin1Label
            // 
            resources.ApplyResources(this.mfPin1Label, "mfPin1Label");
            this.errorProvider.SetError(this.mfPin1Label, resources.GetString("mfPin1Label.Error"));
            this.errorProvider.SetIconAlignment(this.mfPin1Label, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mfPin1Label.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.mfPin1Label, ((int)(resources.GetObject("mfPin1Label.IconPadding"))));
            this.mfPin1Label.Name = "mfPin1Label";
            this.stepperPanelToolTip.SetToolTip(this.mfPin1Label, resources.GetString("mfPin1Label.ToolTip"));
            // 
            // mfPin1ComboBox
            // 
            resources.ApplyResources(this.mfPin1ComboBox, "mfPin1ComboBox");
            this.mfPin1ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.errorProvider.SetError(this.mfPin1ComboBox, resources.GetString("mfPin1ComboBox.Error"));
            this.mfPin1ComboBox.FormattingEnabled = true;
            this.errorProvider.SetIconAlignment(this.mfPin1ComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mfPin1ComboBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.mfPin1ComboBox, ((int)(resources.GetObject("mfPin1ComboBox.IconPadding"))));
            this.mfPin1ComboBox.Name = "mfPin1ComboBox";
            this.stepperPanelToolTip.SetToolTip(this.mfPin1ComboBox, resources.GetString("mfPin1ComboBox.ToolTip"));
            this.mfPin1ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfPin3Label
            // 
            resources.ApplyResources(this.mfPin3Label, "mfPin3Label");
            this.errorProvider.SetError(this.mfPin3Label, resources.GetString("mfPin3Label.Error"));
            this.errorProvider.SetIconAlignment(this.mfPin3Label, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mfPin3Label.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.mfPin3Label, ((int)(resources.GetObject("mfPin3Label.IconPadding"))));
            this.mfPin3Label.Name = "mfPin3Label";
            this.stepperPanelToolTip.SetToolTip(this.mfPin3Label, resources.GetString("mfPin3Label.ToolTip"));
            // 
            // mfPin3ComboBox
            // 
            resources.ApplyResources(this.mfPin3ComboBox, "mfPin3ComboBox");
            this.mfPin3ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.errorProvider.SetError(this.mfPin3ComboBox, resources.GetString("mfPin3ComboBox.Error"));
            this.mfPin3ComboBox.FormattingEnabled = true;
            this.errorProvider.SetIconAlignment(this.mfPin3ComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mfPin3ComboBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.mfPin3ComboBox, ((int)(resources.GetObject("mfPin3ComboBox.IconPadding"))));
            this.mfPin3ComboBox.Name = "mfPin3ComboBox";
            this.stepperPanelToolTip.SetToolTip(this.mfPin3ComboBox, resources.GetString("mfPin3ComboBox.ToolTip"));
            this.mfPin3ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfPin2Label
            // 
            resources.ApplyResources(this.mfPin2Label, "mfPin2Label");
            this.errorProvider.SetError(this.mfPin2Label, resources.GetString("mfPin2Label.Error"));
            this.errorProvider.SetIconAlignment(this.mfPin2Label, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mfPin2Label.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.mfPin2Label, ((int)(resources.GetObject("mfPin2Label.IconPadding"))));
            this.mfPin2Label.Name = "mfPin2Label";
            this.stepperPanelToolTip.SetToolTip(this.mfPin2Label, resources.GetString("mfPin2Label.ToolTip"));
            // 
            // mfPin2ComboBox
            // 
            resources.ApplyResources(this.mfPin2ComboBox, "mfPin2ComboBox");
            this.mfPin2ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.errorProvider.SetError(this.mfPin2ComboBox, resources.GetString("mfPin2ComboBox.Error"));
            this.mfPin2ComboBox.FormattingEnabled = true;
            this.errorProvider.SetIconAlignment(this.mfPin2ComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mfPin2ComboBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.mfPin2ComboBox, ((int)(resources.GetObject("mfPin2ComboBox.IconPadding"))));
            this.mfPin2ComboBox.Name = "mfPin2ComboBox";
            this.stepperPanelToolTip.SetToolTip(this.mfPin2ComboBox, resources.GetString("mfPin2ComboBox.ToolTip"));
            this.mfPin2ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.mfNameTextBox);
            this.errorProvider.SetError(this.groupBox2, resources.GetString("groupBox2.Error"));
            this.errorProvider.SetIconAlignment(this.groupBox2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox2.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.groupBox2, ((int)(resources.GetObject("groupBox2.IconPadding"))));
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.stepperPanelToolTip.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
            // 
            // mfNameTextBox
            // 
            resources.ApplyResources(this.mfNameTextBox, "mfNameTextBox");
            this.errorProvider.SetError(this.mfNameTextBox, resources.GetString("mfNameTextBox.Error"));
            this.errorProvider.SetIconAlignment(this.mfNameTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mfNameTextBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.mfNameTextBox, ((int)(resources.GetObject("mfNameTextBox.IconPadding"))));
            this.mfNameTextBox.Name = "mfNameTextBox";
            this.stepperPanelToolTip.SetToolTip(this.mfNameTextBox, resources.GetString("mfNameTextBox.ToolTip"));
            this.mfNameTextBox.TextChanged += new System.EventHandler(this.value_Changed);
            // 
            // autoZeroPinGroupBox
            // 
            resources.ApplyResources(this.autoZeroPinGroupBox, "autoZeroPinGroupBox");
            this.autoZeroPinGroupBox.Controls.Add(this.autoZeroCheckBox);
            this.autoZeroPinGroupBox.Controls.Add(this.label2);
            this.autoZeroPinGroupBox.Controls.Add(this.mfBtnPinComboBox);
            this.errorProvider.SetError(this.autoZeroPinGroupBox, resources.GetString("autoZeroPinGroupBox.Error"));
            this.errorProvider.SetIconAlignment(this.autoZeroPinGroupBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("autoZeroPinGroupBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.autoZeroPinGroupBox, ((int)(resources.GetObject("autoZeroPinGroupBox.IconPadding"))));
            this.autoZeroPinGroupBox.Name = "autoZeroPinGroupBox";
            this.autoZeroPinGroupBox.TabStop = false;
            this.stepperPanelToolTip.SetToolTip(this.autoZeroPinGroupBox, resources.GetString("autoZeroPinGroupBox.ToolTip"));
            // 
            // autoZeroCheckBox
            // 
            resources.ApplyResources(this.autoZeroCheckBox, "autoZeroCheckBox");
            this.errorProvider.SetError(this.autoZeroCheckBox, resources.GetString("autoZeroCheckBox.Error"));
            this.errorProvider.SetIconAlignment(this.autoZeroCheckBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("autoZeroCheckBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.autoZeroCheckBox, ((int)(resources.GetObject("autoZeroCheckBox.IconPadding"))));
            this.autoZeroCheckBox.Name = "autoZeroCheckBox";
            this.stepperPanelToolTip.SetToolTip(this.autoZeroCheckBox, resources.GetString("autoZeroCheckBox.ToolTip"));
            this.autoZeroCheckBox.UseVisualStyleBackColor = true;
            this.autoZeroCheckBox.CheckedChanged += new System.EventHandler(this.autoZeroCheckBox_CheckedChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.errorProvider.SetError(this.label2, resources.GetString("label2.Error"));
            this.errorProvider.SetIconAlignment(this.label2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label2.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label2, ((int)(resources.GetObject("label2.IconPadding"))));
            this.label2.Name = "label2";
            this.stepperPanelToolTip.SetToolTip(this.label2, resources.GetString("label2.ToolTip"));
            // 
            // mfBtnPinComboBox
            // 
            resources.ApplyResources(this.mfBtnPinComboBox, "mfBtnPinComboBox");
            this.mfBtnPinComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.errorProvider.SetError(this.mfBtnPinComboBox, resources.GetString("mfBtnPinComboBox.Error"));
            this.mfBtnPinComboBox.FormattingEnabled = true;
            this.errorProvider.SetIconAlignment(this.mfBtnPinComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("mfBtnPinComboBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.mfBtnPinComboBox, ((int)(resources.GetObject("mfBtnPinComboBox.IconPadding"))));
            this.mfBtnPinComboBox.Name = "mfBtnPinComboBox";
            this.stepperPanelToolTip.SetToolTip(this.mfBtnPinComboBox, resources.GetString("mfBtnPinComboBox.ToolTip"));
            this.mfBtnPinComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // additionalSettingsGroupBox
            // 
            resources.ApplyResources(this.additionalSettingsGroupBox, "additionalSettingsGroupBox");
            this.additionalSettingsGroupBox.Controls.Add(this.panel2);
            this.additionalSettingsGroupBox.Controls.Add(this.deactivateCheckBox);
            this.additionalSettingsGroupBox.Controls.Add(this.backlashPanel);
            this.additionalSettingsGroupBox.Controls.Add(this.panel1);
            this.additionalSettingsGroupBox.Controls.Add(this.stepperProfilePanel);
            this.errorProvider.SetError(this.additionalSettingsGroupBox, resources.GetString("additionalSettingsGroupBox.Error"));
            this.errorProvider.SetIconAlignment(this.additionalSettingsGroupBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("additionalSettingsGroupBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.additionalSettingsGroupBox, ((int)(resources.GetObject("additionalSettingsGroupBox.IconPadding"))));
            this.additionalSettingsGroupBox.Name = "additionalSettingsGroupBox";
            this.additionalSettingsGroupBox.TabStop = false;
            this.stepperPanelToolTip.SetToolTip(this.additionalSettingsGroupBox, resources.GetString("additionalSettingsGroupBox.ToolTip"));
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.DefaultSpeedTextBox);
            this.panel2.Controls.Add(this.DefaultAccelerationTextBox);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label1);
            this.errorProvider.SetError(this.panel2, resources.GetString("panel2.Error"));
            this.errorProvider.SetIconAlignment(this.panel2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("panel2.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.panel2, ((int)(resources.GetObject("panel2.IconPadding"))));
            this.panel2.Name = "panel2";
            this.stepperPanelToolTip.SetToolTip(this.panel2, resources.GetString("panel2.ToolTip"));
            // 
            // DefaultSpeedTextBox
            // 
            resources.ApplyResources(this.DefaultSpeedTextBox, "DefaultSpeedTextBox");
            this.errorProvider.SetError(this.DefaultSpeedTextBox, resources.GetString("DefaultSpeedTextBox.Error"));
            this.errorProvider.SetIconAlignment(this.DefaultSpeedTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("DefaultSpeedTextBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.DefaultSpeedTextBox, ((int)(resources.GetObject("DefaultSpeedTextBox.IconPadding"))));
            this.DefaultSpeedTextBox.Name = "DefaultSpeedTextBox";
            this.stepperPanelToolTip.SetToolTip(this.DefaultSpeedTextBox, resources.GetString("DefaultSpeedTextBox.ToolTip"));
            // 
            // DefaultAccelerationTextBox
            // 
            resources.ApplyResources(this.DefaultAccelerationTextBox, "DefaultAccelerationTextBox");
            this.errorProvider.SetError(this.DefaultAccelerationTextBox, resources.GetString("DefaultAccelerationTextBox.Error"));
            this.errorProvider.SetIconAlignment(this.DefaultAccelerationTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("DefaultAccelerationTextBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.DefaultAccelerationTextBox, ((int)(resources.GetObject("DefaultAccelerationTextBox.IconPadding"))));
            this.DefaultAccelerationTextBox.Name = "DefaultAccelerationTextBox";
            this.stepperPanelToolTip.SetToolTip(this.DefaultAccelerationTextBox, resources.GetString("DefaultAccelerationTextBox.ToolTip"));
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.errorProvider.SetError(this.label3, resources.GetString("label3.Error"));
            this.errorProvider.SetIconAlignment(this.label3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label3.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label3, ((int)(resources.GetObject("label3.IconPadding"))));
            this.label3.Name = "label3";
            this.stepperPanelToolTip.SetToolTip(this.label3, resources.GetString("label3.ToolTip"));
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.errorProvider.SetError(this.label1, resources.GetString("label1.Error"));
            this.errorProvider.SetIconAlignment(this.label1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label1.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label1, ((int)(resources.GetObject("label1.IconPadding"))));
            this.label1.Name = "label1";
            this.stepperPanelToolTip.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // deactivateCheckBox
            // 
            resources.ApplyResources(this.deactivateCheckBox, "deactivateCheckBox");
            this.errorProvider.SetError(this.deactivateCheckBox, resources.GetString("deactivateCheckBox.Error"));
            this.errorProvider.SetIconAlignment(this.deactivateCheckBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("deactivateCheckBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.deactivateCheckBox, ((int)(resources.GetObject("deactivateCheckBox.IconPadding"))));
            this.deactivateCheckBox.Name = "deactivateCheckBox";
            this.stepperPanelToolTip.SetToolTip(this.deactivateCheckBox, resources.GetString("deactivateCheckBox.ToolTip"));
            this.deactivateCheckBox.UseVisualStyleBackColor = true;
            this.deactivateCheckBox.CheckedChanged += new System.EventHandler(this.value_Changed);
            // 
            // backlashPanel
            // 
            resources.ApplyResources(this.backlashPanel, "backlashPanel");
            this.backlashPanel.Controls.Add(this.backlashValueLabel);
            this.backlashPanel.Controls.Add(this.BacklashTextBox);
            this.errorProvider.SetError(this.backlashPanel, resources.GetString("backlashPanel.Error"));
            this.errorProvider.SetIconAlignment(this.backlashPanel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("backlashPanel.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.backlashPanel, ((int)(resources.GetObject("backlashPanel.IconPadding"))));
            this.backlashPanel.Name = "backlashPanel";
            this.stepperPanelToolTip.SetToolTip(this.backlashPanel, resources.GetString("backlashPanel.ToolTip"));
            // 
            // backlashValueLabel
            // 
            resources.ApplyResources(this.backlashValueLabel, "backlashValueLabel");
            this.errorProvider.SetError(this.backlashValueLabel, resources.GetString("backlashValueLabel.Error"));
            this.errorProvider.SetIconAlignment(this.backlashValueLabel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("backlashValueLabel.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.backlashValueLabel, ((int)(resources.GetObject("backlashValueLabel.IconPadding"))));
            this.backlashValueLabel.Name = "backlashValueLabel";
            this.stepperPanelToolTip.SetToolTip(this.backlashValueLabel, resources.GetString("backlashValueLabel.ToolTip"));
            // 
            // BacklashTextBox
            // 
            resources.ApplyResources(this.BacklashTextBox, "BacklashTextBox");
            this.errorProvider.SetError(this.BacklashTextBox, resources.GetString("BacklashTextBox.Error"));
            this.errorProvider.SetIconAlignment(this.BacklashTextBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("BacklashTextBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.BacklashTextBox, ((int)(resources.GetObject("BacklashTextBox.IconPadding"))));
            this.BacklashTextBox.Name = "BacklashTextBox";
            this.stepperPanelToolTip.SetToolTip(this.BacklashTextBox, resources.GetString("BacklashTextBox.ToolTip"));
            this.BacklashTextBox.TextChanged += new System.EventHandler(this.value_Changed);
            this.BacklashTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.BacklashTextBox_Validating);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.ModeComboBox);
            this.panel1.Controls.Add(this.stepperModeLabel);
            this.errorProvider.SetError(this.panel1, resources.GetString("panel1.Error"));
            this.errorProvider.SetIconAlignment(this.panel1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("panel1.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.panel1, ((int)(resources.GetObject("panel1.IconPadding"))));
            this.panel1.Name = "panel1";
            this.stepperPanelToolTip.SetToolTip(this.panel1, resources.GetString("panel1.ToolTip"));
            // 
            // ModeComboBox
            // 
            resources.ApplyResources(this.ModeComboBox, "ModeComboBox");
            this.ModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.errorProvider.SetError(this.ModeComboBox, resources.GetString("ModeComboBox.Error"));
            this.ModeComboBox.FormattingEnabled = true;
            this.errorProvider.SetIconAlignment(this.ModeComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("ModeComboBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.ModeComboBox, ((int)(resources.GetObject("ModeComboBox.IconPadding"))));
            this.ModeComboBox.Name = "ModeComboBox";
            this.stepperPanelToolTip.SetToolTip(this.ModeComboBox, resources.GetString("ModeComboBox.ToolTip"));
            this.ModeComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // stepperModeLabel
            // 
            resources.ApplyResources(this.stepperModeLabel, "stepperModeLabel");
            this.errorProvider.SetError(this.stepperModeLabel, resources.GetString("stepperModeLabel.Error"));
            this.errorProvider.SetIconAlignment(this.stepperModeLabel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("stepperModeLabel.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.stepperModeLabel, ((int)(resources.GetObject("stepperModeLabel.IconPadding"))));
            this.stepperModeLabel.Name = "stepperModeLabel";
            this.stepperPanelToolTip.SetToolTip(this.stepperModeLabel, resources.GetString("stepperModeLabel.ToolTip"));
            // 
            // stepperProfilePanel
            // 
            resources.ApplyResources(this.stepperProfilePanel, "stepperProfilePanel");
            this.stepperProfilePanel.Controls.Add(this.PresetComboBox);
            this.stepperProfilePanel.Controls.Add(this.stepperProfileLabel);
            this.errorProvider.SetError(this.stepperProfilePanel, resources.GetString("stepperProfilePanel.Error"));
            this.errorProvider.SetIconAlignment(this.stepperProfilePanel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("stepperProfilePanel.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.stepperProfilePanel, ((int)(resources.GetObject("stepperProfilePanel.IconPadding"))));
            this.stepperProfilePanel.Name = "stepperProfilePanel";
            this.stepperPanelToolTip.SetToolTip(this.stepperProfilePanel, resources.GetString("stepperProfilePanel.ToolTip"));
            // 
            // PresetComboBox
            // 
            resources.ApplyResources(this.PresetComboBox, "PresetComboBox");
            this.PresetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.errorProvider.SetError(this.PresetComboBox, resources.GetString("PresetComboBox.Error"));
            this.PresetComboBox.FormattingEnabled = true;
            this.errorProvider.SetIconAlignment(this.PresetComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("PresetComboBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.PresetComboBox, ((int)(resources.GetObject("PresetComboBox.IconPadding"))));
            this.PresetComboBox.Items.AddRange(new object[] {
            resources.GetString("PresetComboBox.Items"),
            resources.GetString("PresetComboBox.Items1"),
            resources.GetString("PresetComboBox.Items2")});
            this.PresetComboBox.Name = "PresetComboBox";
            this.stepperPanelToolTip.SetToolTip(this.PresetComboBox, resources.GetString("PresetComboBox.ToolTip"));
            this.PresetComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // stepperProfileLabel
            // 
            resources.ApplyResources(this.stepperProfileLabel, "stepperProfileLabel");
            this.errorProvider.SetError(this.stepperProfileLabel, resources.GetString("stepperProfileLabel.Error"));
            this.errorProvider.SetIconAlignment(this.stepperProfileLabel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("stepperProfileLabel.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.stepperProfileLabel, ((int)(resources.GetObject("stepperProfileLabel.IconPadding"))));
            this.stepperProfileLabel.Name = "stepperProfileLabel";
            this.stepperPanelToolTip.SetToolTip(this.stepperProfileLabel, resources.GetString("stepperProfileLabel.ToolTip"));
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // MFStepperPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.additionalSettingsGroupBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.autoZeroPinGroupBox);
            this.Controls.Add(this.groupBox1);
            this.errorProvider.SetError(this, resources.GetString("$this.Error"));
            this.errorProvider.SetIconAlignment(this, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("$this.IconAlignment"))));
            this.errorProvider.SetIconPadding(this, ((int)(resources.GetObject("$this.IconPadding"))));
            this.Name = "MFStepperPanel";
            this.stepperPanelToolTip.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.autoZeroPinGroupBox.ResumeLayout(false);
            this.autoZeroPinGroupBox.PerformLayout();
            this.additionalSettingsGroupBox.ResumeLayout(false);
            this.additionalSettingsGroupBox.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.backlashPanel.ResumeLayout(false);
            this.backlashPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.stepperProfilePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label mfPin3Label;
        private System.Windows.Forms.ComboBox mfPin3ComboBox;
        private System.Windows.Forms.Label mfPin2Label;
        private System.Windows.Forms.ComboBox mfPin2ComboBox;
        private System.Windows.Forms.Label mfPin1Label;
        private System.Windows.Forms.ComboBox mfPin1ComboBox;
        private System.Windows.Forms.Label mfPin4Label;
        private System.Windows.Forms.ComboBox mfPin4ComboBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox mfNameTextBox;
        private System.Windows.Forms.GroupBox autoZeroPinGroupBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox mfBtnPinComboBox;
        private System.Windows.Forms.CheckBox autoZeroCheckBox;
        private System.Windows.Forms.GroupBox additionalSettingsGroupBox;
        private System.Windows.Forms.CheckBox deactivateCheckBox;
        private System.Windows.Forms.Label stepperModeLabel;
        private System.Windows.Forms.ComboBox ModeComboBox;
        private System.Windows.Forms.Panel backlashPanel;
        private System.Windows.Forms.Label backlashValueLabel;
        private System.Windows.Forms.TextBox BacklashTextBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel stepperProfilePanel;
        private System.Windows.Forms.ComboBox PresetComboBox;
        private System.Windows.Forms.Label stepperProfileLabel;
        private System.Windows.Forms.Label DefaultSpeedTextBox;
        private System.Windows.Forms.Label DefaultAccelerationTextBox;
        private System.Windows.Forms.ToolTip stepperPanelToolTip;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
