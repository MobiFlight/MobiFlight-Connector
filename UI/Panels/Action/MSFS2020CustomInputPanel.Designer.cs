namespace MobiFlight.UI.Panels.Action
{
    partial class MSFS2020CustomInputPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MSFS2020CustomInputPanel));
            this.CommandTextBox = new System.Windows.Forms.TextBox();
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.HintLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // CommandTextBox
            // 
            resources.ApplyResources(this.CommandTextBox, "CommandTextBox");
            this.CommandTextBox.Name = "CommandTextBox";
            // 
            // DescriptionLabel
            // 
            resources.ApplyResources(this.DescriptionLabel, "DescriptionLabel");
            this.DescriptionLabel.Name = "DescriptionLabel";
            // 
            // HintLabel
            // 
            resources.ApplyResources(this.HintLabel, "HintLabel");
            this.HintLabel.Name = "HintLabel";
            // 
            // MSFS2020CustomInputPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.HintLabel);
            this.Controls.Add(this.DescriptionLabel);
            this.Controls.Add(this.CommandTextBox);
            this.Name = "MSFS2020CustomInputPanel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox CommandTextBox;
        private System.Windows.Forms.Label DescriptionLabel;
        private System.Windows.Forms.Label HintLabel;
    }
}
