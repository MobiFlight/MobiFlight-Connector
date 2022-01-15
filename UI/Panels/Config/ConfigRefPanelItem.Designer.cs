namespace MobiFlight.UI.Panels.Config
{
    partial class ConfigRefPanelItem
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigRefPanelItem));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.configRefComboBox = new System.Windows.Forms.ComboBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.removeConfigReferenceButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            // 
            // configRefComboBox
            // 
            resources.ApplyResources(this.configRefComboBox, "configRefComboBox");
            this.configRefComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.configRefComboBox.FormattingEnabled = true;
            this.configRefComboBox.Items.AddRange(new object[] {
            resources.GetString("configRefComboBox.Items"),
            resources.GetString("configRefComboBox.Items1"),
            resources.GetString("configRefComboBox.Items2")});
            this.configRefComboBox.Name = "configRefComboBox";
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // removeConfigReferenceButton
            // 
            resources.ApplyResources(this.removeConfigReferenceButton, "removeConfigReferenceButton");
            this.removeConfigReferenceButton.Name = "removeConfigReferenceButton";
            this.removeConfigReferenceButton.UseVisualStyleBackColor = true;
            this.removeConfigReferenceButton.Click += new System.EventHandler(this.removeConfigReferenceButton_Click);
            // 
            // ConfigRefPanelItem
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.removeConfigReferenceButton);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.configRefComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox1);
            this.Name = "ConfigRefPanelItem";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.ComboBox configRefComboBox;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button removeConfigReferenceButton;
    }
}
