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
            this.textBoxPlaceholder = new System.Windows.Forms.TextBox();
            this.configRefComboBox = new System.Windows.Forms.ComboBox();
            this.checkBoxUse = new System.Windows.Forms.CheckBox();
            this.labelAs = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.removeConfigReferenceButton = new System.Windows.Forms.Button();
            this.labelTestValue = new System.Windows.Forms.Label();
            this.textBoxTestValue = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxPlaceholder
            // 
            resources.ApplyResources(this.textBoxPlaceholder, "textBoxPlaceholder");
            this.textBoxPlaceholder.Name = "textBoxPlaceholder";
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
            // checkBoxUse
            // 
            resources.ApplyResources(this.checkBoxUse, "checkBoxUse");
            this.checkBoxUse.Name = "checkBoxUse";
            this.checkBoxUse.UseVisualStyleBackColor = true;
            // 
            // labelAs
            // 
            resources.ApplyResources(this.labelAs, "labelAs");
            this.labelAs.Name = "labelAs";
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
            // labelTestValue
            // 
            resources.ApplyResources(this.labelTestValue, "labelTestValue");
            this.labelTestValue.Name = "labelTestValue";
            // 
            // textBoxTestValue
            // 
            resources.ApplyResources(this.textBoxTestValue, "textBoxTestValue");
            this.textBoxTestValue.Name = "textBoxTestValue";
            // 
            // ConfigRefPanelItem
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxTestValue);
            this.Controls.Add(this.labelTestValue);
            this.Controls.Add(this.removeConfigReferenceButton);
            this.Controls.Add(this.checkBoxUse);
            this.Controls.Add(this.configRefComboBox);
            this.Controls.Add(this.labelAs);
            this.Controls.Add(this.textBoxPlaceholder);
            this.Name = "ConfigRefPanelItem";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxPlaceholder;
        private System.Windows.Forms.CheckBox checkBoxUse;
        private System.Windows.Forms.Label labelAs;
        public System.Windows.Forms.ComboBox configRefComboBox;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Button removeConfigReferenceButton;
        private System.Windows.Forms.TextBox textBoxTestValue;
        private System.Windows.Forms.Label labelTestValue;
    }
}
