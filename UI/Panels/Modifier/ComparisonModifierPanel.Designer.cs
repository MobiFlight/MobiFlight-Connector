namespace MobiFlight.UI.Panels.Modifier
{
    partial class ComparisonModifierPanel
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
            this.comparisonSettingsPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.comparisonValueTextBox = new System.Windows.Forms.TextBox();
            this.comparisonElseValueTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.comparisonIfValueTextBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.comparisonOperandComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comparisonSettingsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // comparisonSettingsPanel
            // 
            this.comparisonSettingsPanel.AutoSize = true;
            this.comparisonSettingsPanel.Controls.Add(this.label1);
            this.comparisonSettingsPanel.Controls.Add(this.button1);
            this.comparisonSettingsPanel.Controls.Add(this.comparisonValueTextBox);
            this.comparisonSettingsPanel.Controls.Add(this.comparisonElseValueTextBox);
            this.comparisonSettingsPanel.Controls.Add(this.label8);
            this.comparisonSettingsPanel.Controls.Add(this.comparisonIfValueTextBox);
            this.comparisonSettingsPanel.Controls.Add(this.label7);
            this.comparisonSettingsPanel.Controls.Add(this.comparisonOperandComboBox);
            this.comparisonSettingsPanel.Controls.Add(this.label6);
            this.comparisonSettingsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comparisonSettingsPanel.Location = new System.Drawing.Point(0, 0);
            this.comparisonSettingsPanel.Name = "comparisonSettingsPanel";
            this.comparisonSettingsPanel.Size = new System.Drawing.Size(400, 71);
            this.comparisonSettingsPanel.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(3, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(366, 24);
            this.label1.TabIndex = 20;
            this.label1.Text = "You can use ncalc expession syntax for more complex transformations.\r\n";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Image = global::MobiFlight.Properties.Resources.module_unknown;
            this.button1.Location = new System.Drawing.Point(375, 46);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(22, 22);
            this.button1.TabIndex = 21;
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // comparisonValueTextBox
            // 
            this.comparisonValueTextBox.Location = new System.Drawing.Point(51, 19);
            this.comparisonValueTextBox.Name = "comparisonValueTextBox";
            this.comparisonValueTextBox.Size = new System.Drawing.Size(100, 20);
            this.comparisonValueTextBox.TabIndex = 16;
            // 
            // comparisonElseValueTextBox
            // 
            this.comparisonElseValueTextBox.Location = new System.Drawing.Point(286, 19);
            this.comparisonElseValueTextBox.Name = "comparisonElseValueTextBox";
            this.comparisonElseValueTextBox.Size = new System.Drawing.Size(100, 20);
            this.comparisonElseValueTextBox.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(285, 3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(66, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "else, set it to";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comparisonIfValueTextBox
            // 
            this.comparisonIfValueTextBox.Location = new System.Drawing.Point(169, 19);
            this.comparisonIfValueTextBox.Name = "comparisonIfValueTextBox";
            this.comparisonIfValueTextBox.Size = new System.Drawing.Size(100, 20);
            this.comparisonIfValueTextBox.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(166, 3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "set it to";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            "!="});
            this.comparisonOperandComboBox.Location = new System.Drawing.Point(6, 19);
            this.comparisonOperandComboBox.Name = "comparisonOperandComboBox";
            this.comparisonOperandComboBox.Size = new System.Drawing.Size(39, 21);
            this.comparisonOperandComboBox.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(3, 3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "If current value is";
            // 
            // ComparisonModifierPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.comparisonSettingsPanel);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(400, 0);
            this.Name = "ComparisonModifierPanel";
            this.Size = new System.Drawing.Size(400, 71);
            this.comparisonSettingsPanel.ResumeLayout(false);
            this.comparisonSettingsPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel comparisonSettingsPanel;
        private System.Windows.Forms.TextBox comparisonValueTextBox;
        private System.Windows.Forms.TextBox comparisonElseValueTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox comparisonIfValueTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comparisonOperandComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
    }
}
