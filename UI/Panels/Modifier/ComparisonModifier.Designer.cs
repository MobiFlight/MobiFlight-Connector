namespace MobiFlight.UI.Panels.Modifier
{
    partial class ComparisonModifier
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
            this.comparisonSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.comparisonSettingsPanel = new System.Windows.Forms.Panel();
            this.comparisonValueTextBox = new System.Windows.Forms.TextBox();
            this.comparisonElseValueTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.comparisonIfValueTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comparisonOperandComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comparisonActiveCheckBox = new System.Windows.Forms.CheckBox();
            this.comparisonSettingsGroupBox.SuspendLayout();
            this.comparisonSettingsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // comparisonSettingsGroupBox
            // 
            this.comparisonSettingsGroupBox.Controls.Add(this.comparisonSettingsPanel);
            this.comparisonSettingsGroupBox.Controls.Add(this.comparisonActiveCheckBox);
            this.comparisonSettingsGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.comparisonSettingsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.comparisonSettingsGroupBox.Name = "comparisonSettingsGroupBox";
            this.comparisonSettingsGroupBox.Padding = new System.Windows.Forms.Padding(6);
            this.comparisonSettingsGroupBox.Size = new System.Drawing.Size(294, 138);
            this.comparisonSettingsGroupBox.TabIndex = 9;
            this.comparisonSettingsGroupBox.TabStop = false;
            this.comparisonSettingsGroupBox.Text = "Comparison Settings";
            // 
            // comparisonSettingsPanel
            // 
            this.comparisonSettingsPanel.Controls.Add(this.comparisonValueTextBox);
            this.comparisonSettingsPanel.Controls.Add(this.comparisonElseValueTextBox);
            this.comparisonSettingsPanel.Controls.Add(this.label8);
            this.comparisonSettingsPanel.Controls.Add(this.comparisonIfValueTextBox);
            this.comparisonSettingsPanel.Controls.Add(this.label7);
            this.comparisonSettingsPanel.Controls.Add(this.comparisonOperandComboBox);
            this.comparisonSettingsPanel.Controls.Add(this.label6);
            this.comparisonSettingsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comparisonSettingsPanel.Location = new System.Drawing.Point(6, 36);
            this.comparisonSettingsPanel.Name = "comparisonSettingsPanel";
            this.comparisonSettingsPanel.Size = new System.Drawing.Size(282, 96);
            this.comparisonSettingsPanel.TabIndex = 9;
            // 
            // comparisonValueTextBox
            // 
            this.comparisonValueTextBox.Location = new System.Drawing.Point(171, 10);
            this.comparisonValueTextBox.Name = "comparisonValueTextBox";
            this.comparisonValueTextBox.Size = new System.Drawing.Size(100, 20);
            this.comparisonValueTextBox.TabIndex = 16;
            // 
            // comparisonElseValueTextBox
            // 
            this.comparisonElseValueTextBox.Location = new System.Drawing.Point(171, 61);
            this.comparisonElseValueTextBox.Name = "comparisonElseValueTextBox";
            this.comparisonElseValueTextBox.Size = new System.Drawing.Size(100, 20);
            this.comparisonElseValueTextBox.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(104, 64);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(63, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "else set it to";
            // 
            // comparisonIfValueTextBox
            // 
            this.comparisonIfValueTextBox.Location = new System.Drawing.Point(171, 35);
            this.comparisonIfValueTextBox.Name = "comparisonIfValueTextBox";
            this.comparisonIfValueTextBox.Size = new System.Drawing.Size(100, 20);
            this.comparisonIfValueTextBox.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(126, 38);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "set it to";
            // 
            // comparisonOperandComboBox
            // 
            this.comparisonOperandComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comparisonOperandComboBox.FormattingEnabled = true;
            this.comparisonOperandComboBox.Items.AddRange(new object[] {
            "=",
            ">",
            ">=",
            "<=",
            "<",
            "!=",
            ""});
            this.comparisonOperandComboBox.Location = new System.Drawing.Point(126, 10);
            this.comparisonOperandComboBox.Name = "comparisonOperandComboBox";
            this.comparisonOperandComboBox.Size = new System.Drawing.Size(39, 21);
            this.comparisonOperandComboBox.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(32, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "If current value is";
            // 
            // comparisonActiveCheckBox
            // 
            this.comparisonActiveCheckBox.AutoSize = true;
            this.comparisonActiveCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.comparisonActiveCheckBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.comparisonActiveCheckBox.Location = new System.Drawing.Point(6, 19);
            this.comparisonActiveCheckBox.Name = "comparisonActiveCheckBox";
            this.comparisonActiveCheckBox.Size = new System.Drawing.Size(282, 17);
            this.comparisonActiveCheckBox.TabIndex = 8;
            this.comparisonActiveCheckBox.Text = "Apply comparison to modify the current value";
            this.comparisonActiveCheckBox.UseVisualStyleBackColor = true;
            // 
            // ComparisonModifier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.comparisonSettingsGroupBox);
            this.Name = "ComparisonModifier";
            this.Size = new System.Drawing.Size(294, 278);
            this.comparisonSettingsGroupBox.ResumeLayout(false);
            this.comparisonSettingsGroupBox.PerformLayout();
            this.comparisonSettingsPanel.ResumeLayout(false);
            this.comparisonSettingsPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox comparisonSettingsGroupBox;
        private System.Windows.Forms.Panel comparisonSettingsPanel;
        private System.Windows.Forms.TextBox comparisonValueTextBox;
        private System.Windows.Forms.TextBox comparisonElseValueTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox comparisonIfValueTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comparisonOperandComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox comparisonActiveCheckBox;
    }
}
