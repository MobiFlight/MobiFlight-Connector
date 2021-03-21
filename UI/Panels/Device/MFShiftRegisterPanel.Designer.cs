
namespace MobiFlight.UI.Panels.Settings
{
    partial class MFShiftRegisterPanel
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MFShiftRegisterPanel));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mfPinPWMComboBox = new System.Windows.Forms.ComboBox();
            this.pwmLabel = new System.Windows.Forms.Label();
            this.mfNumModulesComboBox = new System.Windows.Forms.ComboBox();
            this.numberOfModulesLabel = new System.Windows.Forms.Label();
            this.mfPin1Label = new System.Windows.Forms.Label();
            this.mfPin1ComboBox = new System.Windows.Forms.ComboBox();
            this.mfPin3Label = new System.Windows.Forms.Label();
            this.mfPin3ComboBox = new System.Windows.Forms.ComboBox();
            this.mfPin2Label = new System.Windows.Forms.Label();
            this.mfPin2ComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.mfPinPWMComboBox);
            this.groupBox1.Controls.Add(this.pwmLabel);
            this.groupBox1.Controls.Add(this.mfNumModulesComboBox);
            this.groupBox1.Controls.Add(this.numberOfModulesLabel);
            this.groupBox1.Controls.Add(this.mfPin1Label);
            this.groupBox1.Controls.Add(this.mfPin1ComboBox);
            this.groupBox1.Controls.Add(this.mfPin3Label);
            this.groupBox1.Controls.Add(this.mfPin3ComboBox);
            this.groupBox1.Controls.Add(this.mfPin2Label);
            this.groupBox1.Controls.Add(this.mfPin2ComboBox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(340, 220);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pin and number of modules";
            // 
            // mfPinPWMComboBox
            // 
            this.mfPinPWMComboBox.FormattingEnabled = true;
            this.mfPinPWMComboBox.Location = new System.Drawing.Point(187, 47);
            this.mfPinPWMComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.mfPinPWMComboBox.Name = "mfPinPWMComboBox";
            this.mfPinPWMComboBox.Size = new System.Drawing.Size(66, 24);
            this.mfPinPWMComboBox.TabIndex = 23;
            this.mfPinPWMComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            this.mfPinPWMComboBox.SelectedValueChanged += new System.EventHandler(this.value_Changed);
            // 
            // pwmLabel
            // 
            this.pwmLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pwmLabel.Location = new System.Drawing.Point(184, 26);
            this.pwmLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.pwmLabel.Name = "pwmLabel";
            this.pwmLabel.Size = new System.Drawing.Size(113, 17);
            this.pwmLabel.TabIndex = 22;
            this.pwmLabel.Text = "PWM (optional)";
            // 
            // mfNumModulesComboBox
            // 
            this.mfNumModulesComboBox.FormattingEnabled = true;
            this.mfNumModulesComboBox.Location = new System.Drawing.Point(12, 107);
            this.mfNumModulesComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.mfNumModulesComboBox.Name = "mfNumModulesComboBox";
            this.mfNumModulesComboBox.Size = new System.Drawing.Size(110, 24);
            this.mfNumModulesComboBox.TabIndex = 21;
            this.mfNumModulesComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            this.mfNumModulesComboBox.SelectedValueChanged += new System.EventHandler(this.value_Changed);
            // 
            // numberOfModulesLabel
            // 
            this.numberOfModulesLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.numberOfModulesLabel.Location = new System.Drawing.Point(9, 86);
            this.numberOfModulesLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.numberOfModulesLabel.Name = "numberOfModulesLabel";
            this.numberOfModulesLabel.Size = new System.Drawing.Size(169, 17);
            this.numberOfModulesLabel.TabIndex = 19;
            this.numberOfModulesLabel.Text = "# of 8 bit registers";
            // 
            // mfPin1Label
            // 
            this.mfPin1Label.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.mfPin1Label.Location = new System.Drawing.Point(8, 26);
            this.mfPin1Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mfPin1Label.Name = "mfPin1Label";
            this.mfPin1Label.Size = new System.Drawing.Size(53, 17);
            this.mfPin1Label.TabIndex = 16;
            this.mfPin1Label.Text = "Latch";
            // 
            // mfPin1ComboBox
            // 
            this.mfPin1ComboBox.FormattingEnabled = true;
            this.mfPin1ComboBox.Location = new System.Drawing.Point(12, 47);
            this.mfPin1ComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.mfPin1ComboBox.Name = "mfPin1ComboBox";
            this.mfPin1ComboBox.Size = new System.Drawing.Size(49, 24);
            this.mfPin1ComboBox.TabIndex = 15;
            this.mfPin1ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            this.mfPin1ComboBox.SelectedValueChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfPin3Label
            // 
            this.mfPin3Label.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.mfPin3Label.Location = new System.Drawing.Point(125, 26);
            this.mfPin3Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mfPin3Label.Name = "mfPin3Label";
            this.mfPin3Label.Size = new System.Drawing.Size(53, 14);
            this.mfPin3Label.TabIndex = 18;
            this.mfPin3Label.Text = "Data";
            // 
            // mfPin3ComboBox
            // 
            this.mfPin3ComboBox.FormattingEnabled = true;
            this.mfPin3ComboBox.Location = new System.Drawing.Point(129, 47);
            this.mfPin3ComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.mfPin3ComboBox.Name = "mfPin3ComboBox";
            this.mfPin3ComboBox.Size = new System.Drawing.Size(49, 24);
            this.mfPin3ComboBox.TabIndex = 17;
            this.mfPin3ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            this.mfPin3ComboBox.SelectedValueChanged += new System.EventHandler(this.value_Changed);
            // 
            // mfPin2Label
            // 
            this.mfPin2Label.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.mfPin2Label.Location = new System.Drawing.Point(67, 26);
            this.mfPin2Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.mfPin2Label.Name = "mfPin2Label";
            this.mfPin2Label.Size = new System.Drawing.Size(55, 14);
            this.mfPin2Label.TabIndex = 16;
            this.mfPin2Label.Text = "Clock";
            // 
            // mfPin2ComboBox
            // 
            this.mfPin2ComboBox.FormattingEnabled = true;
            this.mfPin2ComboBox.Location = new System.Drawing.Point(71, 47);
            this.mfPin2ComboBox.Margin = new System.Windows.Forms.Padding(4);
            this.mfPin2ComboBox.Name = "mfPin2ComboBox";
            this.mfPin2ComboBox.Size = new System.Drawing.Size(49, 24);
            this.mfPin2ComboBox.TabIndex = 15;
            this.mfPin2ComboBox.SelectedIndexChanged += new System.EventHandler(this.value_Changed);
            this.mfPin2ComboBox.SelectedValueChanged += new System.EventHandler(this.value_Changed);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 220);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(340, 59);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Name";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 29);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(241, 22);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.value_Changed);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(8, 139);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(325, 74);
            this.textBox2.TabIndex = 24;
            this.textBox2.Text = resources.GetString("textBox2.Text");
            // 
            // MFShiftRegisterPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "MFShiftRegisterPanel";
            this.Size = new System.Drawing.Size(340, 280);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox mfNumModulesComboBox;
        private System.Windows.Forms.Label numberOfModulesLabel;
        private System.Windows.Forms.Label mfPin1Label;
        private System.Windows.Forms.ComboBox mfPin1ComboBox;
        private System.Windows.Forms.Label mfPin3Label;
        private System.Windows.Forms.ComboBox mfPin3ComboBox;
        private System.Windows.Forms.Label mfPin2Label;
        private System.Windows.Forms.ComboBox mfPin2ComboBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox mfPinPWMComboBox;
        private System.Windows.Forms.Label pwmLabel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox textBox2;
    }
}
