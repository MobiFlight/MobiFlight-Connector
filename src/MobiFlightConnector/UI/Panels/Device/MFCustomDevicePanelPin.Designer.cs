namespace MobiFlight.UI.Panels.Settings.Device
{
    partial class MFCustomDevicePanelPin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MFCustomDevicePanelPin));
            this.panel1 = new System.Windows.Forms.Panel();
            this.pinLabel = new System.Windows.Forms.Label();
            this.comboBox0 = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pinLabel);
            this.panel1.Controls.Add(this.comboBox0);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // pinLabel
            // 
            resources.ApplyResources(this.pinLabel, "pinLabel");
            this.pinLabel.Name = "pinLabel";
            // 
            // comboBox0
            // 
            this.comboBox0.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox0.FormattingEnabled = true;
            resources.ApplyResources(this.comboBox0, "comboBox0");
            this.comboBox0.Name = "comboBox0";
            // 
            // MFCustomDevicePanelPin
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "MFCustomDevicePanelPin";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label pinLabel;
        private System.Windows.Forms.ComboBox comboBox0;
    }
}
