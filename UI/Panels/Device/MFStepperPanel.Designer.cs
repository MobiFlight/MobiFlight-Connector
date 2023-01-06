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
            this.deactivateCheckBox = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.DefaultAccelerationTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.DefaultSpeedTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.backlashPanel = new System.Windows.Forms.Panel();
            this.backlashValueLabel = new System.Windows.Forms.Label();
            this.BacklashTextBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ModeComboBox = new System.Windows.Forms.ComboBox();
            this.stepperModeLabel = new System.Windows.Forms.Label();
            this.stepperProfilePanel = new System.Windows.Forms.Panel();
            this.PresetComboBox = new System.Windows.Forms.ComboBox();
            this.stepperProfileLabel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.autoZeroPinGroupBox.SuspendLayout();
            this.additionalSettingsGroupBox.SuspendLayout();
            this.panel2.SuspendLayout();
            this.backlashPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.stepperProfilePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.mfPin4Label);
            this.groupBox1.Controls.Add(this.mfPin4ComboBox);
            this.groupBox1.Controls.Add(this.mfPin1Label);
            this.groupBox1.Controls.Add(this.mfPin1ComboBox);
            this.groupBox1.Controls.Add(this.mfPin3Label);
            this.groupBox1.Controls.Add(this.mfPin3ComboBox);
            this.groupBox1.Controls.Add(this.mfPin2Label);
            this.groupBox1.Controls.Add(this.mfPin2ComboBox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(418, 94);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pin Settings Motor";
            // 
            // mfPin4Label
            // 
            this.mfPin4Label.Location = new System.Drawing.Point(256, 26);
            this.mfPin4Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mfPin4Label.Name = "mfPin4Label";
            this.mfPin4Label.Size = new System.Drawing.Size(46, 22);
            this.mfPin4Label.TabIndex = 20;
            this.mfPin4Label.Text = "Pin 4";
            // 
            // mfPin4ComboBox
            // 
            this.mfPin4ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mfPin4ComboBox.FormattingEnabled = true;
            this.mfPin4ComboBox.Location = new System.Drawing.Point(256, 51);
            this.mfPin4ComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mfPin4ComboBox.MaxLength = 2;
            this.mfPin4ComboBox.Name = "mfPin4ComboBox";
            this.mfPin4ComboBox.Size = new System.Drawing.Size(66, 28);
            this.mfPin4ComboBox.TabIndex = 19;
            this.mfPin4ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfPin1Label
            // 
            this.mfPin1Label.Location = new System.Drawing.Point(22, 26);
            this.mfPin1Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mfPin1Label.Name = "mfPin1Label";
            this.mfPin1Label.Size = new System.Drawing.Size(46, 22);
            this.mfPin1Label.TabIndex = 16;
            this.mfPin1Label.Text = "Pin 1";
            // 
            // mfPin1ComboBox
            // 
            this.mfPin1ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mfPin1ComboBox.FormattingEnabled = true;
            this.mfPin1ComboBox.Location = new System.Drawing.Point(27, 51);
            this.mfPin1ComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mfPin1ComboBox.MaxLength = 2;
            this.mfPin1ComboBox.Name = "mfPin1ComboBox";
            this.mfPin1ComboBox.Size = new System.Drawing.Size(66, 28);
            this.mfPin1ComboBox.TabIndex = 15;
            this.mfPin1ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfPin3Label
            // 
            this.mfPin3Label.Location = new System.Drawing.Point(180, 26);
            this.mfPin3Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mfPin3Label.Name = "mfPin3Label";
            this.mfPin3Label.Size = new System.Drawing.Size(46, 22);
            this.mfPin3Label.TabIndex = 18;
            this.mfPin3Label.Text = "Pin 3";
            // 
            // mfPin3ComboBox
            // 
            this.mfPin3ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mfPin3ComboBox.FormattingEnabled = true;
            this.mfPin3ComboBox.Location = new System.Drawing.Point(180, 51);
            this.mfPin3ComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mfPin3ComboBox.MaxLength = 2;
            this.mfPin3ComboBox.Name = "mfPin3ComboBox";
            this.mfPin3ComboBox.Size = new System.Drawing.Size(66, 28);
            this.mfPin3ComboBox.TabIndex = 17;
            this.mfPin3ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfPin2Label
            // 
            this.mfPin2Label.Location = new System.Drawing.Point(104, 26);
            this.mfPin2Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mfPin2Label.Name = "mfPin2Label";
            this.mfPin2Label.Size = new System.Drawing.Size(46, 22);
            this.mfPin2Label.TabIndex = 16;
            this.mfPin2Label.Text = "Pin 2";
            // 
            // mfPin2ComboBox
            // 
            this.mfPin2ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mfPin2ComboBox.FormattingEnabled = true;
            this.mfPin2ComboBox.Location = new System.Drawing.Point(104, 51);
            this.mfPin2ComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mfPin2ComboBox.MaxLength = 2;
            this.mfPin2ComboBox.Name = "mfPin2ComboBox";
            this.mfPin2ComboBox.Size = new System.Drawing.Size(66, 28);
            this.mfPin2ComboBox.TabIndex = 15;
            this.mfPin2ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.mfNameTextBox);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 154);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(418, 74);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Name";
            // 
            // mfNameTextBox
            // 
            this.mfNameTextBox.Location = new System.Drawing.Point(27, 29);
            this.mfNameTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mfNameTextBox.Name = "mfNameTextBox";
            this.mfNameTextBox.Size = new System.Drawing.Size(224, 26);
            this.mfNameTextBox.TabIndex = 0;
            this.mfNameTextBox.TextChanged += new System.EventHandler(this.value_Changed);
            // 
            // autoZeroPinGroupBox
            // 
            this.autoZeroPinGroupBox.Controls.Add(this.autoZeroCheckBox);
            this.autoZeroPinGroupBox.Controls.Add(this.label2);
            this.autoZeroPinGroupBox.Controls.Add(this.mfBtnPinComboBox);
            this.autoZeroPinGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.autoZeroPinGroupBox.Location = new System.Drawing.Point(0, 94);
            this.autoZeroPinGroupBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.autoZeroPinGroupBox.Name = "autoZeroPinGroupBox";
            this.autoZeroPinGroupBox.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.autoZeroPinGroupBox.Size = new System.Drawing.Size(418, 60);
            this.autoZeroPinGroupBox.TabIndex = 3;
            this.autoZeroPinGroupBox.TabStop = false;
            this.autoZeroPinGroupBox.Text = "Auto Zero Input";
            // 
            // autoZeroCheckBox
            // 
            this.autoZeroCheckBox.AutoSize = true;
            this.autoZeroCheckBox.Location = new System.Drawing.Point(27, 25);
            this.autoZeroCheckBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.autoZeroCheckBox.Name = "autoZeroCheckBox";
            this.autoZeroCheckBox.Size = new System.Drawing.Size(73, 24);
            this.autoZeroCheckBox.TabIndex = 17;
            this.autoZeroCheckBox.Text = "None";
            this.autoZeroCheckBox.UseVisualStyleBackColor = true;
            this.autoZeroCheckBox.CheckedChanged += new System.EventHandler(this.autoZeroCheckBox_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(256, 25);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 28);
            this.label2.TabIndex = 16;
            this.label2.Text = "Pin";
            // 
            // mfBtnPinComboBox
            // 
            this.mfBtnPinComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mfBtnPinComboBox.FormattingEnabled = true;
            this.mfBtnPinComboBox.Location = new System.Drawing.Point(180, 22);
            this.mfBtnPinComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mfBtnPinComboBox.MaxLength = 2;
            this.mfBtnPinComboBox.Name = "mfBtnPinComboBox";
            this.mfBtnPinComboBox.Size = new System.Drawing.Size(66, 28);
            this.mfBtnPinComboBox.TabIndex = 15;
            this.mfBtnPinComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // additionalSettingsGroupBox
            // 
            this.additionalSettingsGroupBox.AutoSize = true;
            this.additionalSettingsGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.additionalSettingsGroupBox.Controls.Add(this.deactivateCheckBox);
            this.additionalSettingsGroupBox.Controls.Add(this.panel2);
            this.additionalSettingsGroupBox.Controls.Add(this.backlashPanel);
            this.additionalSettingsGroupBox.Controls.Add(this.panel1);
            this.additionalSettingsGroupBox.Controls.Add(this.stepperProfilePanel);
            this.additionalSettingsGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.additionalSettingsGroupBox.Location = new System.Drawing.Point(0, 228);
            this.additionalSettingsGroupBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.additionalSettingsGroupBox.Name = "additionalSettingsGroupBox";
            this.additionalSettingsGroupBox.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.additionalSettingsGroupBox.Size = new System.Drawing.Size(418, 249);
            this.additionalSettingsGroupBox.TabIndex = 4;
            this.additionalSettingsGroupBox.TabStop = false;
            this.additionalSettingsGroupBox.Text = "Additional settings";
            // 
            // deactivateCheckBox
            // 
            this.deactivateCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.deactivateCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.deactivateCheckBox.Location = new System.Drawing.Point(4, 207);
            this.deactivateCheckBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.deactivateCheckBox.Name = "deactivateCheckBox";
            this.deactivateCheckBox.Padding = new System.Windows.Forms.Padding(3, 0, 8, 0);
            this.deactivateCheckBox.Size = new System.Drawing.Size(410, 37);
            this.deactivateCheckBox.TabIndex = 18;
            this.deactivateCheckBox.Text = "Disable stepper when at target position";
            this.deactivateCheckBox.UseVisualStyleBackColor = true;
            this.deactivateCheckBox.CheckedChanged += new System.EventHandler(this.value_Changed);
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.DefaultAccelerationTextBox);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.DefaultSpeedTextBox);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(4, 133);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(410, 74);
            this.panel2.TabIndex = 24;
            // 
            // DefaultAccelerationTextBox
            // 
            this.DefaultAccelerationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DefaultAccelerationTextBox.Location = new System.Drawing.Point(298, 43);
            this.DefaultAccelerationTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.DefaultAccelerationTextBox.Name = "DefaultAccelerationTextBox";
            this.DefaultAccelerationTextBox.ReadOnly = true;
            this.DefaultAccelerationTextBox.Size = new System.Drawing.Size(103, 26);
            this.DefaultAccelerationTextBox.TabIndex = 23;
            this.DefaultAccelerationTextBox.TextChanged += new System.EventHandler(this.value_Changed);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(4, 43);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(285, 31);
            this.label3.TabIndex = 22;
            this.label3.Text = "Default acceleration";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DefaultSpeedTextBox
            // 
            this.DefaultSpeedTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DefaultSpeedTextBox.Location = new System.Drawing.Point(298, 5);
            this.DefaultSpeedTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.DefaultSpeedTextBox.Name = "DefaultSpeedTextBox";
            this.DefaultSpeedTextBox.ReadOnly = true;
            this.DefaultSpeedTextBox.Size = new System.Drawing.Size(103, 26);
            this.DefaultSpeedTextBox.TabIndex = 21;
            this.DefaultSpeedTextBox.TextChanged += new System.EventHandler(this.value_Changed);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(4, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(285, 31);
            this.label1.TabIndex = 19;
            this.label1.Text = "Default speed";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // backlashPanel
            // 
            this.backlashPanel.AutoSize = true;
            this.backlashPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.backlashPanel.Controls.Add(this.backlashValueLabel);
            this.backlashPanel.Controls.Add(this.BacklashTextBox);
            this.backlashPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.backlashPanel.Location = new System.Drawing.Point(4, 97);
            this.backlashPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.backlashPanel.Name = "backlashPanel";
            this.backlashPanel.Size = new System.Drawing.Size(410, 36);
            this.backlashPanel.TabIndex = 22;
            // 
            // backlashValueLabel
            // 
            this.backlashValueLabel.Location = new System.Drawing.Point(4, 5);
            this.backlashValueLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.backlashValueLabel.Name = "backlashValueLabel";
            this.backlashValueLabel.Size = new System.Drawing.Size(120, 31);
            this.backlashValueLabel.TabIndex = 20;
            this.backlashValueLabel.Text = "Backlash value";
            this.backlashValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // BacklashTextBox
            // 
            this.BacklashTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BacklashTextBox.Location = new System.Drawing.Point(298, 5);
            this.BacklashTextBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.BacklashTextBox.Name = "BacklashTextBox";
            this.BacklashTextBox.ReadOnly = true;
            this.BacklashTextBox.Size = new System.Drawing.Size(103, 26);
            this.BacklashTextBox.TabIndex = 1;
            this.BacklashTextBox.TextChanged += new System.EventHandler(this.value_Changed);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.ModeComboBox);
            this.panel1.Controls.Add(this.stepperModeLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(4, 61);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(410, 36);
            this.panel1.TabIndex = 21;
            // 
            // ModeComboBox
            // 
            this.ModeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ModeComboBox.Enabled = false;
            this.ModeComboBox.FormattingEnabled = true;
            this.ModeComboBox.Location = new System.Drawing.Point(176, 3);
            this.ModeComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ModeComboBox.MaxLength = 2;
            this.ModeComboBox.Name = "ModeComboBox";
            this.ModeComboBox.Size = new System.Drawing.Size(228, 28);
            this.ModeComboBox.TabIndex = 18;
            this.ModeComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // stepperModeLabel
            // 
            this.stepperModeLabel.Location = new System.Drawing.Point(4, 5);
            this.stepperModeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.stepperModeLabel.Name = "stepperModeLabel";
            this.stepperModeLabel.Size = new System.Drawing.Size(86, 31);
            this.stepperModeLabel.TabIndex = 19;
            this.stepperModeLabel.Text = "Mode";
            this.stepperModeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // stepperProfilePanel
            // 
            this.stepperProfilePanel.AutoSize = true;
            this.stepperProfilePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stepperProfilePanel.Controls.Add(this.PresetComboBox);
            this.stepperProfilePanel.Controls.Add(this.stepperProfileLabel);
            this.stepperProfilePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.stepperProfilePanel.Location = new System.Drawing.Point(4, 24);
            this.stepperProfilePanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.stepperProfilePanel.Name = "stepperProfilePanel";
            this.stepperProfilePanel.Size = new System.Drawing.Size(410, 37);
            this.stepperProfilePanel.TabIndex = 23;
            // 
            // PresetComboBox
            // 
            this.PresetComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PresetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PresetComboBox.FormattingEnabled = true;
            this.PresetComboBox.Items.AddRange(new object[] {
            "Custom",
            "28BYJ Half-Step (ULN2003)",
            "x.27 Half-Step (direct)"});
            this.PresetComboBox.Location = new System.Drawing.Point(176, 3);
            this.PresetComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.PresetComboBox.MaxLength = 2;
            this.PresetComboBox.Name = "PresetComboBox";
            this.PresetComboBox.Size = new System.Drawing.Size(228, 28);
            this.PresetComboBox.TabIndex = 20;
            this.PresetComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // stepperProfileLabel
            // 
            this.stepperProfileLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stepperProfileLabel.Location = new System.Drawing.Point(4, 5);
            this.stepperProfileLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.stepperProfileLabel.Name = "stepperProfileLabel";
            this.stepperProfileLabel.Size = new System.Drawing.Size(162, 32);
            this.stepperProfileLabel.TabIndex = 21;
            this.stepperProfileLabel.Text = "Select a preset";
            this.stepperProfileLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MFStepperPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.additionalSettingsGroupBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.autoZeroPinGroupBox);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MFStepperPanel";
            this.Size = new System.Drawing.Size(418, 595);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.autoZeroPinGroupBox.ResumeLayout(false);
            this.autoZeroPinGroupBox.PerformLayout();
            this.additionalSettingsGroupBox.ResumeLayout(false);
            this.additionalSettingsGroupBox.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.backlashPanel.ResumeLayout(false);
            this.backlashPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.stepperProfilePanel.ResumeLayout(false);
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
        private System.Windows.Forms.TextBox DefaultAccelerationTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox DefaultSpeedTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel stepperProfilePanel;
        private System.Windows.Forms.ComboBox PresetComboBox;
        private System.Windows.Forms.Label stepperProfileLabel;
    }
}
