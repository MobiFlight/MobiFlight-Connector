namespace MobiFlight.UI.Panels
{
    partial class InputActionPanel
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
            this.actionTypePanel = new System.Windows.Forms.Panel();
            this.actionTypeLabel = new System.Windows.Forms.Label();
            this.ActionTypeComboBox = new System.Windows.Forms.ComboBox();
            this.fsuipcOffsetActionTypePanel = new System.Windows.Forms.Panel();
            this.actionTypePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // actionTypePanel
            // 
            this.actionTypePanel.Controls.Add(this.actionTypeLabel);
            this.actionTypePanel.Controls.Add(this.ActionTypeComboBox);
            this.actionTypePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.actionTypePanel.Location = new System.Drawing.Point(0, 0);
            this.actionTypePanel.Name = "actionTypePanel";
            this.actionTypePanel.Size = new System.Drawing.Size(312, 34);
            this.actionTypePanel.TabIndex = 19;
            // 
            // actionTypeLabel
            // 
            this.actionTypeLabel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.actionTypeLabel.Location = new System.Drawing.Point(21, 10);
            this.actionTypeLabel.Name = "actionTypeLabel";
            this.actionTypeLabel.Size = new System.Drawing.Size(82, 13);
            this.actionTypeLabel.TabIndex = 18;
            this.actionTypeLabel.Text = "Action Type";
            this.actionTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ActionTypeComboBox
            // 
            this.ActionTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ActionTypeComboBox.FormattingEnabled = true;
            this.ActionTypeComboBox.Items.AddRange(new object[] {
            "FSUIPC Offset",
            "FSUIPC Macro",
            "Keyboard"});
            this.ActionTypeComboBox.Location = new System.Drawing.Point(109, 6);
            this.ActionTypeComboBox.MaximumSize = new System.Drawing.Size(122, 0);
            this.ActionTypeComboBox.MinimumSize = new System.Drawing.Size(47, 0);
            this.ActionTypeComboBox.Name = "ActionTypeComboBox";
            this.ActionTypeComboBox.Size = new System.Drawing.Size(122, 21);
            this.ActionTypeComboBox.TabIndex = 19;
            // 
            // fsuipcOffsetActionTypePanel
            // 
            this.fsuipcOffsetActionTypePanel.AutoSize = true;
            this.fsuipcOffsetActionTypePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.fsuipcOffsetActionTypePanel.Location = new System.Drawing.Point(0, 34);
            this.fsuipcOffsetActionTypePanel.Name = "fsuipcOffsetActionTypePanel";
            this.fsuipcOffsetActionTypePanel.Size = new System.Drawing.Size(312, 0);
            this.fsuipcOffsetActionTypePanel.TabIndex = 20;
            // 
            // InputActionPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fsuipcOffsetActionTypePanel);
            this.Controls.Add(this.actionTypePanel);
            this.Name = "InputActionPanel";
            this.Size = new System.Drawing.Size(312, 38);
            this.actionTypePanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel actionTypePanel;
        private System.Windows.Forms.Label actionTypeLabel;
        public System.Windows.Forms.ComboBox ActionTypeComboBox;
        private System.Windows.Forms.Panel fsuipcOffsetActionTypePanel;


    }
}
