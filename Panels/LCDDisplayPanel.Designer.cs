namespace MobiFlight.Panels
{
    partial class LCDDisplayPanel
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel3 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.DisplayComboBox = new System.Windows.Forms.ComboBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.lcdDisplayTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.configRefMainPanel = new System.Windows.Forms.Panel();
            this.configRefItemPanel = new System.Windows.Forms.Panel();
            this.configRefSpacerPanel = new System.Windows.Forms.Panel();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.configRefMainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.DisplayComboBox);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(337, 33);
            this.panel3.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Display";
            // 
            // DisplayComboBox
            // 
            this.DisplayComboBox.FormattingEnabled = true;
            this.DisplayComboBox.Location = new System.Drawing.Point(58, 6);
            this.DisplayComboBox.Name = "DisplayComboBox";
            this.DisplayComboBox.Size = new System.Drawing.Size(133, 21);
            this.DisplayComboBox.TabIndex = 3;
            this.DisplayComboBox.SelectedIndexChanged += new System.EventHandler(this.DisplayComboBox_SelectedIndexChanged);
            // 
            // panel4
            // 
            this.panel4.Location = new System.Drawing.Point(249, 33);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(200, 100);
            this.panel4.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.panel5);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.panel7);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 33);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(337, 61);
            this.panel1.TabIndex = 7;
            // 
            // panel5
            // 
            this.panel5.AutoSize = true;
            this.panel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel5.BackColor = System.Drawing.Color.Green;
            this.panel5.Controls.Add(this.panel6);
            this.panel5.Location = new System.Drawing.Point(58, 6);
            this.panel5.Name = "panel5";
            this.panel5.Padding = new System.Windows.Forms.Padding(5);
            this.panel5.Size = new System.Drawing.Size(152, 52);
            this.panel5.TabIndex = 11;
            // 
            // panel6
            // 
            this.panel6.AutoSize = true;
            this.panel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel6.BackColor = System.Drawing.Color.Black;
            this.panel6.Controls.Add(this.lcdDisplayTextBox);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(5, 5);
            this.panel6.Name = "panel6";
            this.panel6.Padding = new System.Windows.Forms.Padding(5);
            this.panel6.Size = new System.Drawing.Size(142, 42);
            this.panel6.TabIndex = 5;
            // 
            // lcdDisplayTextBox
            // 
            this.lcdDisplayTextBox.BackColor = System.Drawing.Color.RoyalBlue;
            this.lcdDisplayTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lcdDisplayTextBox.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lcdDisplayTextBox.ForeColor = System.Drawing.Color.White;
            this.lcdDisplayTextBox.Location = new System.Drawing.Point(5, 5);
            this.lcdDisplayTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.lcdDisplayTextBox.Multiline = true;
            this.lcdDisplayTextBox.Name = "lcdDisplayTextBox";
            this.lcdDisplayTextBox.Size = new System.Drawing.Size(132, 32);
            this.lcdDisplayTextBox.TabIndex = 1;
            this.lcdDisplayTextBox.Text = "1234567890123456\r\n######Test######\r\n1234567890123456\r\n######Test######";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(11, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 21);
            this.label2.TabIndex = 10;
            this.label2.Text = "Text";
            // 
            // panel7
            // 
            this.panel7.AutoSize = true;
            this.panel7.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel7.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel7.Location = new System.Drawing.Point(0, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(0, 61);
            this.panel7.TabIndex = 9;
            // 
            // configRefMainPanel
            // 
            this.configRefMainPanel.AutoSize = true;
            this.configRefMainPanel.Controls.Add(this.configRefItemPanel);
            this.configRefMainPanel.Controls.Add(this.configRefSpacerPanel);
            this.configRefMainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configRefMainPanel.Location = new System.Drawing.Point(0, 94);
            this.configRefMainPanel.Name = "configRefMainPanel";
            this.configRefMainPanel.Size = new System.Drawing.Size(337, 43);
            this.configRefMainPanel.TabIndex = 12;
            // 
            // configRefItemPanel
            // 
            this.configRefItemPanel.AutoSize = true;
            this.configRefItemPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configRefItemPanel.Location = new System.Drawing.Point(54, 0);
            this.configRefItemPanel.Name = "configRefItemPanel";
            this.configRefItemPanel.Size = new System.Drawing.Size(283, 43);
            this.configRefItemPanel.TabIndex = 1;
            // 
            // configRefSpacerPanel
            // 
            this.configRefSpacerPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.configRefSpacerPanel.Location = new System.Drawing.Point(0, 0);
            this.configRefSpacerPanel.Name = "configRefSpacerPanel";
            this.configRefSpacerPanel.Size = new System.Drawing.Size(54, 43);
            this.configRefSpacerPanel.TabIndex = 0;
            // 
            // LCDDisplayPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.configRefMainPanel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Name = "LCDDisplayPanel";
            this.Size = new System.Drawing.Size(337, 137);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.configRefMainPanel.ResumeLayout(false);
            this.configRefMainPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox DisplayComboBox;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TextBox lcdDisplayTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel configRefMainPanel;
        private System.Windows.Forms.Panel configRefItemPanel;
        private System.Windows.Forms.Panel configRefSpacerPanel;
    }
}
