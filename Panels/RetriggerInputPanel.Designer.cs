namespace MobiFlight.Panels
{
    partial class RetriggerInputPanel
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
            this.label1 = new System.Windows.Forms.Label();
            this.fsuipcLoadPresetGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // fsuipcLoadPresetGroupBox
            // 
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.label1);
            this.fsuipcLoadPresetGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fsuipcLoadPresetGroupBox.Location = new System.Drawing.Point(0, 0);
            this.fsuipcLoadPresetGroupBox.Name = "fsuipcLoadPresetGroupBox";
            this.fsuipcLoadPresetGroupBox.Size = new System.Drawing.Size(211, 104);
            this.fsuipcLoadPresetGroupBox.TabIndex = 24;
            this.fsuipcLoadPresetGroupBox.TabStop = false;
            this.fsuipcLoadPresetGroupBox.Text = "Retrigger Buttons";
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(5, 28);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(201, 73);
            this.label1.TabIndex = 16;
            this.label1.Text = "Use this input action to re-trigger all button states.\r\n\r\nHelpful to sync your co" +
    "ckpit with your switches.";
            // 
            // RetriggerInputPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fsuipcLoadPresetGroupBox);
            this.Name = "RetriggerInputPanel";
            this.Size = new System.Drawing.Size(211, 104);
            this.fsuipcLoadPresetGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox fsuipcLoadPresetGroupBox;
        private System.Windows.Forms.Label label1;
    }
}
