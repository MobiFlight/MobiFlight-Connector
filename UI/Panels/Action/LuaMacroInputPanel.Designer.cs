namespace MobiFlight.UI.Panels.Action
{
    partial class LuaMacroInputPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LuaMacroInputPanel));
            this.fsuipcLoadPresetGroupBox = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.MacroValueTextBox = new System.Windows.Forms.TextBox();
            this.MacroNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.fsuipcLoadPresetGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // fsuipcLoadPresetGroupBox
            // 
            resources.ApplyResources(this.fsuipcLoadPresetGroupBox, "fsuipcLoadPresetGroupBox");
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.label5);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.label2);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.MacroValueTextBox);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.MacroNameTextBox);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.label1);
            this.fsuipcLoadPresetGroupBox.Name = "fsuipcLoadPresetGroupBox";
            this.fsuipcLoadPresetGroupBox.TabStop = false;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // MacroValueTextBox
            // 
            resources.ApplyResources(this.MacroValueTextBox, "MacroValueTextBox");
            this.MacroValueTextBox.Name = "MacroValueTextBox";
            // 
            // MacroNameTextBox
            // 
            resources.ApplyResources(this.MacroNameTextBox, "MacroNameTextBox");
            this.MacroNameTextBox.Name = "MacroNameTextBox";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // LuaMacroInputPanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fsuipcLoadPresetGroupBox);
            this.Name = "LuaMacroInputPanel";
            this.fsuipcLoadPresetGroupBox.ResumeLayout(false);
            this.fsuipcLoadPresetGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox fsuipcLoadPresetGroupBox;
        private System.Windows.Forms.TextBox MacroNameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox MacroValueTextBox;
        private System.Windows.Forms.Label label5;
    }
}
