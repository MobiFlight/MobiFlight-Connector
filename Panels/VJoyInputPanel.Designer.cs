namespace MobiFlight.Panels
{
    partial class VJoyInputPanel
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
            this.fsuipcLoadPresetGroupBox = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxValue = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButtonOff = new System.Windows.Forms.RadioButton();
            this.radioButtonOn = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxAxis = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxButtonNr = new System.Windows.Forms.ComboBox();
            this.hintLabel = new System.Windows.Forms.Label();
            this.EventIdLabel = new System.Windows.Forms.Label();
            this.ComboBoxID = new System.Windows.Forms.ComboBox();
            this.fsuipcLoadPresetGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // fsuipcLoadPresetGroupBox
            // 
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.label4);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.textBoxValue);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.label3);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.radioButtonOff);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.radioButtonOn);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.label2);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.comboBoxAxis);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.label1);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.comboBoxButtonNr);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.hintLabel);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.EventIdLabel);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.ComboBoxID);
            this.fsuipcLoadPresetGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fsuipcLoadPresetGroupBox.Location = new System.Drawing.Point(0, 0);
            this.fsuipcLoadPresetGroupBox.Name = "fsuipcLoadPresetGroupBox";
            this.fsuipcLoadPresetGroupBox.Size = new System.Drawing.Size(211, 212);
            this.fsuipcLoadPresetGroupBox.TabIndex = 24;
            this.fsuipcLoadPresetGroupBox.TabStop = false;
            this.fsuipcLoadPresetGroupBox.Text = "vJoy virtual Joystick Settings";
            // 
            // label4
            // 
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(6, 105);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 15);
            this.label4.TabIndex = 27;
            this.label4.Text = "Button";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxValue
            // 
            this.textBoxValue.Location = new System.Drawing.Point(81, 130);
            this.textBoxValue.Name = "textBoxValue";
            this.textBoxValue.Size = new System.Drawing.Size(124, 20);
            this.textBoxValue.TabIndex = 26;
            this.textBoxValue.Text = "0";
            // 
            // label3
            // 
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(6, 132);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 15);
            this.label3.TabIndex = 25;
            this.label3.Text = "Value";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // radioButtonOff
            // 
            this.radioButtonOff.AutoSize = true;
            this.radioButtonOff.Location = new System.Drawing.Point(168, 103);
            this.radioButtonOff.Name = "radioButtonOff";
            this.radioButtonOff.Size = new System.Drawing.Size(37, 17);
            this.radioButtonOff.TabIndex = 24;
            this.radioButtonOff.TabStop = true;
            this.radioButtonOff.Text = "off";
            this.radioButtonOff.UseVisualStyleBackColor = true;
            // 
            // radioButtonOn
            // 
            this.radioButtonOn.AutoSize = true;
            this.radioButtonOn.Location = new System.Drawing.Point(125, 103);
            this.radioButtonOn.Name = "radioButtonOn";
            this.radioButtonOn.Size = new System.Drawing.Size(37, 17);
            this.radioButtonOn.TabIndex = 23;
            this.radioButtonOn.TabStop = true;
            this.radioButtonOn.Text = "on";
            this.radioButtonOn.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(6, 78);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 15);
            this.label2.TabIndex = 22;
            this.label2.Text = "Axis";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxAxis
            // 
            this.comboBoxAxis.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxAxis.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBoxAxis.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxAxis.FormattingEnabled = true;
            this.comboBoxAxis.Location = new System.Drawing.Point(81, 76);
            this.comboBoxAxis.Name = "comboBoxAxis";
            this.comboBoxAxis.Size = new System.Drawing.Size(124, 21);
            this.comboBoxAxis.TabIndex = 21;
            this.comboBoxAxis.SelectedIndexChanged += new System.EventHandler(this.comboBoxAxis_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(6, 52);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Button Nr";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxButtonNr
            // 
            this.comboBoxButtonNr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxButtonNr.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBoxButtonNr.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxButtonNr.FormattingEnabled = true;
            this.comboBoxButtonNr.Location = new System.Drawing.Point(81, 49);
            this.comboBoxButtonNr.Name = "comboBoxButtonNr";
            this.comboBoxButtonNr.Size = new System.Drawing.Size(124, 21);
            this.comboBoxButtonNr.TabIndex = 19;
            this.comboBoxButtonNr.SelectedIndexChanged += new System.EventHandler(this.comboBoxButtonNr_SelectedIndexChanged);
            // 
            // hintLabel
            // 
            this.hintLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hintLabel.Location = new System.Drawing.Point(0, 153);
            this.hintLabel.Name = "hintLabel";
            this.hintLabel.Size = new System.Drawing.Size(205, 18);
            this.hintLabel.TabIndex = 18;
            this.hintLabel.Text = "label2";
            // 
            // EventIdLabel
            // 
            this.EventIdLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.EventIdLabel.Location = new System.Drawing.Point(6, 25);
            this.EventIdLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.EventIdLabel.Name = "EventIdLabel";
            this.EventIdLabel.Size = new System.Drawing.Size(70, 13);
            this.EventIdLabel.TabIndex = 15;
            this.EventIdLabel.Text = "Joystick ID";
            this.EventIdLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ComboBoxID
            // 
            this.ComboBoxID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ComboBoxID.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.ComboBoxID.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ComboBoxID.FormattingEnabled = true;
            this.ComboBoxID.Location = new System.Drawing.Point(81, 22);
            this.ComboBoxID.Name = "ComboBoxID";
            this.ComboBoxID.Size = new System.Drawing.Size(124, 21);
            this.ComboBoxID.TabIndex = 13;
            this.ComboBoxID.SelectedIndexChanged += new System.EventHandler(this.ComboBoxID_SelectedIndexChanged);
            // 
            // VJoyInputPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fsuipcLoadPresetGroupBox);
            this.Name = "VJoyInputPanel";
            this.Size = new System.Drawing.Size(211, 212);
            this.fsuipcLoadPresetGroupBox.ResumeLayout(false);
            this.fsuipcLoadPresetGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox fsuipcLoadPresetGroupBox;
        private System.Windows.Forms.ComboBox ComboBoxID;
        private System.Windows.Forms.Label EventIdLabel;
        private System.Windows.Forms.Label hintLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxButtonNr;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxAxis;
        private System.Windows.Forms.TextBox textBoxValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioButtonOff;
        private System.Windows.Forms.RadioButton radioButtonOn;
        private System.Windows.Forms.Label label4;
    }
}
