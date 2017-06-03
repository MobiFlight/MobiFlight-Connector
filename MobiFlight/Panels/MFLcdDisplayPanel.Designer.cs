namespace MobiFlight.Panels
{
    partial class MFLcddDisplayPanel
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
            this.DiksplaySettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.LinesTextBox = new System.Windows.Forms.TextBox();
            this.ColTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.AddressTextBox = new System.Windows.Forms.TextBox();
            this.NameGroupBox = new System.Windows.Forms.GroupBox();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.DiksplaySettingsGroupBox.SuspendLayout();
            this.NameGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // DiksplaySettingsGroupBox
            // 
            this.DiksplaySettingsGroupBox.Controls.Add(this.label3);
            this.DiksplaySettingsGroupBox.Controls.Add(this.LinesTextBox);
            this.DiksplaySettingsGroupBox.Controls.Add(this.ColTextBox);
            this.DiksplaySettingsGroupBox.Controls.Add(this.label2);
            this.DiksplaySettingsGroupBox.Controls.Add(this.label1);
            this.DiksplaySettingsGroupBox.Controls.Add(this.AddressTextBox);
            this.DiksplaySettingsGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.DiksplaySettingsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.DiksplaySettingsGroupBox.Name = "DiksplaySettingsGroupBox";
            this.DiksplaySettingsGroupBox.Size = new System.Drawing.Size(185, 72);
            this.DiksplaySettingsGroupBox.TabIndex = 1;
            this.DiksplaySettingsGroupBox.TabStop = false;
            this.DiksplaySettingsGroupBox.Text = "Display settings";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(100, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Lines";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LinesTextBox
            // 
            this.LinesTextBox.Location = new System.Drawing.Point(134, 42);
            this.LinesTextBox.Name = "LinesTextBox";
            this.LinesTextBox.Size = new System.Drawing.Size(35, 20);
            this.LinesTextBox.TabIndex = 5;
            this.LinesTextBox.Text = "2";
            this.LinesTextBox.TextChanged += new System.EventHandler(this.value_Changed);
            // 
            // ColTextBox
            // 
            this.ColTextBox.Location = new System.Drawing.Point(59, 42);
            this.ColTextBox.Name = "ColTextBox";
            this.ColTextBox.Size = new System.Drawing.Size(35, 20);
            this.ColTextBox.TabIndex = 4;
            this.ColTextBox.Text = "16";
            this.ColTextBox.TextChanged += new System.EventHandler(this.value_Changed);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Columns";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Address";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AddressTextBox
            // 
            this.AddressTextBox.Location = new System.Drawing.Point(59, 19);
            this.AddressTextBox.Name = "AddressTextBox";
            this.AddressTextBox.Size = new System.Drawing.Size(35, 20);
            this.AddressTextBox.TabIndex = 1;
            this.AddressTextBox.TextChanged += new System.EventHandler(this.value_Changed);
            // 
            // NameGroupBox
            // 
            this.NameGroupBox.Controls.Add(this.NameTextBox);
            this.NameGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.NameGroupBox.Location = new System.Drawing.Point(0, 72);
            this.NameGroupBox.Name = "NameGroupBox";
            this.NameGroupBox.Size = new System.Drawing.Size(185, 48);
            this.NameGroupBox.TabIndex = 5;
            this.NameGroupBox.TabStop = false;
            this.NameGroupBox.Text = "Name";
            // 
            // NameTextBox
            // 
            this.NameTextBox.Location = new System.Drawing.Point(18, 19);
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.Size = new System.Drawing.Size(151, 20);
            this.NameTextBox.TabIndex = 0;
            this.NameTextBox.TextChanged += new System.EventHandler(this.value_Changed);
            // 
            // MFLcddDisplayPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.NameGroupBox);
            this.Controls.Add(this.DiksplaySettingsGroupBox);
            this.Name = "MFLcddDisplayPanel";
            this.Size = new System.Drawing.Size(185, 241);
            this.DiksplaySettingsGroupBox.ResumeLayout(false);
            this.DiksplaySettingsGroupBox.PerformLayout();
            this.NameGroupBox.ResumeLayout(false);
            this.NameGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox DiksplaySettingsGroupBox;
        private System.Windows.Forms.GroupBox NameGroupBox;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox LinesTextBox;
        private System.Windows.Forms.TextBox ColTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox AddressTextBox;
    }
}
