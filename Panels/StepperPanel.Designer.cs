namespace MobiFlight.Panels
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
            this.displayPinComoBoxLabel = new System.Windows.Forms.Label();
            this.inputRevTextBox = new System.Windows.Forms.TextBox();
            this.outputRevTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.stepperAddressesComboBox = new System.Windows.Forms.ComboBox();
            this.ManualCalibrateLabel = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // displayPinComoBoxLabel
            // 
            this.displayPinComoBoxLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.displayPinComoBoxLabel.Location = new System.Drawing.Point(6, 27);
            this.displayPinComoBoxLabel.Name = "displayPinComoBoxLabel";
            this.displayPinComoBoxLabel.Size = new System.Drawing.Size(82, 13);
            this.displayPinComoBoxLabel.TabIndex = 1;
            this.displayPinComoBoxLabel.Text = "Input Rev";
            this.displayPinComoBoxLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // inputRevTextBox
            // 
            this.inputRevTextBox.Location = new System.Drawing.Point(94, 24);
            this.inputRevTextBox.Name = "inputRevTextBox";
            this.inputRevTextBox.Size = new System.Drawing.Size(73, 20);
            this.inputRevTextBox.TabIndex = 2;
            this.inputRevTextBox.Text = "0";
            // 
            // outputRevTextBox
            // 
            this.outputRevTextBox.Location = new System.Drawing.Point(94, 46);
            this.outputRevTextBox.Name = "outputRevTextBox";
            this.outputRevTextBox.Size = new System.Drawing.Size(73, 20);
            this.outputRevTextBox.TabIndex = 4;
            this.outputRevTextBox.Text = "255";
            this.outputRevTextBox.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(6, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Output rev";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(6, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Stepper";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // stepperAddressesComboBox
            // 
            this.stepperAddressesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.stepperAddressesComboBox.FormattingEnabled = true;
            this.stepperAddressesComboBox.Items.AddRange(new object[] {
            "Pin",
            "7-Segment",
            "3BCD-8Bit-with-Strobe"});
            this.stepperAddressesComboBox.Location = new System.Drawing.Point(94, 0);
            this.stepperAddressesComboBox.MaximumSize = new System.Drawing.Size(122, 0);
            this.stepperAddressesComboBox.MinimumSize = new System.Drawing.Size(47, 0);
            this.stepperAddressesComboBox.Name = "stepperAddressesComboBox";
            this.stepperAddressesComboBox.Size = new System.Drawing.Size(122, 21);
            this.stepperAddressesComboBox.TabIndex = 9;
            // 
            // ManualCalibrateLabel
            // 
            this.ManualCalibrateLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ManualCalibrateLabel.Location = new System.Drawing.Point(6, 71);
            this.ManualCalibrateLabel.Name = "ManualCalibrateLabel";
            this.ManualCalibrateLabel.Size = new System.Drawing.Size(82, 13);
            this.ManualCalibrateLabel.TabIndex = 10;
            this.ManualCalibrateLabel.Text = "Man. Calibrate";
            this.ManualCalibrateLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // trackBar1
            // 
            this.trackBar1.BackColor = System.Drawing.SystemColors.Window;
            this.trackBar1.LargeChange = 1;
            this.trackBar1.Location = new System.Drawing.Point(94, 67);
            this.trackBar1.Maximum = 5;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(158, 45);
            this.trackBar1.TabIndex = 11;
            this.trackBar1.Value = 1;
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(91, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "-50";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(227, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(25, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "+50";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(117, 99);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(25, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "-10";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label6
            // 
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(148, 99);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(20, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "-1";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label7
            // 
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(169, 99);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(25, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "+1";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label8
            // 
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(199, 99);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(25, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "+10";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(93, 118);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 18;
            this.button1.Text = "Move";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(177, 118);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 19;
            this.button2.Text = "Set Zero";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // StepperPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.ManualCalibrateLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.stepperAddressesComboBox);
            this.Controls.Add(this.outputRevTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.inputRevTextBox);
            this.Controls.Add(this.displayPinComoBoxLabel);
            this.Name = "StepperPanel";
            this.Size = new System.Drawing.Size(255, 146);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label displayPinComoBoxLabel;
        public System.Windows.Forms.TextBox inputRevTextBox;
        public System.Windows.Forms.TextBox outputRevTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox stepperAddressesComboBox;
        private System.Windows.Forms.Label ManualCalibrateLabel;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}
