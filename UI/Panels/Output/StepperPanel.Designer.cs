namespace MobiFlight.UI.Panels
{
    partial class StepperPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StepperPanel));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.CompassModeCheckBox = new System.Windows.Forms.CheckBox();
            this.inputRevTextBox = new System.Windows.Forms.TextBox();
            this.displayPinComoBoxLabel = new System.Windows.Forms.Label();
            this.stepperTestValueTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.outputRevTextBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.stepperAddressesComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.ManualCalibrateLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.SpeedResetButton = new System.Windows.Forms.Button();
            this.AccelerationResetButton = new System.Windows.Forms.Button();
            this.OutputRevResetButton = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.SpeedTextBox = new System.Windows.Forms.TextBox();
            this.AccelerationTextBox = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.CompassModeCheckBox);
            this.groupBox1.Controls.Add(this.inputRevTextBox);
            this.groupBox1.Controls.Add(this.displayPinComoBoxLabel);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            this.toolTip1.SetToolTip(this.label13, resources.GetString("label13.ToolTip"));
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // CompassModeCheckBox
            // 
            resources.ApplyResources(this.CompassModeCheckBox, "CompassModeCheckBox");
            this.CompassModeCheckBox.Name = "CompassModeCheckBox";
            this.CompassModeCheckBox.UseVisualStyleBackColor = true;
            // 
            // inputRevTextBox
            // 
            resources.ApplyResources(this.inputRevTextBox, "inputRevTextBox");
            this.inputRevTextBox.Name = "inputRevTextBox";
            this.inputRevTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.inputRevTextBox_Validating);
            // 
            // displayPinComoBoxLabel
            // 
            resources.ApplyResources(this.displayPinComoBoxLabel, "displayPinComoBoxLabel");
            this.displayPinComoBoxLabel.Name = "displayPinComoBoxLabel";
            this.toolTip1.SetToolTip(this.displayPinComoBoxLabel, resources.GetString("displayPinComoBoxLabel.ToolTip"));
            // 
            // stepperTestValueTextBox
            // 
            resources.ApplyResources(this.stepperTestValueTextBox, "stepperTestValueTextBox");
            this.stepperTestValueTextBox.Name = "stepperTestValueTextBox";
            this.stepperTestValueTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.inputRevTextBox_Validating);
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // outputRevTextBox
            // 
            resources.ApplyResources(this.outputRevTextBox, "outputRevTextBox");
            this.outputRevTextBox.Name = "outputRevTextBox";
            this.outputRevTextBox.TextChanged += new System.EventHandler(this.StepperSettingsTextBox_TextChanged);
            this.outputRevTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.inputRevTextBox_Validating);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.stepperAddressesComboBox);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // stepperAddressesComboBox
            // 
            this.stepperAddressesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.stepperAddressesComboBox.FormattingEnabled = true;
            this.stepperAddressesComboBox.Items.AddRange(new object[] {
            resources.GetString("stepperAddressesComboBox.Items"),
            resources.GetString("stepperAddressesComboBox.Items1"),
            resources.GetString("stepperAddressesComboBox.Items2")});
            resources.ApplyResources(this.stepperAddressesComboBox, "stepperAddressesComboBox");
            this.stepperAddressesComboBox.Name = "stepperAddressesComboBox";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.trackBar1);
            this.groupBox2.Controls.Add(this.ManualCalibrateLabel);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // trackBar1
            // 
            this.trackBar1.BackColor = System.Drawing.SystemColors.Window;
            this.trackBar1.LargeChange = 1;
            resources.ApplyResources(this.trackBar1, "trackBar1");
            this.trackBar1.Maximum = 5;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Value = 1;
            // 
            // ManualCalibrateLabel
            // 
            resources.ApplyResources(this.ManualCalibrateLabel, "ManualCalibrateLabel");
            this.ManualCalibrateLabel.Name = "ManualCalibrateLabel";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.SpeedResetButton);
            this.groupBox3.Controls.Add(this.AccelerationResetButton);
            this.groupBox3.Controls.Add(this.OutputRevResetButton);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.outputRevTextBox);
            this.groupBox3.Controls.Add(this.SpeedTextBox);
            this.groupBox3.Controls.Add(this.AccelerationTextBox);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // SpeedResetButton
            // 
            resources.ApplyResources(this.SpeedResetButton, "SpeedResetButton");
            this.SpeedResetButton.Name = "SpeedResetButton";
            this.SpeedResetButton.UseVisualStyleBackColor = true;
            this.SpeedResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // AccelerationResetButton
            // 
            resources.ApplyResources(this.AccelerationResetButton, "AccelerationResetButton");
            this.AccelerationResetButton.Name = "AccelerationResetButton";
            this.AccelerationResetButton.UseVisualStyleBackColor = true;
            this.AccelerationResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // OutputRevResetButton
            // 
            resources.ApplyResources(this.OutputRevResetButton, "OutputRevResetButton");
            this.OutputRevResetButton.Name = "OutputRevResetButton";
            this.OutputRevResetButton.UseVisualStyleBackColor = true;
            this.OutputRevResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            this.toolTip1.SetToolTip(this.label10, resources.GetString("label10.ToolTip"));
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            this.toolTip1.SetToolTip(this.label11, resources.GetString("label11.ToolTip"));
            // 
            // SpeedTextBox
            // 
            resources.ApplyResources(this.SpeedTextBox, "SpeedTextBox");
            this.SpeedTextBox.Name = "SpeedTextBox";
            this.SpeedTextBox.TextChanged += new System.EventHandler(this.StepperSettingsTextBox_TextChanged);
            this.SpeedTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.inputRevTextBox_Validating);
            // 
            // AccelerationTextBox
            // 
            resources.ApplyResources(this.AccelerationTextBox, "AccelerationTextBox");
            this.AccelerationTextBox.Name = "AccelerationTextBox";
            this.AccelerationTextBox.TextChanged += new System.EventHandler(this.StepperSettingsTextBox_TextChanged);
            this.AccelerationTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.inputRevTextBox_Validating);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.stepperTestValueTextBox);
            this.groupBox4.Controls.Add(this.label9);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // StepperPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Name = "StepperPanel";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.TextBox outputRevTextBox;
        public System.Windows.Forms.TextBox inputRevTextBox;
        private System.Windows.Forms.Label displayPinComoBoxLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox stepperAddressesComboBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label ManualCalibrateLabel;
        public System.Windows.Forms.TextBox stepperTestValueTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox CompassModeCheckBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label11;
        public System.Windows.Forms.TextBox AccelerationTextBox;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.TextBox SpeedTextBox;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button SpeedResetButton;
        private System.Windows.Forms.Button AccelerationResetButton;
        private System.Windows.Forms.Button OutputRevResetButton;
        private System.Windows.Forms.Label label13;
    }
}
