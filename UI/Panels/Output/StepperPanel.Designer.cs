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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StepperPanel));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CompassModeCheckBox = new System.Windows.Forms.CheckBox();
            this.stepperTestValueTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.outputRevTextBox = new System.Windows.Forms.TextBox();
            this.inputRevTextBox = new System.Windows.Forms.TextBox();
            this.displayPinComoBoxLabel = new System.Windows.Forms.Label();
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
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.CompassModeCheckBox);
            this.groupBox1.Controls.Add(this.stepperTestValueTextBox);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.outputRevTextBox);
            this.groupBox1.Controls.Add(this.inputRevTextBox);
            this.groupBox1.Controls.Add(this.displayPinComoBoxLabel);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // CompassModeCheckBox
            // 
            resources.ApplyResources(this.CompassModeCheckBox, "CompassModeCheckBox");
            this.CompassModeCheckBox.Name = "CompassModeCheckBox";
            this.CompassModeCheckBox.UseVisualStyleBackColor = true;
            this.CompassModeCheckBox.CheckedChanged += new System.EventHandler(this.CompassModeCheckBox_CheckedChanged);
            // 
            // stepperTestValueTextBox
            // 
            resources.ApplyResources(this.stepperTestValueTextBox, "stepperTestValueTextBox");
            this.stepperTestValueTextBox.Name = "stepperTestValueTextBox";
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
            // StepperPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Name = "StepperPanel";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
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
    }
}
