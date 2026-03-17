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
            this.DefaultAccelerationTextBox = new System.Windows.Forms.Label();
            this.DefaultSpeedTextBox = new System.Windows.Forms.Label();
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
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(279, 61);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pin Settings Motor";
            // 
            // mfPin4Label
            // 
            this.mfPin4Label.Location = new System.Drawing.Point(171, 17);
            this.mfPin4Label.Name = "mfPin4Label";
            this.mfPin4Label.Size = new System.Drawing.Size(31, 14);
            this.mfPin4Label.TabIndex = 20;
            this.mfPin4Label.Text = "Pin 4";
            // 
            // mfPin4ComboBox
            // 
            this.mfPin4ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mfPin4ComboBox.FormattingEnabled = true;
            this.mfPin4ComboBox.Location = new System.Drawing.Point(171, 33);
            this.mfPin4ComboBox.MaxLength = 2;
            this.mfPin4ComboBox.Name = "mfPin4ComboBox";
            this.mfPin4ComboBox.Size = new System.Drawing.Size(45, 21);
            this.mfPin4ComboBox.TabIndex = 19;
            this.mfPin4ComboBox.SelectedIndexChanged += new System.EventHandler(this.Value_Changed);
            // 
            // mfPin1Label
            // 
            this.mfPin1Label.Location = new System.Drawing.Point(15, 17);
            this.mfPin1Label.Name = "mfPin1Label";
            this.mfPin1Label.Size = new System.Drawing.Size(31, 14);
            this.mfPin1Label.TabIndex = 16;
            this.mfPin1Label.Text = "Pin 1";
            // 
            // mfPin1ComboBox
            // 
            this.mfPin1ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mfPin1ComboBox.FormattingEnabled = true;
            this.mfPin1ComboBox.Location = new System.Drawing.Point(18, 33);
            this.mfPin1ComboBox.MaxLength = 2;
            this.mfPin1ComboBox.Name = "mfPin1ComboBox";
            this.mfPin1ComboBox.Size = new System.Drawing.Size(45, 21);
            this.mfPin1ComboBox.TabIndex = 15;
            this.mfPin1ComboBox.SelectedIndexChanged += new System.EventHandler(this.Value_Changed);
            // 
            // mfPin3Label
            // 
            this.mfPin3Label.Location = new System.Drawing.Point(120, 17);
            this.mfPin3Label.Name = "mfPin3Label";
            this.mfPin3Label.Size = new System.Drawing.Size(31, 14);
            this.mfPin3Label.TabIndex = 18;
            this.mfPin3Label.Text = "Pin 3";
            // 
            // mfPin3ComboBox
            // 
            this.mfPin3ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mfPin3ComboBox.FormattingEnabled = true;
            this.mfPin3ComboBox.Location = new System.Drawing.Point(120, 33);
            this.mfPin3ComboBox.MaxLength = 2;
            this.mfPin3ComboBox.Name = "mfPin3ComboBox";
            this.mfPin3ComboBox.Size = new System.Drawing.Size(45, 21);
            this.mfPin3ComboBox.TabIndex = 17;
            this.mfPin3ComboBox.SelectedIndexChanged += new System.EventHandler(this.Value_Changed);
            // 
            // mfPin2Label
            // 
            this.mfPin2Label.Location = new System.Drawing.Point(69, 17);
            this.mfPin2Label.Name = "mfPin2Label";
            this.mfPin2Label.Size = new System.Drawing.Size(31, 14);
            this.mfPin2Label.TabIndex = 16;
            this.mfPin2Label.Text = "Pin 2";
            // 
            // mfPin2ComboBox
            // 
            this.mfPin2ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mfPin2ComboBox.FormattingEnabled = true;
            this.mfPin2ComboBox.Location = new System.Drawing.Point(69, 33);
            this.mfPin2ComboBox.MaxLength = 2;
            this.mfPin2ComboBox.Name = "mfPin2ComboBox";
            this.mfPin2ComboBox.Size = new System.Drawing.Size(45, 21);
            this.mfPin2ComboBox.TabIndex = 15;
            this.mfPin2ComboBox.SelectedIndexChanged += new System.EventHandler(this.Value_Changed);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.mfNameTextBox);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 100);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(279, 48);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Name";
            // 
            // mfNameTextBox
            // 
            this.mfNameTextBox.Location = new System.Drawing.Point(18, 19);
            this.mfNameTextBox.Name = "mfNameTextBox";
            this.mfNameTextBox.Size = new System.Drawing.Size(151, 20);
            this.mfNameTextBox.TabIndex = 0;
            this.mfNameTextBox.TextChanged += new System.EventHandler(this.Value_Changed);
            // 
            // autoZeroPinGroupBox
            // 
            this.autoZeroPinGroupBox.Controls.Add(this.autoZeroCheckBox);
            this.autoZeroPinGroupBox.Controls.Add(this.label2);
            this.autoZeroPinGroupBox.Controls.Add(this.mfBtnPinComboBox);
            this.autoZeroPinGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.autoZeroPinGroupBox.Location = new System.Drawing.Point(0, 61);
            this.autoZeroPinGroupBox.Name = "autoZeroPinGroupBox";
            this.autoZeroPinGroupBox.Size = new System.Drawing.Size(279, 39);
            this.autoZeroPinGroupBox.TabIndex = 3;
            this.autoZeroPinGroupBox.TabStop = false;
            this.autoZeroPinGroupBox.Text = "Auto Zero Input";
            // 
            // autoZeroCheckBox
            // 
            this.autoZeroCheckBox.AutoSize = true;
            this.autoZeroCheckBox.Location = new System.Drawing.Point(18, 16);
            this.autoZeroCheckBox.Name = "autoZeroCheckBox";
            this.autoZeroCheckBox.Size = new System.Drawing.Size(52, 17);
            this.autoZeroCheckBox.TabIndex = 17;
            this.autoZeroCheckBox.Text = "None";
            this.autoZeroCheckBox.UseVisualStyleBackColor = true;
            this.autoZeroCheckBox.CheckedChanged += new System.EventHandler(this.AutoZeroCheckBox_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(171, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 18);
            this.label2.TabIndex = 16;
            this.label2.Text = "Pin";
            // 
            // mfBtnPinComboBox
            // 
            this.mfBtnPinComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mfBtnPinComboBox.FormattingEnabled = true;
            this.mfBtnPinComboBox.Location = new System.Drawing.Point(120, 14);
            this.mfBtnPinComboBox.MaxLength = 2;
            this.mfBtnPinComboBox.Name = "mfBtnPinComboBox";
            this.mfBtnPinComboBox.Size = new System.Drawing.Size(45, 21);
            this.mfBtnPinComboBox.TabIndex = 15;
            this.mfBtnPinComboBox.SelectedIndexChanged += new System.EventHandler(this.Value_Changed);
            // 
            // additionalSettingsGroupBox
            // 
            this.additionalSettingsGroupBox.AutoSize = true;
            this.additionalSettingsGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.additionalSettingsGroupBox.Controls.Add(this.panel2);
            this.additionalSettingsGroupBox.Controls.Add(this.deactivateCheckBox);
            this.additionalSettingsGroupBox.Controls.Add(this.backlashPanel);
            this.additionalSettingsGroupBox.Controls.Add(this.panel1);
            this.additionalSettingsGroupBox.Controls.Add(this.stepperProfilePanel);
            this.additionalSettingsGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.additionalSettingsGroupBox.Location = new System.Drawing.Point(0, 148);
            this.additionalSettingsGroupBox.Name = "additionalSettingsGroupBox";
            this.additionalSettingsGroupBox.Size = new System.Drawing.Size(279, 169);
            this.additionalSettingsGroupBox.TabIndex = 4;
            this.additionalSettingsGroupBox.TabStop = false;
            this.additionalSettingsGroupBox.Text = "Additional settings";
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.DefaultSpeedTextBox);
            this.panel2.Controls.Add(this.DefaultAccelerationTextBox);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 118);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(273, 48);
            this.panel2.TabIndex = 24;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(3, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(190, 20);
            this.label3.TabIndex = 22;
            this.label3.Text = "Default acceleration";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.stepperPanelToolTip.SetToolTip(this.label3, "The acceleration value that is used as default in output configs.");
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(190, 20);
            this.label1.TabIndex = 19;
            this.label1.Text = "Default speed";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.stepperPanelToolTip.SetToolTip(this.label1, "The maximum speed that is as default value in output configs.");
            // 
            // deactivateCheckBox
            // 
            this.deactivateCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.deactivateCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.deactivateCheckBox.Location = new System.Drawing.Point(3, 94);
            this.deactivateCheckBox.Name = "deactivateCheckBox";
            this.deactivateCheckBox.Padding = new System.Windows.Forms.Padding(3, 0, 5, 0);
            this.deactivateCheckBox.Size = new System.Drawing.Size(273, 24);
            this.deactivateCheckBox.TabIndex = 18;
            this.deactivateCheckBox.Text = "Disable stepper when at target position";
            this.stepperPanelToolTip.SetToolTip(this.deactivateCheckBox, "Stop energizing the stepper once it has reached it target position.");
            this.deactivateCheckBox.UseVisualStyleBackColor = true;
            this.deactivateCheckBox.CheckedChanged += new System.EventHandler(this.Value_Changed);
            // 
            // backlashPanel
            // 
            this.backlashPanel.AutoSize = true;
            this.backlashPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.backlashPanel.Controls.Add(this.backlashValueLabel);
            this.backlashPanel.Controls.Add(this.BacklashTextBox);
            this.backlashPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.backlashPanel.Location = new System.Drawing.Point(3, 68);
            this.backlashPanel.Name = "backlashPanel";
            this.backlashPanel.Size = new System.Drawing.Size(273, 26);
            this.backlashPanel.TabIndex = 22;
            // 
            // backlashValueLabel
            // 
            this.backlashValueLabel.Location = new System.Drawing.Point(3, 3);
            this.backlashValueLabel.Name = "backlashValueLabel";
            this.backlashValueLabel.Size = new System.Drawing.Size(108, 20);
            this.backlashValueLabel.TabIndex = 20;
            this.backlashValueLabel.Text = "Backlash value";
            this.backlashValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.stepperPanelToolTip.SetToolTip(this.backlashValueLabel, "This value compensates for potential backlash of geared steppers.");
            // 
            // BacklashTextBox
            // 
            this.BacklashTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BacklashTextBox.Location = new System.Drawing.Point(199, 3);
            this.BacklashTextBox.Name = "BacklashTextBox";
            this.BacklashTextBox.Size = new System.Drawing.Size(70, 20);
            this.BacklashTextBox.TabIndex = 1;
            this.BacklashTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.BacklashTextBox.TextChanged += new System.EventHandler(this.Value_Changed);
            this.BacklashTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.BacklashTextBox_Validating);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.ModeComboBox);
            this.panel1.Controls.Add(this.stepperModeLabel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 42);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(273, 26);
            this.panel1.TabIndex = 21;
            // 
            // ModeComboBox
            // 
            this.ModeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ModeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ModeComboBox.FormattingEnabled = true;
            this.ModeComboBox.Location = new System.Drawing.Point(117, 2);
            this.ModeComboBox.MaxLength = 2;
            this.ModeComboBox.Name = "ModeComboBox";
            this.ModeComboBox.Size = new System.Drawing.Size(153, 21);
            this.ModeComboBox.TabIndex = 18;
            this.ModeComboBox.SelectedIndexChanged += new System.EventHandler(this.Value_Changed);
            // 
            // stepperModeLabel
            // 
            this.stepperModeLabel.Location = new System.Drawing.Point(3, 3);
            this.stepperModeLabel.Name = "stepperModeLabel";
            this.stepperModeLabel.Size = new System.Drawing.Size(108, 20);
            this.stepperModeLabel.TabIndex = 19;
            this.stepperModeLabel.Text = "Mode";
            this.stepperModeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.stepperPanelToolTip.SetToolTip(this.stepperModeLabel, "The stepper mode");
            // 
            // stepperProfilePanel
            // 
            this.stepperProfilePanel.AutoSize = true;
            this.stepperProfilePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.stepperProfilePanel.Controls.Add(this.PresetComboBox);
            this.stepperProfilePanel.Controls.Add(this.stepperProfileLabel);
            this.stepperProfilePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.stepperProfilePanel.Location = new System.Drawing.Point(3, 16);
            this.stepperProfilePanel.Name = "stepperProfilePanel";
            this.stepperProfilePanel.Size = new System.Drawing.Size(273, 26);
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
            this.PresetComboBox.Location = new System.Drawing.Point(117, 2);
            this.PresetComboBox.MaxLength = 2;
            this.PresetComboBox.Name = "PresetComboBox";
            this.PresetComboBox.Size = new System.Drawing.Size(153, 21);
            this.PresetComboBox.TabIndex = 20;
            this.PresetComboBox.SelectedIndexChanged += new System.EventHandler(this.Value_Changed);
            // 
            // stepperProfileLabel
            // 
            this.stepperProfileLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stepperProfileLabel.Location = new System.Drawing.Point(3, 3);
            this.stepperProfileLabel.Name = "stepperProfileLabel";
            this.stepperProfileLabel.Size = new System.Drawing.Size(108, 21);
            this.stepperProfileLabel.TabIndex = 21;
            this.stepperProfileLabel.Text = "Select a preset";
            this.stepperProfileLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.stepperPanelToolTip.SetToolTip(this.stepperProfileLabel, "Select a tested preset for common stepper models. Use custom preset for customiza" +
        "tion.");
            // 
            // DefaultAccelerationTextBox
            // 
            this.DefaultAccelerationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DefaultAccelerationTextBox.Location = new System.Drawing.Point(199, 28);
            this.DefaultAccelerationTextBox.Name = "DefaultAccelerationTextBox";
            this.DefaultAccelerationTextBox.Size = new System.Drawing.Size(70, 20);
            this.DefaultAccelerationTextBox.TabIndex = 23;
            this.DefaultAccelerationTextBox.Text = "####";
            this.DefaultAccelerationTextBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DefaultSpeedTextBox
            // 
            this.DefaultSpeedTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DefaultSpeedTextBox.Location = new System.Drawing.Point(199, 3);
            this.DefaultSpeedTextBox.Name = "DefaultSpeedTextBox";
            this.DefaultSpeedTextBox.Size = new System.Drawing.Size(70, 20);
            this.DefaultSpeedTextBox.TabIndex = 24;
            this.DefaultSpeedTextBox.Text = "####";
            this.DefaultSpeedTextBox.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // MFStepperPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.additionalSettingsGroupBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.autoZeroPinGroupBox);
            this.Controls.Add(this.groupBox1);
            this.Name = "MFStepperPanel";
            this.Size = new System.Drawing.Size(279, 387);
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
