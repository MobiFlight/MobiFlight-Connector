namespace MobiFlight.UI.Panels.Action
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VJoyInputPanel));
            this.fsuipcLoadPresetGroupBox = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
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
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.label5);
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
            resources.ApplyResources(this.fsuipcLoadPresetGroupBox, "fsuipcLoadPresetGroupBox");
            this.fsuipcLoadPresetGroupBox.Name = "fsuipcLoadPresetGroupBox";
            this.fsuipcLoadPresetGroupBox.TabStop = false;
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
            // textBoxValue
            // 
            resources.ApplyResources(this.textBoxValue, "textBoxValue");
            this.textBoxValue.Name = "textBoxValue";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // radioButtonOff
            // 
            resources.ApplyResources(this.radioButtonOff, "radioButtonOff");
            this.radioButtonOff.Name = "radioButtonOff";
            this.radioButtonOff.TabStop = true;
            this.radioButtonOff.UseVisualStyleBackColor = true;
            // 
            // radioButtonOn
            // 
            resources.ApplyResources(this.radioButtonOn, "radioButtonOn");
            this.radioButtonOn.Name = "radioButtonOn";
            this.radioButtonOn.TabStop = true;
            this.radioButtonOn.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // comboBoxAxis
            // 
            resources.ApplyResources(this.comboBoxAxis, "comboBoxAxis");
            this.comboBoxAxis.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBoxAxis.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxAxis.FormattingEnabled = true;
            this.comboBoxAxis.Name = "comboBoxAxis";
            this.comboBoxAxis.SelectedIndexChanged += new System.EventHandler(this.comboBoxAxis_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // comboBoxButtonNr
            // 
            resources.ApplyResources(this.comboBoxButtonNr, "comboBoxButtonNr");
            this.comboBoxButtonNr.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBoxButtonNr.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBoxButtonNr.FormattingEnabled = true;
            this.comboBoxButtonNr.Name = "comboBoxButtonNr";
            this.comboBoxButtonNr.SelectedIndexChanged += new System.EventHandler(this.comboBoxButtonNr_SelectedIndexChanged);
            // 
            // hintLabel
            // 
            resources.ApplyResources(this.hintLabel, "hintLabel");
            this.hintLabel.Name = "hintLabel";
            // 
            // EventIdLabel
            // 
            resources.ApplyResources(this.EventIdLabel, "EventIdLabel");
            this.EventIdLabel.Name = "EventIdLabel";
            // 
            // ComboBoxID
            // 
            resources.ApplyResources(this.ComboBoxID, "ComboBoxID");
            this.ComboBoxID.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.ComboBoxID.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ComboBoxID.FormattingEnabled = true;
            this.ComboBoxID.Name = "ComboBoxID";
            this.ComboBoxID.SelectedIndexChanged += new System.EventHandler(this.ComboBoxID_SelectedIndexChanged);
            // 
            // VJoyInputPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fsuipcLoadPresetGroupBox);
            this.Name = "VJoyInputPanel";
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
        private System.Windows.Forms.Label label5;
    }
}
