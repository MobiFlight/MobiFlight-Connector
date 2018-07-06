namespace MobiFlight.Panels
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
            this.fsuipcLoadPresetGroupBox = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.MacroValueTextBox = new System.Windows.Forms.TextBox();
            this.MacroNameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.fsuipcLoadPresetGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // fsuipcLoadPresetGroupBox
            // 
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.label2);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.MacroValueTextBox);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.MacroNameTextBox);
            this.fsuipcLoadPresetGroupBox.Controls.Add(this.label1);
            this.fsuipcLoadPresetGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fsuipcLoadPresetGroupBox.Location = new System.Drawing.Point(0, 0);
            this.fsuipcLoadPresetGroupBox.Name = "fsuipcLoadPresetGroupBox";
            this.fsuipcLoadPresetGroupBox.Size = new System.Drawing.Size(211, 104);
            this.fsuipcLoadPresetGroupBox.TabIndex = 24;
            this.fsuipcLoadPresetGroupBox.TabStop = false;
            this.fsuipcLoadPresetGroupBox.Text = "FSUIPC Lua Macro Settings";
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(5, 54);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Value";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // MacroValueTextBox
            // 
            this.MacroValueTextBox.Location = new System.Drawing.Point(81, 51);
            this.MacroValueTextBox.Name = "MacroValueTextBox";
            this.MacroValueTextBox.Size = new System.Drawing.Size(124, 20);
            this.MacroValueTextBox.TabIndex = 18;
            // 
            // MacroNameTextBox
            // 
            this.MacroNameTextBox.Location = new System.Drawing.Point(81, 25);
            this.MacroNameTextBox.Name = "MacroNameTextBox";
            this.MacroNameTextBox.Size = new System.Drawing.Size(124, 20);
            this.MacroNameTextBox.TabIndex = 17;
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(5, 28);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Macro Name";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LuaMacroInputPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fsuipcLoadPresetGroupBox);
            this.Name = "LuaMacroInputPanel";
            this.Size = new System.Drawing.Size(211, 104);
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
    }
}
