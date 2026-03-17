
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
            this.ValuePlaceholderLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.fsuipcValueTextBox = new System.Windows.Forms.TextBox();
            this.fsuipcMoreOptionsGroupBox.SuspendLayout();
            this.ValuePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // fsuipcMoreOptionsGroupBox
            // 
            resources.ApplyResources(this.fsuipcMoreOptionsGroupBox, "fsuipcMoreOptionsGroupBox");
            this.fsuipcMoreOptionsGroupBox.Controls.Add(this.ValuePanel);
            this.fsuipcMoreOptionsGroupBox.Name = "fsuipcMoreOptionsGroupBox";
            this.fsuipcMoreOptionsGroupBox.TabStop = false;
            // 
            // ValuePanel
            // 
            resources.ApplyResources(this.ValuePanel, "ValuePanel");
            this.ValuePanel.Controls.Add(this.ValuePlaceholderLabel);
            this.ValuePanel.Controls.Add(this.label5);
            this.ValuePanel.Controls.Add(this.fsuipcValueTextBox);
            this.ValuePanel.Name = "ValuePanel";
            // 
            // ValuePlaceholderLabel
            // 
            resources.ApplyResources(this.ValuePlaceholderLabel, "ValuePlaceholderLabel");
            this.ValuePlaceholderLabel.Name = "ValuePlaceholderLabel";
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox fsuipcMoreOptionsGroupBox;
        private System.Windows.Forms.Panel ValuePanel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox fsuipcValueTextBox;
        private System.Windows.Forms.Label ValuePlaceholderLabel;
    }
}
