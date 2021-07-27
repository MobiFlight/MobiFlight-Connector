namespace MobiFlight.UI.Panels.Config
{
    partial class ActionTypePanel
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.ActionTypeComboBox = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.ActionTypeComboBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(339, 33);
            this.panel1.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(17, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Action Type";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // ActionTypeComboBox
            // 
            this.ActionTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ActionTypeComboBox.FormattingEnabled = true;
            this.ActionTypeComboBox.Items.AddRange(new object[] {
            "none",
            "FSUIPC Offset",
            "FSUIPC Macro",
            "Key"});
            this.ActionTypeComboBox.Location = new System.Drawing.Point(105, 2);
            this.ActionTypeComboBox.MaximumSize = new System.Drawing.Size(250, 0);
            this.ActionTypeComboBox.MinimumSize = new System.Drawing.Size(47, 0);
            this.ActionTypeComboBox.Name = "ActionTypeComboBox";
            this.ActionTypeComboBox.Size = new System.Drawing.Size(196, 21);
            this.ActionTypeComboBox.TabIndex = 17;
            // 
            // ActionTypePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "ActionTypePanel";
            this.Size = new System.Drawing.Size(339, 36);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox ActionTypeComboBox;
    }
}
