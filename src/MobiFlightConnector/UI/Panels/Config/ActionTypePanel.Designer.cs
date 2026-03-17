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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActionTypePanel));
            this.panel1 = new System.Windows.Forms.Panel();
            this.PasteButton = new System.Windows.Forms.Button();
            this.CopyButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ActionTypeComboBox = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.PasteButton);
            this.panel1.Controls.Add(this.CopyButton);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.ActionTypeComboBox);
            this.panel1.Name = "panel1";
            // 
            // PasteButton
            // 
            resources.ApplyResources(this.PasteButton, "PasteButton");
            this.PasteButton.Name = "PasteButton";
            this.PasteButton.UseVisualStyleBackColor = true;
            this.PasteButton.Click += new System.EventHandler(this.PasteButton_Click);
            // 
            // CopyButton
            // 
            resources.ApplyResources(this.CopyButton, "CopyButton");
            this.CopyButton.Name = "CopyButton";
            this.CopyButton.UseVisualStyleBackColor = true;
            this.CopyButton.Click += new System.EventHandler(this.CopyButton_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // ActionTypeComboBox
            // 
            this.ActionTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ActionTypeComboBox.FormattingEnabled = true;
            this.ActionTypeComboBox.Items.AddRange(new object[] {
            resources.GetString("ActionTypeComboBox.Items"),
            resources.GetString("ActionTypeComboBox.Items1"),
            resources.GetString("ActionTypeComboBox.Items2"),
            resources.GetString("ActionTypeComboBox.Items3")});
            resources.ApplyResources(this.ActionTypeComboBox, "ActionTypeComboBox");
            this.ActionTypeComboBox.Name = "ActionTypeComboBox";
            // 
            // ActionTypePanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "ActionTypePanel";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox ActionTypeComboBox;
        private System.Windows.Forms.Button PasteButton;
        private System.Windows.Forms.Button CopyButton;
    }
}
