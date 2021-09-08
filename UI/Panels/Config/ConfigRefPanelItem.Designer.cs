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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.errorProvider1.SetError(this.textBox1, resources.GetString("textBox1.Error"));
            this.errorProvider1.SetIconAlignment(this.textBox1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("textBox1.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.textBox1, ((int)(resources.GetObject("textBox1.IconPadding"))));
            this.textBox1.Name = "textBox1";
            // 
            // configRefComboBox
            // 
            resources.ApplyResources(this.configRefComboBox, "configRefComboBox");
            this.errorProvider1.SetError(this.configRefComboBox, resources.GetString("configRefComboBox.Error"));
            this.configRefComboBox.FormattingEnabled = true;
            this.errorProvider1.SetIconAlignment(this.configRefComboBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("configRefComboBox.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.configRefComboBox, ((int)(resources.GetObject("configRefComboBox.IconPadding"))));
            this.configRefComboBox.Name = "configRefComboBox";
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.errorProvider1.SetError(this.checkBox1, resources.GetString("checkBox1.Error"));
            this.errorProvider1.SetIconAlignment(this.checkBox1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("checkBox1.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.checkBox1, ((int)(resources.GetObject("checkBox1.IconPadding"))));
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.errorProvider1.SetError(this.label3, resources.GetString("label3.Error"));
            this.errorProvider1.SetIconAlignment(this.label3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label3.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.label3, ((int)(resources.GetObject("label3.IconPadding"))));
            this.label3.Name = "label3";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            resources.ApplyResources(this.errorProvider1, "errorProvider1");
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.checkBox1);
            this.flowLayoutPanel1.Controls.Add(this.configRefComboBox);
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.textBox1);
            this.errorProvider1.SetError(this.flowLayoutPanel1, resources.GetString("flowLayoutPanel1.Error"));
            this.errorProvider1.SetIconAlignment(this.flowLayoutPanel1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("flowLayoutPanel1.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this.flowLayoutPanel1, ((int)(resources.GetObject("flowLayoutPanel1.IconPadding"))));
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // ConfigRefPanelItem
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.errorProvider1.SetError(this, resources.GetString("$this.Error"));
            this.errorProvider1.SetIconAlignment(this, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("$this.IconAlignment"))));
            this.errorProvider1.SetIconPadding(this, ((int)(resources.GetObject("$this.IconPadding"))));
            this.Name = "ConfigRefPanelItem";
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.ComboBox configRefComboBox;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}
