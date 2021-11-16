
namespace MobiFlight.UI.Panels.Config
{
    partial class TransformOptionsGroup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransformOptionsGroup));
            this.fsuipcMoreOptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.ValuePanel = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.fsuipcValueTextBox = new System.Windows.Forms.TextBox();
            this.SubstringPanel = new System.Windows.Forms.Panel();
            this.SubstringTransformationCheckBox = new System.Windows.Forms.CheckBox();
            this.SubStringToTextBox = new System.Windows.Forms.TextBox();
            this.SubStringToLabel = new System.Windows.Forms.Label();
            this.SubStringFromTextBox = new System.Windows.Forms.TextBox();
            this.MultiplyPanel = new System.Windows.Forms.Panel();
            this.TransformationCheckBox = new System.Windows.Forms.CheckBox();
            this.TransformTextBox = new System.Windows.Forms.TextBox();
            this.fsuipcMoreOptionsGroupBox.SuspendLayout();
            this.ValuePanel.SuspendLayout();
            this.SubstringPanel.SuspendLayout();
            this.MultiplyPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // fsuipcMoreOptionsGroupBox
            // 
            resources.ApplyResources(this.fsuipcMoreOptionsGroupBox, "fsuipcMoreOptionsGroupBox");
            this.fsuipcMoreOptionsGroupBox.Controls.Add(this.ValuePanel);
            this.fsuipcMoreOptionsGroupBox.Controls.Add(this.SubstringPanel);
            this.fsuipcMoreOptionsGroupBox.Controls.Add(this.MultiplyPanel);
            this.fsuipcMoreOptionsGroupBox.Name = "fsuipcMoreOptionsGroupBox";
            this.fsuipcMoreOptionsGroupBox.TabStop = false;
            // 
            // valuePanel
            // 
            resources.ApplyResources(this.ValuePanel, "valuePanel");
            this.ValuePanel.Controls.Add(this.label5);
            this.ValuePanel.Controls.Add(this.fsuipcValueTextBox);
            this.ValuePanel.Name = "valuePanel";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // fsuipcValueTextBox
            // 
            resources.ApplyResources(this.fsuipcValueTextBox, "fsuipcValueTextBox");
            this.fsuipcValueTextBox.Name = "fsuipcValueTextBox";
            // 
            // SubstringPanel
            // 
            resources.ApplyResources(this.SubstringPanel, "SubstringPanel");
            this.SubstringPanel.Controls.Add(this.SubstringTransformationCheckBox);
            this.SubstringPanel.Controls.Add(this.SubStringToTextBox);
            this.SubstringPanel.Controls.Add(this.SubStringToLabel);
            this.SubstringPanel.Controls.Add(this.SubStringFromTextBox);
            this.SubstringPanel.Name = "SubstringPanel";
            // 
            // SubstringTransformationCheckBox
            // 
            this.SubstringTransformationCheckBox.Checked = true;
            this.SubstringTransformationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.SubstringTransformationCheckBox, "SubstringTransformationCheckBox");
            this.SubstringTransformationCheckBox.Name = "SubstringTransformationCheckBox";
            this.SubstringTransformationCheckBox.UseVisualStyleBackColor = true;
            // 
            // SubStringToTextBox
            // 
            resources.ApplyResources(this.SubStringToTextBox, "SubStringToTextBox");
            this.SubStringToTextBox.Name = "SubStringToTextBox";
            // 
            // SubStringToLabel
            // 
            resources.ApplyResources(this.SubStringToLabel, "SubStringToLabel");
            this.SubStringToLabel.Name = "SubStringToLabel";
            // 
            // SubStringFromTextBox
            // 
            resources.ApplyResources(this.SubStringFromTextBox, "SubStringFromTextBox");
            this.SubStringFromTextBox.Name = "SubStringFromTextBox";
            // 
            // multiplyPanel
            // 
            resources.ApplyResources(this.MultiplyPanel, "multiplyPanel");
            this.MultiplyPanel.Controls.Add(this.TransformationCheckBox);
            this.MultiplyPanel.Controls.Add(this.TransformTextBox);
            this.MultiplyPanel.Name = "multiplyPanel";
            // 
            // TransformationCheckBox
            // 
            this.TransformationCheckBox.Checked = true;
            this.TransformationCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.TransformationCheckBox, "TransformationCheckBox");
            this.TransformationCheckBox.Name = "TransformationCheckBox";
            this.TransformationCheckBox.UseVisualStyleBackColor = true;
            this.TransformationCheckBox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // fsuipcMultiplyTextBox
            // 
            resources.ApplyResources(this.TransformTextBox, "fsuipcMultiplyTextBox");
            this.TransformTextBox.Name = "fsuipcMultiplyTextBox";
            // 
            // TransformOptionsGroup
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fsuipcMoreOptionsGroupBox);
            this.Name = "TransformOptionsGroup";
            this.fsuipcMoreOptionsGroupBox.ResumeLayout(false);
            this.fsuipcMoreOptionsGroupBox.PerformLayout();
            this.ValuePanel.ResumeLayout(false);
            this.ValuePanel.PerformLayout();
            this.SubstringPanel.ResumeLayout(false);
            this.SubstringPanel.PerformLayout();
            this.MultiplyPanel.ResumeLayout(false);
            this.MultiplyPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox fsuipcMoreOptionsGroupBox;
        private System.Windows.Forms.Panel ValuePanel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox fsuipcValueTextBox;
        private System.Windows.Forms.Panel SubstringPanel;
        private System.Windows.Forms.CheckBox SubstringTransformationCheckBox;
        private System.Windows.Forms.TextBox SubStringToTextBox;
        private System.Windows.Forms.Label SubStringToLabel;
        private System.Windows.Forms.TextBox SubStringFromTextBox;
        private System.Windows.Forms.Panel MultiplyPanel;
        private System.Windows.Forms.CheckBox TransformationCheckBox;
        private System.Windows.Forms.TextBox TransformTextBox;
    }
}
