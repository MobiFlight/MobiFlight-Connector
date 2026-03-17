namespace MobiFlight.UI.Panels.Settings.Device
{
    partial class MFCustomDevicePanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MFCustomDevicePanel));
            this.groupBoxPinSettings = new System.Windows.Forms.GroupBox();
            this.groupBoxAdditionalConfig = new System.Windows.Forms.GroupBox();
            this.textBoxAdditionalConfig = new System.Windows.Forms.TextBox();
            this.groupBoxName = new System.Windows.Forms.GroupBox();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.mfCustomDevicePanelPin2 = new MobiFlight.UI.Panels.Settings.Device.MFCustomDevicePanelPin();
            this.mfCustomDevicePanelPin1 = new MobiFlight.UI.Panels.Settings.Device.MFCustomDevicePanelPin();
            this.groupBoxPinSettings.SuspendLayout();
            this.groupBoxAdditionalConfig.SuspendLayout();
            this.groupBoxName.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxPinSettings
            // 
            resources.ApplyResources(this.groupBoxPinSettings, "groupBoxPinSettings");
            this.groupBoxPinSettings.Controls.Add(this.mfCustomDevicePanelPin2);
            this.groupBoxPinSettings.Controls.Add(this.mfCustomDevicePanelPin1);
            this.groupBoxPinSettings.Name = "groupBoxPinSettings";
            this.groupBoxPinSettings.TabStop = false;
            // 
            // groupBoxAdditionalConfig
            // 
            this.groupBoxAdditionalConfig.Controls.Add(this.textBoxAdditionalConfig);
            resources.ApplyResources(this.groupBoxAdditionalConfig, "groupBoxAdditionalConfig");
            this.groupBoxAdditionalConfig.Name = "groupBoxAdditionalConfig";
            this.groupBoxAdditionalConfig.TabStop = false;
            // 
            // textBoxAdditionalConfig
            // 
            resources.ApplyResources(this.textBoxAdditionalConfig, "textBoxAdditionalConfig");
            this.textBoxAdditionalConfig.Name = "textBoxAdditionalConfig";
            this.textBoxAdditionalConfig.TextChanged += new System.EventHandler(this.value_Changed);
            // 
            // groupBoxName
            // 
            this.groupBoxName.Controls.Add(this.textBoxName);
            resources.ApplyResources(this.groupBoxName, "groupBoxName");
            this.groupBoxName.Name = "groupBoxName";
            this.groupBoxName.TabStop = false;
            // 
            // textBoxName
            // 
            resources.ApplyResources(this.textBoxName, "textBoxName");
            this.textBoxName.Name = "textBoxName";
            // 
            // mfCustomDevicePanelPin2
            // 
            resources.ApplyResources(this.mfCustomDevicePanelPin2, "mfCustomDevicePanelPin2");
            this.mfCustomDevicePanelPin2.Name = "mfCustomDevicePanelPin2";
            // 
            // mfCustomDevicePanelPin1
            // 
            resources.ApplyResources(this.mfCustomDevicePanelPin1, "mfCustomDevicePanelPin1");
            this.mfCustomDevicePanelPin1.Name = "mfCustomDevicePanelPin1";
            // 
            // MFCustomDevicePanel
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxName);
            this.Controls.Add(this.groupBoxAdditionalConfig);
            this.Controls.Add(this.groupBoxPinSettings);
            this.Name = "MFCustomDevicePanel";
            this.groupBoxPinSettings.ResumeLayout(false);
            this.groupBoxPinSettings.PerformLayout();
            this.groupBoxAdditionalConfig.ResumeLayout(false);
            this.groupBoxAdditionalConfig.PerformLayout();
            this.groupBoxName.ResumeLayout(false);
            this.groupBoxName.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxPinSettings;
        private System.Windows.Forms.GroupBox groupBoxAdditionalConfig;
        private System.Windows.Forms.TextBox textBoxAdditionalConfig;
        private MFCustomDevicePanelPin mfCustomDevicePanelPin2;
        private MFCustomDevicePanelPin mfCustomDevicePanelPin1;
        private System.Windows.Forms.GroupBox groupBoxName;
        private System.Windows.Forms.TextBox textBoxName;
    }
}
