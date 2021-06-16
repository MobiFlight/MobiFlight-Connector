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
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.autoZeroPinGroupBox.SuspendLayout();
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
            this.groupBox1.Size = new System.Drawing.Size(304, 112);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pin Settings Motor";
            // 
            // mfPin4Label
            // 
            this.mfPin4Label.Location = new System.Drawing.Point(222, 77);
            this.mfPin4Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mfPin4Label.Name = "mfPin4Label";
            this.mfPin4Label.Size = new System.Drawing.Size(52, 28);
            this.mfPin4Label.TabIndex = 20;
            this.mfPin4Label.Text = "Pin 4";
            // 
            // mfPin4ComboBox
            // 
            this.mfPin4ComboBox.FormattingEnabled = true;
            this.mfPin4ComboBox.Location = new System.Drawing.Point(159, 72);
            this.mfPin4ComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mfPin4ComboBox.MaxLength = 2;
            this.mfPin4ComboBox.Name = "mfPin4ComboBox";
            this.mfPin4ComboBox.Size = new System.Drawing.Size(60, 28);
            this.mfPin4ComboBox.TabIndex = 19;
            this.mfPin4ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfPin1Label
            // 
            this.mfPin1Label.Location = new System.Drawing.Point(90, 32);
            this.mfPin1Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mfPin1Label.Name = "mfPin1Label";
            this.mfPin1Label.Size = new System.Drawing.Size(46, 28);
            this.mfPin1Label.TabIndex = 16;
            this.mfPin1Label.Text = "Pin 1";
            // 
            // mfPin1ComboBox
            // 
            this.mfPin1ComboBox.FormattingEnabled = true;
            this.mfPin1ComboBox.Location = new System.Drawing.Point(27, 28);
            this.mfPin1ComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mfPin1ComboBox.MaxLength = 2;
            this.mfPin1ComboBox.Name = "mfPin1ComboBox";
            this.mfPin1ComboBox.Size = new System.Drawing.Size(60, 28);
            this.mfPin1ComboBox.TabIndex = 15;
            this.mfPin1ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfPin3Label
            // 
            this.mfPin3Label.Location = new System.Drawing.Point(90, 74);
            this.mfPin3Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mfPin3Label.Name = "mfPin3Label";
            this.mfPin3Label.Size = new System.Drawing.Size(52, 28);
            this.mfPin3Label.TabIndex = 18;
            this.mfPin3Label.Text = "Pin 3";
            // 
            // mfPin3ComboBox
            // 
            this.mfPin3ComboBox.FormattingEnabled = true;
            this.mfPin3ComboBox.Location = new System.Drawing.Point(27, 69);
            this.mfPin3ComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mfPin3ComboBox.MaxLength = 2;
            this.mfPin3ComboBox.Name = "mfPin3ComboBox";
            this.mfPin3ComboBox.Size = new System.Drawing.Size(60, 28);
            this.mfPin3ComboBox.TabIndex = 17;
            this.mfPin3ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfPin2Label
            // 
            this.mfPin2Label.Location = new System.Drawing.Point(222, 32);
            this.mfPin2Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mfPin2Label.Name = "mfPin2Label";
            this.mfPin2Label.Size = new System.Drawing.Size(46, 28);
            this.mfPin2Label.TabIndex = 16;
            this.mfPin2Label.Text = "Pin 2";
            // 
            // mfPin2ComboBox
            // 
            this.mfPin2ComboBox.FormattingEnabled = true;
            this.mfPin2ComboBox.Location = new System.Drawing.Point(159, 28);
            this.mfPin2ComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mfPin2ComboBox.MaxLength = 2;
            this.mfPin2ComboBox.Name = "mfPin2ComboBox";
            this.mfPin2ComboBox.Size = new System.Drawing.Size(60, 28);
            this.mfPin2ComboBox.TabIndex = 15;
            this.mfPin2ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.mfNameTextBox);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 181);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(304, 74);
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
            this.autoZeroPinGroupBox.Location = new System.Drawing.Point(0, 112);
            this.autoZeroPinGroupBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.autoZeroPinGroupBox.Name = "autoZeroPinGroupBox";
            this.autoZeroPinGroupBox.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.autoZeroPinGroupBox.Size = new System.Drawing.Size(304, 69);
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
            this.label2.Location = new System.Drawing.Point(222, 25);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 28);
            this.label2.TabIndex = 16;
            this.label2.Text = "Pin";
            // 
            // mfBtnPinComboBox
            // 
            this.mfBtnPinComboBox.FormattingEnabled = true;
            this.mfBtnPinComboBox.Location = new System.Drawing.Point(159, 20);
            this.mfBtnPinComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mfBtnPinComboBox.MaxLength = 2;
            this.mfBtnPinComboBox.Name = "mfBtnPinComboBox";
            this.mfBtnPinComboBox.Size = new System.Drawing.Size(60, 28);
            this.mfBtnPinComboBox.TabIndex = 15;
            this.mfBtnPinComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            // 
            // MFStepperPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.autoZeroPinGroupBox);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MFStepperPanel";
            this.Size = new System.Drawing.Size(304, 295);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.autoZeroPinGroupBox.ResumeLayout(false);
            this.autoZeroPinGroupBox.PerformLayout();
            this.ResumeLayout(false);

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
    }
}
